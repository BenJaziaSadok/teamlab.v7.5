/* 
 * 
 * (c) Copyright Ascensio System Limited 2010-2014
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * http://www.gnu.org/licenses/agpl.html 
 * 
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using System.Web.Configuration;
using System.Net;
using System.IO;
using ASC.Mail.Aggregator.Extension;
using ASC.Core.Tenants;
using Newtonsoft.Json.Linq;


namespace ASC.Mail.Aggregator
{
    public partial class MailBoxManager
    {
        public enum TariffType
        {
            Active = 0,
            Overdue,
            LongDead
        };

        #region public methods
        public bool LockMailbox(int mailbox_id, Int64 utc_ticks_time)
        {
            return LockMailbox(mailbox_id, utc_ticks_time, false, null);
        }

        public bool LockMailbox(int mailbox_id, Int64 utc_ticks_time, bool is_additional_proccessed_check_needed, DbManager external_db)
        {
            var update_query = new SqlUpdate(MAIL_MAILBOX)
                .Set(MailBoxFields.time_checked, utc_ticks_time)
                .Set(MailBoxFields.is_processed, true)
                .Where(MailBoxFields.id, mailbox_id);

            if (is_additional_proccessed_check_needed)
            {
                update_query = update_query
                               .Where(MailBoxFields.is_processed, false)
                               .Where(MailBoxFields.is_removed, false);
            }

            if (external_db == null)
            {
                using (var db = GetDb())
                {
                    return db.ExecuteNonQuery(update_query) > 0;
                }
            }

            return external_db.ExecuteNonQuery(update_query) > 0;
        }

        public List<int> KillOldTasks(int old_tasks_timeout_in_minutes)
        {
            // Reset is_processed field for potentially crushed aggregators
            List<int> old_tasks_list;
            var aggregator_timeout = TimeSpan.FromMinutes(old_tasks_timeout_in_minutes).Ticks;

            using (var db = GetDb())
            {
                var utc_ticks = DateTime.UtcNow.Ticks;

                old_tasks_list =
                    db.ExecuteList(
                        new SqlQuery(MAIL_MAILBOX)
                            .Select(MailBoxFields.id)
                            .Where(MailBoxFields.is_processed, true)
                            .Where(Exp.Gt(utc_ticks.ToString(CultureInfo.InvariantCulture), MailBoxFields.time_checked))
                            .Where(Exp.Gt(string.Format("{0} - {1}", utc_ticks, MailBoxFields.time_checked),
                                          aggregator_timeout)))
                      .ConvertAll(r => Convert.ToInt32(r[0]));

                if (old_tasks_list.Any())
                {
                    var mail_boxes = "";

                    old_tasks_list.ForEach(i => mail_boxes += i.ToString(CultureInfo.InvariantCulture) + "|");

                    db.ExecuteNonQuery(
                        new SqlUpdate(MAIL_MAILBOX)
                            .Set(MailBoxFields.is_processed, false)
                            .Where(Exp.In(MailBoxFields.id, old_tasks_list.ToArray())));
                }
            }
            return old_tasks_list;
        }

        public TariffType GetTariffType(int tenant_id)
        {
            TariffType result;
            CoreContext.TenantManager.SetCurrentTenant(tenant_id);
            var tenant_info = CoreContext.TenantManager.GetCurrentTenant();

            if (tenant_info.Status != TenantStatus.Active)
                return TariffType.LongDead;

            var response_api = GetApiResponse("portal/tariff.json", tenant_info);

            var date_string = JObject.Parse(response_api)["response"];
            var state = int.Parse(date_string["state"].ToString());

            if (state == 0 || state == 1) result = TariffType.Active;
            else
            {
                var due_date = DateTime.Parse(date_string["dueDate"].ToString());
                result = due_date.AddDays(30) <= DateTime.UtcNow ? TariffType.LongDead : TariffType.Overdue;
            }
            return result;
        }


        public MailBox GetMailboxForProcessing(TimeSpan activity_interval)
        {
            bool inactive_flag;

            lock (ticks)
            {
                inactive_flag = ticks.Tick();
            }

            MailBox mail;

            if (inactive_flag || null == (mail = GetActiveMailboxForProcessing(activity_interval)))
            {
                mail = GetInactiveMailboxForProcessing(activity_interval);
                if (mail != null)
                {
                    mail.Active = false;
                }
            }
            else mail.Active = true;

            return mail;
        }


        public void SetNextLoginDelayedFor(MailBox account, TimeSpan delay)
        {
            using (var db = GetDb())
            {
                var update_account_query = new SqlUpdate(MAIL_MAILBOX)
                    .Where(MailBoxFields.id, account.MailBoxId)
                    .Where(MailBoxFields.id_tenant, account.TenantId)
                    .Set(MailBoxFields.is_processed, false)
                    .Set(MailBoxFields.login_delay_expires, DateTime.UtcNow.Add(delay).Ticks);

                db.ExecuteNonQuery(update_account_query);
            }
        }


        private static SqlUpdate GetBaseUpdateAccountQueryOnMailboxProccessingComplete(MailBox account)
        {
            var utc_ticks_now = DateTime.UtcNow.Ticks;

            return new SqlUpdate(MAIL_MAILBOX)
                    .Where(MailBoxFields.id, account.MailBoxId)
                    .Set(MailBoxFields.is_processed, false)
                    .Set(MailBoxFields.msg_count_last, account.MessagesCount)
                    .Set(MailBoxFields.size_last, account.Size)
                    .Set(MailBoxFields.time_checked, utc_ticks_now); //Its needed for more uniform distribution in GetMailBoxForProccessing().
        }


        public void MailboxProcessingCompleted(MailBox account)
        {
            using (var db = GetDb())
            {
                var update_account_query = GetBaseUpdateAccountQueryOnMailboxProccessingComplete(account);
                if (account.ImapFolderChanged)
                {
                    update_account_query
                        .Where(MailBoxFields.begin_date, account.BeginDate)
                        .Set(MailBoxFields.imap_folders, account.ImapFoldersJson);
                }

                var result = db.ExecuteNonQuery(update_account_query);

                if (result == 0) // BeginDate has been changed
                {
                    db.ExecuteNonQuery(GetBaseUpdateAccountQueryOnMailboxProccessingComplete(account)
                                        .Set(MailBoxFields.imap_folders, "[]"));
                }
            }
        }


        // TODO: Implement new error proccessing
        public void MailboxProcessingError(MailBox account, Exception exception)
        {
            SetNextLoginDelayedFor(account, TimeSpan.FromSeconds(account.ServerLoginDelay));
        }


        public void SetMailboxQuotaError(MailBox account, bool state)
        {
            using (var db = GetDb())
            {
                db.ExecuteNonQuery(
                        new SqlUpdate(MAIL_MAILBOX)
                            .Where(MailBoxFields.id, account.MailBoxId)
                            .Where(MailBoxFields.id_tenant, account.TenantId)
                            .Set(MailBoxFields.quota_error, state));
            }
        }


        public void SetEmailLoginDelayExpires(string email, DateTime expires)
        {
            using (var db = GetDb())
            {
                db.ExecuteNonQuery(
                    new SqlUpdate(MAIL_MAILBOX)
                        .Where(MailBoxFields.address, email)
                        .Where(MailBoxFields.is_removed, false)
                        .Set(MailBoxFields.login_delay_expires, expires.Ticks));
            }
        }

        public void UpdateUserActivity(int id_tenant, string id_user)
        {
            using (var db = GetDb())
            {
                db.ExecuteNonQuery(
                   new SqlUpdate(MAIL_MAILBOX)
                       .Where(MailBoxFields.id_tenant, id_tenant)
                       .Where(MailBoxFields.id_user, id_user)
                       .Set(MailBoxFields.user_time_checked, DateTime.UtcNow.Ticks));
            }
        }

        public void SetAuthError(MailBox mailbox, bool error)
        {
            using (var db = GetDb())
            {
                var instr = new SqlUpdate(MAIL_MAILBOX)
                        .Where(MailBoxFields.id, mailbox.MailBoxId)
                        .Where(MailBoxFields.id_user, mailbox.UserId)
                        .Where(MailBoxFields.id_tenant, mailbox.TenantId);
                db.ExecuteNonQuery(error
                                       ? instr.Where(MailBoxFields.auth_error, null)
                                              .Set(MailBoxFields.auth_error, DateTime.UtcNow.Ticks)
                                       : instr.Set(MailBoxFields.auth_error, null));
            }
        }

        #endregion

        #region private methods

        public string GetAuthCookie(Tenant tenant_info)
        {
            return SecurityContext.AuthenticateMe(tenant_info.OwnerId);
        }

        public string GetAuthCookie(Guid user_id)
        {
            return SecurityContext.AuthenticateMe(user_id);
        }

        private string GetApiResponse(string api_url, Tenant tenant_info)
        {
            var request_uri_builder = new UriBuilder(Uri.UriSchemeHttp, CoreContext.TenantManager.GetCurrentTenant().TenantAlias);

            api_url = string.Format("{0}/{1}", WebConfigurationManager.AppSettings["api.url"].Trim('~', '/'), api_url.TrimStart('/'));

            if (CoreContext.TenantManager.GetCurrentTenant().TenantAlias == "localhost")
            {
                var virtual_dir = WebConfigurationManager.AppSettings["core.virtual-dir"];
                api_url = virtual_dir.Trim('/') + "/" + api_url;

                var host = WebConfigurationManager.AppSettings["core.host"];
                if (!string.IsNullOrEmpty(host)) request_uri_builder.Host = host;

                var port = WebConfigurationManager.AppSettings["core.port"];
                if (!string.IsNullOrEmpty(port)) request_uri_builder.Port = int.Parse(port);
            }
            else
                request_uri_builder.Host += "." + WebConfigurationManager.AppSettings["core.base-domain"];

            request_uri_builder.Path = api_url;

            var api_request = (HttpWebRequest)WebRequest.Create(request_uri_builder.Uri);
            api_request.Headers.Add("Payment-Info", "false");
            api_request.AllowAutoRedirect = true;
            api_request.CookieContainer = new CookieContainer();
            api_request.CookieContainer.Add(new Cookie("asc_auth_key", GetAuthCookie(tenant_info), "/", request_uri_builder.Host));

            using (var api_response = (HttpWebResponse)api_request.GetResponse())
            using (var resp_stream = api_response.GetResponseStream())
            {
                return resp_stream != null ? new StreamReader(resp_stream).ReadToEnd() : null;
            }
        }

        private MailBox GetActiveMailboxForProcessing(TimeSpan activity_interval)
        {
            var mail = GetMailboxForProcessing(activity_interval, "({0} - " + MailBoxFields.user_time_checked.Prefix(mail_mailbox_alias) + " ) < {1}");
            return mail;
        }

        private MailBox GetInactiveMailboxForProcessing(TimeSpan activity_interval)
        {
            var mail = GetMailboxForProcessing(activity_interval, "({0} - " + MailBoxFields.user_time_checked.Prefix(mail_mailbox_alias) + " ) > {1}");
            return mail;
        }

        private MailBox GetMailboxForProcessing(TimeSpan activity_interval, string where_usertime_sql_format)
        {
            using (var db = GetDb())
            {
                int? locker = 0;
                try
                {
                    locker = db.ExecuteScalar<int?>("SELECT GET_LOCK('lock_id', 5)");

                    if (locker == 1)
                    {
                        var utc_ticks = DateTime.UtcNow.Ticks;

                        var query = GetSelectMailBoxFieldsQuery()
                            .Where(MailBoxFields.is_processed.Prefix(mail_mailbox_alias), false)
                            .Where(string.Format(where_usertime_sql_format, utc_ticks, activity_interval.Ticks))
                            .Where(Exp.Le(MailBoxFields.login_delay_expires.Prefix(mail_mailbox_alias), utc_ticks))
                            .Where(Exp.And(Exp.Eq(MailBoxFields.is_removed.Prefix(mail_mailbox_alias), false), Exp.Eq(MailBoxFields.enabled.Prefix(mail_mailbox_alias), true)))
                            .OrderBy(MailBoxFields.time_checked.Prefix(mail_mailbox_alias), true)
                            .SetMaxResults(1);

                        var user_for_collection = WebConfigurationManager.AppSettings["mail.OneUserMode"];
                        if (!string.IsNullOrEmpty(user_for_collection))
                            query.Where(MailBoxFields.id_user, user_for_collection);


                        var list_results = db.ExecuteList(query);

                        var selected_box = list_results.ConvertAll(r => ToMailBox(r)).FirstOrDefault();

                        if (selected_box != null)
                        {
                            bool need_additional_checks = true;
                            var is_successed = LockMailbox(selected_box.MailBoxId, utc_ticks, need_additional_checks, db);
                            if (!is_successed)
                            {
                                selected_box = null;
                            }
                        }

                        return selected_box;
                    }
                }
                finally
                {
                    if (locker == 1)
                    {
                        db.ExecuteScalar<int>("SELECT RELEASE_LOCK('lock_id')");
                    }
                }
            }

            return null;
        }
        #endregion
    }
}
