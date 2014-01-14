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
using System.Linq;
using System.Threading;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Tenants;
using System.Net.Mail;
using System.IO;
using ASC.Mail.Aggregator.DbSchema;
using ASC.Mail.Aggregator.Extension;
using ActiveUp.Net.Mail;
using ASC.Mail.Aggregator.Authorization;

namespace ASC.Mail.Aggregator
{
    public partial class MailBoxManager
    {
        #region db defines

// ReSharper disable InconsistentNaming
        private const string MAIL_MAILBOX = "mail_mailbox";
        private const string MAIL_MAILBOX_PROVIDER = "mail_mailbox_provider";
        private const string MAIL_MAILBOX_DOMAIN = "mail_mailbox_domain";
        private const string MAIL_MAILBOX_SERVER = "mail_mailbox_server";
        public static readonly DateTime MIN_BEGIN_DATE = new DateTime(1975, 1, 1, 0, 0, 0);
// ReSharper restore InconsistentNaming

        private struct MailBoxFields
        {
            public const string id = "id";
            public const string id_user = "id_user";
            public const string id_tenant = "tenant";
            public const string address = "address";
            public const string enabled = "enabled";
            public const string password = "pop3_password";
            public const string msg_count_last = "msg_count_last";
            public const string size_last = "size_last";
            public const string smtp_password = "smtp_password";
            public const string name = "name";
            public const string login_delay = "login_delay";
            public const string time_checked = "time_checked";
            public const string is_processed = "is_processed";
            public const string user_time_checked = "user_time_checked";
            public const string login_delay_expires = "login_delay_expires";
            public const string is_removed = "is_removed";
            public const string quota_error = "quota_error";
            public const string auth_error = "auth_error";
            public const string imap = "imap";
            public const string begin_date = "begin_date";
            public const string service_type = "service_type";
            public const string refresh_token = "refresh_token";
            public const string imap_folders = "imap_folders";
            public const string id_smtp_server = "id_smtp_server";
            public const string id_in_server = "id_in_server";
        };

        private struct MailBoxProviderFields
        {
            public const string id = "id";
            public const string name = "name";
            public const string display_name = "display_name";
            public const string display_short_name = "display_short_name";
            public const string documentation = "documentation";
        };

        private struct MailBoxDomainFields
        {
            //public const string id = "id";
            public const string id_provider = "id_provider";
            public const string name = "name";
        };

        private struct MailBoxServerFields
        {
            public const string id = "id";
            public const string id_provider = "id_provider";
            public const string type = "type";
            public const string hostname = "hostname";
            public const string port = "port";
            public const string socket_type = "socket_type";
            public const string username = "username";
            public const string authentication = "authentication";
            public const string is_user_data = "is_user_data";
        };

        #endregion

        # region public methods

        public bool SaveMailBox(MailBox mail_box)
        {
            if (mail_box == null) throw new ArgumentNullException("mail_box");

            var id_mailbox = MailBoxExists(GetAddress(mail_box.EMail), mail_box.UserId, mail_box.TenantId);

            using (var db = GetDb())
            {
                int result;

                var login_delay_time = GetLoginDelayTime(mail_box);

                if (id_mailbox == 0)
                {
                    result = db.ExecuteScalar<int>(
                        new SqlInsert(MAIL_MAILBOX)
                            .InColumnValue(MailBoxFields.id, 0)
                            .InColumnValue(MailBoxFields.id_tenant, mail_box.TenantId)
                            .InColumnValue(MailBoxFields.id_user, mail_box.UserId)
                            .InColumnValue(MailBoxFields.address, GetAddress(mail_box.EMail))
                            .InColumnValue(MailBoxFields.name, mail_box.Name)
                            .InColumnValue(MailBoxFields.password, EncryptPassword(mail_box.Password))
                            .InColumnValue(MailBoxFields.msg_count_last, mail_box.MessagesCount)
                            .InColumnValue(MailBoxFields.smtp_password,
                                           string.IsNullOrEmpty(mail_box.SmtpPassword)
                                               ? EncryptPassword(mail_box.Password)
                                               : EncryptPassword(mail_box.SmtpPassword))
                            .InColumnValue(MailBoxFields.size_last, mail_box.Size)
                            .InColumnValue(MailBoxFields.login_delay, login_delay_time)
                            .InColumnValue(MailBoxFields.enabled, true)
                            .InColumnValue(MailBoxFields.imap, mail_box.Imap)
                            .InColumnValue(MailBoxFields.begin_date, mail_box.BeginDate)
                            .InColumnValue(MailBoxFields.service_type, mail_box.ServiceType)
                            .InColumnValue(MailBoxFields.refresh_token, mail_box.RefreshToken)
                            .InColumnValue(MailBoxFields.id_smtp_server, mail_box.SmtpServerId)
                            .InColumnValue(MailBoxFields.id_in_server, mail_box.InServerId)
                            .Identity(0, 0, true));

                    mail_box.MailBoxId = result;
                }
                else
                {
                    mail_box.MailBoxId = id_mailbox;

                    var query_update = new SqlUpdate(MAIL_MAILBOX)
                        .Where(MailBoxFields.id, id_mailbox)
                        .Set(MailBoxFields.id_tenant, mail_box.TenantId)
                        .Set(MailBoxFields.id_user, mail_box.UserId)
                        .Set(MailBoxFields.address, GetAddress(mail_box.EMail))
                        .Set(MailBoxFields.name, mail_box.Name)
                        .Set(MailBoxFields.password, EncryptPassword(mail_box.Password))
                        .Set(MailBoxFields.msg_count_last, mail_box.MessagesCount)
                        .Set(MailBoxFields.smtp_password,
                             string.IsNullOrEmpty(mail_box.SmtpPassword)
                                 ? EncryptPassword(mail_box.Password)
                                 : EncryptPassword(mail_box.SmtpPassword))
                        .Set(MailBoxFields.size_last, mail_box.Size)
                        .Set(MailBoxFields.login_delay, login_delay_time)
                        .Set(MailBoxFields.is_removed, false)
                        .Set(MailBoxFields.imap, mail_box.Imap)
                        .Set(MailBoxFields.begin_date, mail_box.BeginDate)
                        .Set(MailBoxFields.service_type, mail_box.ServiceType)
                        .Set(MailBoxFields.refresh_token, mail_box.RefreshToken)
                        .Set(MailBoxFields.id_smtp_server, mail_box.SmtpServerId)
                        .Set(MailBoxFields.id_in_server, mail_box.InServerId);

                    if (mail_box.BeginDate == MIN_BEGIN_DATE)
                        query_update.Set(MailBoxFields.imap_folders, "[]");

                    result = db.ExecuteNonQuery(query_update);
                }

                return result > 0;
            }
        }



        public List<MailBox> GetMailBoxes(int id_tenant, string id_user)
        {
            var where = Exp.Empty;
            if (id_tenant != Tenant.DEFAULT_TENANT) where = where & Exp.Eq(MailBoxFields.id_tenant, id_tenant);
            if (!string.IsNullOrEmpty(id_user)) where = where & Exp.Eq(MailBoxFields.id_user, id_user.ToLowerInvariant());
            return GetMailBoxes(where);
        }

        public MailBox GetMailBox(int id_tenant, string id_user, MailAddress email)
        {
            return GetMailBoxes(Exp.Eq(MailBoxFields.address.Prefix(mail_mailbox_alias), GetAddress(email)) &
                Exp.Eq(MailBoxFields.id_tenant.Prefix(mail_mailbox_alias), id_tenant) &
                Exp.Eq(MailBoxFields.id_user.Prefix(mail_mailbox_alias), id_user.ToLowerInvariant()))
                .SingleOrDefault();
        }

        public bool UpdateEnableFlag(int id_tenant, string id_user, string email, bool new_value)
        {
            var id_mailbox = MailBoxExists(email, id_user, id_tenant);

            using (var db = GetDb())
            {
                var result = db.ExecuteNonQuery(
                    new SqlUpdate(MAIL_MAILBOX)
                        .Where(MailBoxFields.id, id_mailbox)
                        .Set(MailBoxFields.enabled, new_value));

                return result > 0;
            }
        }

        public void RemoveMailBox(MailBox mail_box)
        {
            if (mail_box.MailBoxId <= 0)
                throw new Exception("MailBox id is 0");

            long total_attachments_size;

            using (var db = GetDb())
            {
                using (var tx = db.BeginTransaction())
                {
                    db.ExecuteNonQuery(
                        new SqlUpdate(MAIL_MAILBOX)
                            .Set(MailBoxFields.is_removed, true)
                            .Where(MailBoxFields.id, mail_box.MailBoxId));

                    db.ExecuteNonQuery(
                        new SqlDelete(ChainTable.name)
                        .Where(GetUserWhere(mail_box.UserId, mail_box.TenantId))
                        .Where(ChainTable.Columns.id_mailbox, mail_box.MailBoxId));


                    db.ExecuteNonQuery(
                        new SqlUpdate(MailTable.name)
                            .Set(MailTable.Columns.is_removed, true)
                            .Where(MailTable.Columns.id_mailbox, mail_box.MailBoxId)
                            .Where(GetUserWhere(mail_box.UserId, mail_box.TenantId)));

                    total_attachments_size = db.ExecuteScalar<long>(
                        string.Format(
                            "select sum(a.size) from {0} a inner join {1} m on a.{2} = m.{3} where m.{4} = {5} and m.{6} = {7} and a.{8} != {9}",
                            AttachmentTable.name,
                            MailTable.name,
                            AttachmentTable.Columns.id_mail,
                            MailTable.Columns.id,
                            MailTable.Columns.id_mailbox,
                            mail_box.MailBoxId,
                            MailTable.Columns.id_tenant,
                            mail_box.TenantId,
                            AttachmentTable.Columns.need_remove,
                            1));

                    var query =
                        string.Format(
                            "Update {0} a INNER JOIN {1} m ON a.{2} = m.{3} SET a.{4} = {5} WHERE m.{6} = {7}",
                            AttachmentTable.name, MailTable.name, AttachmentTable.Columns.id_mail, MailTable.Columns.id,
                            AttachmentTable.Columns.need_remove, true, MailTable.Columns.id_mailbox, mail_box.MailBoxId);

                    db.ExecuteNonQuery(query);

                    query = string.Format("Select t.{0} FROM {1} t INNER JOIN {2} m ON t.{3} = m.{4} WHERE m.{5} = {6}",
                                          TagMailFields.id_tag, MAIL_TAG_MAIL, MailTable.name, TagMailFields.id_mail, MailTable.Columns.id,
                                          MailTable.Columns.id_mailbox, mail_box.MailBoxId);

                    var affected_tags = db.ExecuteList(query).ConvertAll(r => Convert.ToInt32(r[0])).Distinct();

                    query = string.Format("Delete t FROM {0} t INNER JOIN {1} m ON t.{2} = m.{3} WHERE m.{4} = {5}",
                                          MAIL_TAG_MAIL, MailTable.name, TagMailFields.id_mail, MailTable.Columns.id,
                                          MailTable.Columns.id_mailbox, mail_box.MailBoxId);

                    db.ExecuteNonQuery(query);

                    UpdateTagsCount(db, mail_box.TenantId, mail_box.UserId, affected_tags);

                    RecalculateFolders(db, mail_box.TenantId, mail_box.UserId);

                    tx.Commit();
                }
            }

            QuotaUsedDelete(mail_box.TenantId, total_attachments_size);
        }

        public clientConfig GetMailBoxSettings(string host)
        {
            using (var db = GetDb())
            {
                var id_provider = db.ExecuteScalar<int>(
                                new SqlQuery(MAIL_MAILBOX_DOMAIN)
                                    .Select(MailBoxDomainFields.id_provider)
                                    .Where(MailBoxDomainFields.name, host));

                if (id_provider < 1)
                    return null;

                var config = new clientConfig();

                config.emailProvider.domain.Add(host);

                var provider = db.ExecuteList(
                    new SqlQuery(MAIL_MAILBOX_PROVIDER)
                        .Select(MailBoxProviderFields.name, MailBoxProviderFields.display_name,
                        MailBoxProviderFields.display_short_name, MailBoxProviderFields.documentation)
                        .Where(MailBoxProviderFields.id, id_provider))
                        .FirstOrDefault();

                if (provider == null)
                    return null;

                config.emailProvider.id = Convert.ToString(provider[0]);
                config.emailProvider.displayName = Convert.ToString(provider[1]);
                config.emailProvider.displayShortName = Convert.ToString(provider[2]);
                config.emailProvider.documentation.url = Convert.ToString(provider[3]);

                var servers = db.ExecuteList(
                    new SqlQuery(MAIL_MAILBOX_SERVER)
                        .Select(MailBoxServerFields.hostname, MailBoxServerFields.port, MailBoxServerFields.type,
                        MailBoxServerFields.socket_type, MailBoxServerFields.username, MailBoxServerFields.authentication)
                        .Where(MailBoxServerFields.id_provider, id_provider)
                        .Where(MailBoxServerFields.is_user_data, false)); //This condition excludes new data from mail_mailbox_server. That needed for resolving security issues.

                if (servers.Count == 0)
                    return null;

                servers.ForEach(serv =>
                {
                    var hostname = Convert.ToString(serv[0]);
                    var port = Convert.ToInt32(serv[1]);
                    var type = Convert.ToString(serv[2]);
                    var socket_type = Convert.ToString(serv[3]);
                    var username = Convert.ToString(serv[4]);
                    var authentication = Convert.ToString(serv[5]);

                    if (type == "smtp")
                    {
                        config.emailProvider.outgoingServer.Add(new clientConfigEmailProviderOutgoingServer
                            {
                                type = type,
                                socketType = socket_type,
                                hostname = hostname,
                                port = port,
                                username = username,
                                authentication = authentication
                            });
                    }
                    else
                    {
                        config.emailProvider.incomingServer.Add(new clientConfigEmailProviderIncomingServer
                            {
                                type = type,
                                socketType = socket_type,
                                hostname = hostname,
                                port = port,
                                username = username,
                                authentication = authentication
                            });
                    }

                });

                return config;
            }
        }

        public bool SetMailBoxSettings(clientConfig config)
        {
            try
            {
                if (string.IsNullOrEmpty(config.emailProvider.id) ||
                    config.emailProvider.incomingServer == null ||
                    !config.emailProvider.incomingServer.Any() ||
                    config.emailProvider.outgoingServer == null ||
                    !config.emailProvider.outgoingServer.Any())
                    throw new Exception("Incorrect config");

                using (var db = GetDb())
                {
                    using (var tx = db.BeginTransaction())
                    {
                        var id_provider = db.ExecuteScalar<int>(
                            new SqlQuery(MAIL_MAILBOX_PROVIDER)
                                .Select(MailBoxProviderFields.id)
                                .Where(MailBoxProviderFields.name, config.emailProvider.id));

                        if (id_provider < 1)
                        {
                            id_provider = db.ExecuteScalar<int>(
                                new SqlInsert(MAIL_MAILBOX_PROVIDER)
                                    .InColumnValue(MailBoxProviderFields.id, 0)
                                    .InColumnValue(MailBoxProviderFields.name, config.emailProvider.id)
                                    .InColumnValue(MailBoxProviderFields.display_name, config.emailProvider.displayName)
                                    .InColumnValue(MailBoxProviderFields.display_short_name,
                                                   config.emailProvider.displayShortName)
                                    .InColumnValue(MailBoxProviderFields.documentation,
                                                   config.emailProvider.documentation.url)
                                    .Identity(0, 0, true));

                            if (id_provider < 1)
                                throw new Exception("id_provider not saved in DB");
                        }

                        var insert_query = new SqlInsert(MAIL_MAILBOX_DOMAIN)
                            .IgnoreExists(true)
                            .InColumns(MailBoxDomainFields.id_provider, MailBoxDomainFields.name);

                        config.emailProvider.domain
                              .ForEach(domain =>
                                       insert_query
                                           .Values(id_provider, domain));

                        db.ExecuteNonQuery(insert_query);

                        insert_query = new SqlInsert(MAIL_MAILBOX_SERVER)
                            .IgnoreExists(true)
                            .InColumns(
                                MailBoxServerFields.id_provider,
                                MailBoxServerFields.type,
                                MailBoxServerFields.hostname,
                                MailBoxServerFields.port,
                                MailBoxServerFields.socket_type,
                                MailBoxServerFields.username,
                                MailBoxServerFields.authentication
                            );

                        config.emailProvider.incomingServer
                              .ForEach(server =>
                                       insert_query
                                           .Values(id_provider,
                                                   server.type,
                                                   server.hostname,
                                                   server.port,
                                                   server.socketType,
                                                   server.username,
                                                   server.authentication));

                        config.emailProvider.outgoingServer
                              .ForEach(server =>
                                       insert_query
                                           .Values(id_provider,
                                                   server.type,
                                                   server.hostname,
                                                   server.port,
                                                   server.socketType,
                                                   server.username,
                                                   server.authentication));

                        db.ExecuteNonQuery(insert_query);

                        tx.Commit();
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static EncryptionType ConvertToEncryptionType(string type_string)
        {
            switch (type_string.ToLower().Trim())
            {
                case "ssl":
                    return EncryptionType.SSL;
                case "starttls":
                    return EncryptionType.StartTLS;
                case "plain":
                    return EncryptionType.None;
                default:
                    throw new ArgumentException("Unknown mail server socket type: " + type_string);
            }
        }

        private static string ConvertFromEncryptionType(EncryptionType enc_type)
        {
            switch (enc_type)
            {
                case EncryptionType.SSL:
                    return "SSL";
                case EncryptionType.StartTLS:
                    return "STARTTLS";
                case EncryptionType.None:
                    return "plain";
                default:
                    throw new ArgumentException("Unknown mail server EncryptionType: " + Enum.GetName(typeof(EncryptionType), enc_type));
            }
        }

        private static SaslMechanism ConvertToSaslMechanism(string type_string)
        {
            switch (type_string.ToLower().Trim())
            {
                case "":
                case "oauth2":
                case "password-cleartext":
                    return SaslMechanism.Login;
                case "none":
                    return SaslMechanism.None;
                case "password-encrypted":
                    return SaslMechanism.CramMd5;
                default:
                    throw new ArgumentException("Unknown mail server authentication type: " + type_string);
            }
        }

        private static string ConvertFromSaslMechanism(SaslMechanism sasl_type)
        {
            switch (sasl_type)
            {
                case SaslMechanism.Login:
                    return "";
                case SaslMechanism.None:
                    return "none";
                case SaslMechanism.CramMd5:
                    return "password-encrypted";
                default:
                    throw new ArgumentException("Unknown mail server SaslMechanism: " + Enum.GetName(typeof(SaslMechanism), sasl_type));
            }
        }

        public MailBox ObtainMailboxSettings(int id_tenant, string id_user, string email, string password,
            AuthorizationServiceType type, bool? imap, bool is_null_needed)
        {
            var address = new MailAddress(email);

            var host = address.Host.ToLowerInvariant();

            if (type == AuthorizationServiceType.Google) host = GoogleHost;

            MailBox initial_value = null;

            if (imap.HasValue)
            {
                try
                {
                    var settings = GetMailBoxSettings(host);

                    if (settings != null)
                    {
                        var outgoing_server_login = "";

                        var incomming_type = imap.Value ? "imap" : "pop3";

                        var incoming_server =
                            settings.emailProvider.incomingServer
                            .FirstOrDefault(serv =>
                                serv.type
                                .ToLowerInvariant()
                                .Equals(incomming_type));

                        var outgoing_server = settings.emailProvider.outgoingServer.FirstOrDefault() ?? new clientConfigEmailProviderOutgoingServer();

                        if (incoming_server != null && !string.IsNullOrEmpty(incoming_server.username))
                        {
                            var incoming_server_login = FormatLoginFromDb(incoming_server.username, address);

                            if (!string.IsNullOrEmpty(outgoing_server.username))
                            {
                                outgoing_server_login = FormatLoginFromDb(outgoing_server.username, address);
                            }

                            initial_value = new MailBox
                            {
                                EMail = address,
                                Name = "",

                                Account = incoming_server_login,
                                Password = password,
                                Server = FormatServerFromDb(incoming_server.hostname, host),
                                Port = incoming_server.port,
                                IncomingEncryptionType = ConvertToEncryptionType(incoming_server.socketType),
                                OutcomingEncryptionType = ConvertToEncryptionType(outgoing_server.socketType),
                                AuthenticationTypeIn = ConvertToSaslMechanism(incoming_server.authentication),
                                AuthenticationTypeSmtp = ConvertToSaslMechanism(outgoing_server.authentication),
                                Imap = imap.Value,

                                SmtpAccount = outgoing_server_login,
                                SmtpPassword = password,
                                SmtpServer = FormatServerFromDb(outgoing_server.hostname, host),
                                SmtpPort = outgoing_server.port,
                                SmtpAuth = !string.IsNullOrEmpty(outgoing_server.username),

                                Enabled = true,
                                TenantId = id_tenant,
                                UserId = id_user,
                                Restrict = true,
                                BeginDate = DateTime.Now.Subtract(new TimeSpan(MailBox.DefaultMailLimitedTimeDelta)),
                                ServiceType = (byte) type
                            };
                        }
                    }
                }
                catch (Exception)
                {
                    initial_value = null;
                }
            }

            if(initial_value != null || is_null_needed)
            {
                return initial_value;
            }

            bool is_imap = imap.GetValueOrDefault(true);
            return new MailBox
                {
                    EMail = address,
                    Name = "",
                    Account = email,
                    Password = password,
                    Server = string.Format((is_imap ? "imap.{0}" : "pop.{0}"), host),
                    Port = (is_imap ? 993 : 110),
                    IncomingEncryptionType = is_imap ? EncryptionType.SSL : EncryptionType.None,
                    OutcomingEncryptionType = EncryptionType.None,
                    Imap = is_imap,
                    SmtpAccount = email,
                    SmtpPassword = password,
                    SmtpServer = string.Format("smtp.{0}", host),
                    SmtpPort = 25,
                    SmtpAuth = true,
                    Enabled = true,
                    TenantId = id_tenant,
                    UserId = id_user,
                    Restrict = true,
                    BeginDate = DateTime.Now.Subtract(new TimeSpan(MailBox.DefaultMailLimitedTimeDelta)),
                    AuthenticationTypeIn = SaslMechanism.Login,
                    AuthenticationTypeSmtp = SaslMechanism.Login
                };
        }


        public MailBox SearchMailboxSettings(string email, string password, string id_user, int id_tenant)
        {
            //Mailbox splitted for excluding data race from cocurency.
            //This is out mailbox. In that we store smtp settings.
            var mbox = new MailBox
                {
                    SmtpServer = String.Format("smtp.{0}", email.Substring(email.IndexOf('@') + 1)),
                    SmtpAccount = email,
                    Account = email,
                    EMail = new MailAddress(email),
                    SmtpAuth = true,
                    SmtpPassword = password,
                    Password = password,
                    Name = ""
                };
            //This mailbox using by pop_imap_search_thread for result storing.
            var mbox_in = new MailBox();

            var settings_from_db = GetMailBoxSettings(email.Substring(email.IndexOf('@') + 1));

            bool is_smtp_failed = false;
            bool is_successed_in = false;
            string last_in_error_pop = String.Empty;
            string last_in_error_imap = String.Empty;
            var pop_imap_search_thread = new Thread(() =>
            {
                mbox_in.Imap = true;
                mbox_in.Server = String.Format("imap.{0}", email.Substring(email.IndexOf('@') + 1));
                foreach (var settings in GetImapSettingsVariants(email, password, mbox_in, settings_from_db))
                {
                    if (is_smtp_failed) return;
                    if (MailServerHelper.TryTestImap(settings, out last_in_error_pop))
                    {
                        mbox_in.Account = settings.AccountName;
                        mbox_in.Password = settings.AccountPass;
                        mbox_in.Server = settings.Url;
                        mbox_in.Port = settings.Port;
                        mbox_in.AuthenticationTypeIn = settings.AuthenticationType;
                        mbox_in.IncomingEncryptionType = settings.EncryptionType;
                        is_successed_in = true;
                        break;
                    }
                }

                if (!is_successed_in && !is_smtp_failed)
                {
                    mbox_in.Imap = false;
                    mbox_in.Server = String.Format("pop.{0}", email.Substring(email.IndexOf('@') + 1));
                    foreach (var settings in GetPopSettingsVariants(email, password, mbox_in, settings_from_db))
                    {
                        if (is_smtp_failed) return;
                        if (MailServerHelper.TryTestPop(settings, out last_in_error_imap))
                        {
                            mbox_in.Account = settings.AccountName;
                            mbox_in.Password = settings.AccountPass;
                            mbox_in.Server = settings.Url;
                            mbox_in.Port = settings.Port;
                            mbox_in.AuthenticationTypeIn = settings.AuthenticationType;
                            mbox_in.IncomingEncryptionType = settings.EncryptionType;
                            is_successed_in = true;
                            break;
                        }
                    }
                }
            });

            pop_imap_search_thread.Start();

            string last_error = String.Empty;
            bool is_successed = false;
            foreach (var settings in GetSmtpSettingsVariants(email, password, mbox, settings_from_db))
            {
                if(MailServerHelper.TryTestSmtp(settings, out last_error))
                {
                    mbox.SmtpPassword = settings.AccountPass;
                    mbox.SmtpAccount = settings.AccountName;
                    mbox.SmtpServer = settings.Url;
                    mbox.SmtpPort = settings.Port;
                    mbox.AuthenticationTypeSmtp = settings.AuthenticationType;
                    mbox.OutcomingEncryptionType = settings.EncryptionType;
                    is_successed = true;
                    break;
                }
            }

            if (!is_successed)
            {
                is_smtp_failed = true;
                Thread.Sleep(0);
                throw new SmtpConnectionException(last_error);
            }

            pop_imap_search_thread.Join(30000);

            if (!is_successed_in)
            {
                if (last_in_error_pop == String.Empty && last_in_error_imap == String.Empty)
                {
                    last_in_error_imap = MailQueueItem.CONNECTION_TIMEOUT_ERROR;
                }
                throw new ImapConnectionException(String.Format("{0}\n{1}", last_in_error_pop, last_in_error_imap));
            }

            mbox.Imap = mbox_in.Imap;
            mbox.Account = mbox_in.Account;
            mbox.Password = mbox_in.Password;
            mbox.IncomingEncryptionType = mbox_in.IncomingEncryptionType;
            mbox.AuthenticationTypeIn = mbox_in.AuthenticationTypeIn;
            mbox.Server = mbox_in.Server;
            mbox.Port = mbox_in.Port;
            mbox.UserId = id_user;
            mbox.TenantId = id_tenant;
            mbox.Restrict = true;
            mbox.BeginDate = DateTime.Now.Subtract(new TimeSpan(MailBox.DefaultMailLimitedTimeDelta));

            return mbox;
        }

        public int SaveMailServerSettings(MailAddress email, MailServerSettings settings, string server_type, 
            AuthorizationServiceType authorization_type)
        {
            var host = (authorization_type == AuthorizationServiceType.Google) ? GoogleHost : email.Host; 

            using (var db = GetDb())
            {
                var provider_id = db.ExecuteScalar<int>(new SqlQuery(MAIL_MAILBOX_DOMAIN)
                                                            .Select(MailBoxDomainFields.id_provider)
                                                            .Where(MailBoxProviderFields.name, host));

                //Save Mailbox provider if not exists
                if (provider_id == 0)
                {
                    provider_id = db.ExecuteScalar<int>(new SqlInsert(MAIL_MAILBOX_PROVIDER)
                                                            .InColumnValue(MailBoxProviderFields.id, 0)
                                                            .InColumnValue(MailBoxProviderFields.name, email.Host)
                                                            .Identity(0, 0, true));
                    db.ExecuteNonQuery(new SqlInsert(MAIL_MAILBOX_DOMAIN)
                                                            .InColumnValue(MailBoxDomainFields.id_provider, provider_id)
                                                            .InColumnValue(MailBoxDomainFields.name, email.Host));
                }

                //Identify mask for account name
                var account_name_mask = "";
                if (settings.AuthenticationType != SaslMechanism.None)
                {
                    account_name_mask = GetLoginFormatFrom(email, settings.AccountName);
                    if (String.IsNullOrEmpty(account_name_mask))
                    {
                        account_name_mask = settings.AccountName;
                    }
                }

                var settings_id = db.ExecuteScalar<int>(new SqlQuery(MAIL_MAILBOX_SERVER)
                    .Select(MailBoxServerFields.id)
                    .Where(MailBoxServerFields.id_provider, provider_id)
                    .Where(MailBoxServerFields.type, server_type)
                    .Where(MailBoxServerFields.hostname, settings.Url)
                    .Where(MailBoxServerFields.port, settings.Port)
                    .Where(MailBoxServerFields.socket_type,
                           ConvertFromEncryptionType(settings.EncryptionType))
                     .Where(MailBoxServerFields.authentication,
                                    ConvertFromSaslMechanism(settings.AuthenticationType))
                     .Where(MailBoxServerFields.username, account_name_mask)
                     .Where(MailBoxServerFields.is_user_data, false));

                if (settings_id == 0)
                {
                    settings_id = db.ExecuteScalar<int>(new SqlInsert(MAIL_MAILBOX_SERVER)
                                           .InColumnValue(MailBoxServerFields.id, 0)
                                           .InColumnValue(MailBoxServerFields.id_provider, provider_id)
                                           .InColumnValue(MailBoxServerFields.type, server_type)
                                           .InColumnValue(MailBoxServerFields.hostname, settings.Url)
                                           .InColumnValue(MailBoxServerFields.port, settings.Port)
                                           .InColumnValue(MailBoxServerFields.socket_type,
                                                          ConvertFromEncryptionType(settings.EncryptionType))
                                           .InColumnValue(MailBoxServerFields.authentication,
                                                          ConvertFromSaslMechanism(settings.AuthenticationType))
                                           .InColumnValue(MailBoxServerFields.username, account_name_mask)
                                           .InColumnValue(MailBoxServerFields.is_user_data, true)
                                           .Identity(0,0, true));
                }

                return settings_id;
            }
        }
        #endregion

        #region private methods

        private List<MailBox> GetMailBoxes(Exp where)
        {
            using (var db = GetDb())
            {
                var query = GetSelectMailBoxFieldsQuery()
                    .Where(MailBoxFields.is_removed.Prefix(mail_mailbox_alias), false)
                    .Where(where)
                    .OrderBy(1, true)
                    .OrderBy(2, true);

                var res = db.ExecuteList(query)
                    .ConvertAll(r => ToMailBox(r));

                return res;
            }
        }

        private enum MailBoxFieldSelectPosition
        {
            IdTenant, IdUser, Name, Address, Account, Password, InServer, InPort, SizeLast, MsgCountLast, SmtpServer, SmtpPort,
            SmtpPassword, SmtpAccount, LoginDelay, Id, Enabled, QuotaError, AuthError, Imap, BeginDate,
            ServiceType, RefreshToken, ImapFolders, OutcomingEncryptionType, IncomingEncryptionType,
            AuthTypeIn, AuthtTypeSmtp, IdSmtpServer, IdInServer
        };

        private enum MailItemAttachmentSelectPosition
        {
            Id, Name, StoredName, Type, Size, FileNumber, IdStream, Tenant, User, ContentId
        }

        private const int SelectMailBoxFieldsCont = 30;
        private const int SelectMailItemAttachmentFieldsCont = 10;

        private const string mail_mailbox_alias = "mm";
        private const string smtp_alias = "smtp";
        private const string in_alias = "ins";


        private static SqlQuery GetSelectMailBoxFieldsQuery()
        {
            var fields_for_select = new string[SelectMailBoxFieldsCont];
            fields_for_select[(int)MailBoxFieldSelectPosition.IdTenant] = MailBoxFields.id_tenant.Prefix(mail_mailbox_alias);
            fields_for_select[(int)MailBoxFieldSelectPosition.IdUser] = MailBoxFields.id_user.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.Name ] = MailBoxFields.name.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.Address ] = MailBoxFields.address.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.Account ] = MailBoxServerFields.username.Prefix(in_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.Password ] = MailBoxFields.password.Prefix(mail_mailbox_alias);
            fields_for_select[(int)MailBoxFieldSelectPosition.InServer] = MailBoxServerFields.hostname.Prefix(in_alias);
            fields_for_select[(int)MailBoxFieldSelectPosition.InPort] = MailBoxServerFields.port.Prefix(in_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.SizeLast ] = MailBoxFields.size_last.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.MsgCountLast ] = MailBoxFields.msg_count_last.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.SmtpServer ] = MailBoxServerFields.hostname.Prefix(smtp_alias);
            fields_for_select[(int)MailBoxFieldSelectPosition.SmtpPort] = MailBoxServerFields.port.Prefix(smtp_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.SmtpPassword ] = MailBoxFields.smtp_password.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.SmtpAccount ] =MailBoxServerFields.username.Prefix(smtp_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.LoginDelay ] = MailBoxFields.login_delay.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.Id ] = MailBoxFields.id.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.Enabled ] =MailBoxFields.enabled.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.QuotaError ] = MailBoxFields.quota_error.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.AuthError ] = MailBoxFields.auth_error.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.Imap ] = MailBoxFields.imap.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.BeginDate ] = MailBoxFields.begin_date.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.ServiceType ] = MailBoxFields.service_type.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.RefreshToken ] = MailBoxFields.refresh_token.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.ImapFolders ] = MailBoxFields.imap_folders.Prefix(mail_mailbox_alias);
            fields_for_select[ (int)MailBoxFieldSelectPosition.OutcomingEncryptionType ] = MailBoxServerFields.socket_type.Prefix(smtp_alias);
            fields_for_select[(int)MailBoxFieldSelectPosition.IncomingEncryptionType] = MailBoxServerFields.socket_type.Prefix(in_alias);
            fields_for_select[(int)MailBoxFieldSelectPosition.AuthTypeIn] = MailBoxServerFields.authentication.Prefix(in_alias);
            fields_for_select[(int)MailBoxFieldSelectPosition.AuthtTypeSmtp] = MailBoxServerFields.authentication.Prefix(smtp_alias);
            fields_for_select[(int)MailBoxFieldSelectPosition.IdSmtpServer] = MailBoxServerFields.id.Prefix(smtp_alias);
            fields_for_select[(int)MailBoxFieldSelectPosition.IdInServer] = MailBoxServerFields.id.Prefix(in_alias);

            return new SqlQuery(MAIL_MAILBOX +" "+ mail_mailbox_alias)
                .InnerJoin(MAIL_MAILBOX_SERVER + " " + smtp_alias,
                            Exp.EqColumns(MailBoxFields.id_smtp_server.Prefix(mail_mailbox_alias), MailBoxServerFields.id.Prefix(smtp_alias)))
                .InnerJoin(MAIL_MAILBOX_SERVER + " " + in_alias,
                            Exp.EqColumns(MailBoxFields.id_in_server.Prefix(mail_mailbox_alias), MailBoxServerFields.id.Prefix(in_alias)))
                .Select(fields_for_select);
        }

        private MailBox ToMailBox(object[] r)
        {
            if (r.Length != SelectMailBoxFieldsCont)
            {
                Console.WriteLine("Count of returned fields not equal to");
                var results = r;
                foreach (var field in results)
                {
                    Console.WriteLine(field == null ? "null" : field.ToString());
                }
                return null;
            }

            var in_mail_address = new MailAddress((string) r[(int) MailBoxFieldSelectPosition.Address]);
            var in_account = FormatLoginFromDb((string) r[(int) MailBoxFieldSelectPosition.Account], in_mail_address);
            var smtp_account = FormatLoginFromDb((string)r[(int)MailBoxFieldSelectPosition.SmtpAccount], in_mail_address);
            var in_server_old_format = (string) r[(int) MailBoxFieldSelectPosition.InServer] + ":" + r[(int) MailBoxFieldSelectPosition.InPort];
            var smtp_server_old_format = (string)r[(int)MailBoxFieldSelectPosition.SmtpServer] + ":" + r[(int)MailBoxFieldSelectPosition.SmtpPort];
            var in_encryption = ConvertToEncryptionType((string)r[(int) MailBoxFieldSelectPosition.IncomingEncryptionType]);
            var smtp_encryption = ConvertToEncryptionType((string)r[(int)MailBoxFieldSelectPosition.OutcomingEncryptionType]);
            var in_auth = ConvertToSaslMechanism((string)r[(int)MailBoxFieldSelectPosition.AuthTypeIn]);
            var smtp_auth = ConvertToSaslMechanism((string)r[(int)MailBoxFieldSelectPosition.AuthtTypeSmtp]);

            var res = new MailBox(
                Convert.ToInt32(r[(int)MailBoxFieldSelectPosition.IdTenant]),
                (string)r[(int)MailBoxFieldSelectPosition.IdUser],
                (string)r[(int)MailBoxFieldSelectPosition.Name],
                in_mail_address,
                in_account,
                DecryptPassword((string)r[(int)MailBoxFieldSelectPosition.Password]),
                in_server_old_format,
                Convert.ToBoolean(r[(int)MailBoxFieldSelectPosition.Imap]),
                smtp_server_old_format,
                (string)r[(int)MailBoxFieldSelectPosition.SmtpPassword] == null ? null : DecryptPassword((string)r[(int)MailBoxFieldSelectPosition.SmtpPassword]),
                in_auth != SaslMechanism.None,
                Convert.ToInt32(r[(int)MailBoxFieldSelectPosition.Id]),
                (DateTime)r[(int)MailBoxFieldSelectPosition.BeginDate],
                in_encryption,
                smtp_encryption,
                Convert.ToByte(r[(int)MailBoxFieldSelectPosition.ServiceType]),
                (string)r[(int)MailBoxFieldSelectPosition.RefreshToken]
                )
            {
                Size = Convert.ToInt64(r[(int)MailBoxFieldSelectPosition.SizeLast]),
                MessagesCount = Convert.ToInt32(r[(int)MailBoxFieldSelectPosition.MsgCountLast]),
                SmtpAccount = smtp_account,
                ServerLoginDelay = Convert.ToInt32(r[(int)MailBoxFieldSelectPosition.LoginDelay]),
                Enabled = Convert.ToBoolean(r[(int)MailBoxFieldSelectPosition.Enabled]),
                QuotaError = Convert.ToBoolean(r[(int)MailBoxFieldSelectPosition.QuotaError]),
                AuthError = (r[(int)MailBoxFieldSelectPosition.AuthError] != null) && (DateTime.UtcNow.Ticks - Convert.ToInt64(r[(int)MailBoxFieldSelectPosition.AuthError]) > _authErrorPeriod.Ticks),
                ImapFoldersJson = (string)r[(int)MailBoxFieldSelectPosition.ImapFolders],
                AuthenticationTypeIn = in_auth,
                AuthenticationTypeSmtp = smtp_auth,
                SmtpServerId = (int)r[(int)MailBoxFieldSelectPosition.IdSmtpServer],
                InServerId = (int)r[(int)MailBoxFieldSelectPosition.IdInServer]
            };

            return res;
        }

        private MailAttachment ToMailItemAttachment(object[] r)
        {
            if (r.Length != SelectMailItemAttachmentFieldsCont)
            {
                Console.WriteLine("Count of returned fields not equal to");
                var results = r;
                foreach (var field in results)
                {
                    Console.WriteLine(field == null ? "null" : field.ToString());
                }
                return null;
            }

            var attachment = new MailAttachment
                {
                    fileId = Convert.ToInt32(r[(int) MailItemAttachmentSelectPosition.Id]),
                    fileName = Convert.ToString(r[(int) MailItemAttachmentSelectPosition.Name]),
                    storedName = Convert.ToString(r[(int) MailItemAttachmentSelectPosition.StoredName]),
                    contentType = Convert.ToString(r[(int) MailItemAttachmentSelectPosition.Type]),
                    size = Convert.ToInt64(r[(int) MailItemAttachmentSelectPosition.Size]),
                    fileNumber = Convert.ToInt32(r[(int) MailItemAttachmentSelectPosition.FileNumber]),
                    streamId = Convert.ToString(r[(int)MailItemAttachmentSelectPosition.IdStream]),
                    tenant = Convert.ToInt32(r[(int)MailItemAttachmentSelectPosition.Tenant]),
                    user = Convert.ToString(r[(int)MailItemAttachmentSelectPosition.User]),
                    contentId = Convert.ToString(r[(int)MailItemAttachmentSelectPosition.ContentId])
                };
            
            // if StoredName is empty then attachment had been stored by filename (old attachment);
            attachment.storedName = string.IsNullOrEmpty(attachment.storedName)? attachment.fileName: attachment.storedName;

            return attachment;
        }

        private void GetMailBoxState(int id, out bool is_removed, out bool is_deactivated, out DateTime begin_date)
        {
            is_removed = true;
            is_deactivated = true;
            begin_date = MIN_BEGIN_DATE;

            using (var db = GetDb())
            {
                var res = db.ExecuteList(new SqlQuery(MAIL_MAILBOX)
                        .Select(MailBoxFields.is_removed, MailBoxFields.enabled, MailBoxFields.begin_date)
                        .Where(Exp.Eq(MailBoxFields.id, id)))
                        .FirstOrDefault();

                if (res == null) return;
                is_removed = Convert.ToBoolean(res[0]);
                is_deactivated = !Convert.ToBoolean(res[1]);
                begin_date = Convert.ToDateTime(res[2]);
            }
        }

        // Return id_mailbox > 0 if address exists in mail_mailbox table.
        private int MailBoxExists(string address, string id_user, int tenant)
        {
            using (var db = GetDb())
            {
                return db.ExecuteScalar<int>(
                    new SqlQuery(MAIL_MAILBOX)
                        .Select(MailBoxFields.id)
                        .Where(GetUserWhere(id_user, tenant))
                        .Where(MailBoxFields.address, address)
                        .Where(MailBoxFields.is_removed, false));
            }
        }

        private static List<MailServerSettings> GetPopSettingsVariants(string email, string password, MailBox mbox, clientConfig config)
        {
            var temp_list = new List<MailServerSettings>();
            if (config != null && config.emailProvider.incomingServer != null)
            {
                var address = new MailAddress(email);
                foreach (var pop_server in config.emailProvider.incomingServer.Where(x => x.type == "pop3"))
                {
                    if (pop_server.hostname == null) continue;
                    temp_list.Add(new MailServerSettings
                    {
                        Url = FormatServerFromDb(pop_server.hostname, address.Host.ToLowerInvariant()),
                        Port = pop_server.port,
                        AccountName = FormatLoginFromDb(pop_server.username, address),
                        AccountPass = password,
                        AuthenticationType = ConvertToSaslMechanism(pop_server.authentication),
                        EncryptionType = ConvertToEncryptionType(pop_server.socketType)
                    });
                }

                if (temp_list.Any())
                {
                    //if settings was founded in db then we will finish settings tuning.
                    return temp_list;
                }
            }
            temp_list.Add(new MailServerSettings
            {
                Url = mbox.Server,
                Port = 110,
                AccountName = email,
                AccountPass = password,
                AuthenticationType = SaslMechanism.Login,
                EncryptionType = EncryptionType.StartTLS
            });
            temp_list.Add(new MailServerSettings
            {
                Url = mbox.Server,
                Port = 995,
                AccountName = email,
                AccountPass = password,
                AuthenticationType = SaslMechanism.Login,
                EncryptionType = EncryptionType.SSL
            });
            temp_list.Add(new MailServerSettings
            {
                Url = mbox.Server,
                Port = 110,
                AccountName = email,
                AccountPass = password,
                AuthenticationType = SaslMechanism.CramMd5,
                EncryptionType = EncryptionType.None
            });
            temp_list.Add(new MailServerSettings
            {
                Url = mbox.Server,
                Port = 110,
                AccountName = email,
                AccountPass = password,
                AuthenticationType = SaslMechanism.Login,
                EncryptionType = EncryptionType.None
            });
            return temp_list;
        }

        private static List<MailServerSettings> GetImapSettingsVariants(string email, string password, MailBox mbox, clientConfig config)
        {
            var temp_list = new List<MailServerSettings>();
            if (config != null && config.emailProvider.incomingServer != null)
            {
                var address = new MailAddress(email);
                foreach (var imap_server in config.emailProvider.incomingServer.Where(x => x.type == "imap"))
                {
                    if(imap_server.hostname == null) continue;
                    temp_list.Add(new MailServerSettings
                    {
                        Url = FormatServerFromDb(imap_server.hostname, address.Host.ToLowerInvariant()),
                        Port = imap_server.port,
                        AccountName = FormatLoginFromDb(imap_server.username, address),
                        AccountPass = password,
                        AuthenticationType = ConvertToSaslMechanism(imap_server.authentication),
                        EncryptionType = ConvertToEncryptionType(imap_server.socketType)
                    });
                }

                if (temp_list.Any())
                {
                    //if settings was founded in db then we will finish settings tuning.
                    return temp_list;
                }
            }
            temp_list.Add(new MailServerSettings
            {
                Url = mbox.Server,
                Port = 143,
                AccountName = email,
                AccountPass = password,
                AuthenticationType = SaslMechanism.Login,
                EncryptionType = EncryptionType.StartTLS
            });
            temp_list.Add(new MailServerSettings
            {
                Url = mbox.Server,
                Port = 993,
                AccountName = email,
                AccountPass = password,
                AuthenticationType = SaslMechanism.Login,
                EncryptionType = EncryptionType.SSL
            });
            temp_list.Add(new MailServerSettings
            {
                Url = mbox.Server,
                Port = 143,
                AccountName = email,
                AccountPass = password,
                AuthenticationType = SaslMechanism.CramMd5,
                EncryptionType = EncryptionType.None
            });
            temp_list.Add(new MailServerSettings
            {
                Url = mbox.Server,
                Port = 143,
                AccountName = email,
                AccountPass = password,
                AuthenticationType = SaslMechanism.Login,
                EncryptionType = EncryptionType.None
            });
            return temp_list;
        }

        private static List<MailServerSettings> GetSmtpSettingsVariants(string email, string password, MailBox mbox, clientConfig config)
        {

            var temp_list = new List<MailServerSettings>();
            if (config != null && config.emailProvider.outgoingServer != null)
            {
                var address = new MailAddress(email);
                foreach (var mail_server_settingse in config.emailProvider.outgoingServer)
                {

                    temp_list.Add(new MailServerSettings
                        {
                            Url = FormatServerFromDb(mail_server_settingse.hostname, address.Host.ToLowerInvariant()),
                            Port = mail_server_settingse.port,
                            AccountName = FormatLoginFromDb(mail_server_settingse.username, address),
                            AccountPass = password,
                            AuthenticationType = ConvertToSaslMechanism(mail_server_settingse.authentication),
                            EncryptionType = ConvertToEncryptionType(mail_server_settingse.socketType)
                        });
                }

                if (temp_list.Any())
                {
                    //if settings was founded in db then we will finish settings tuning.
                    return temp_list;
                }
            }
            temp_list.Add(new MailServerSettings
            {
                Url = mbox.SmtpServer,
                Port = 587,
                AccountName = email,
                AccountPass = password,
                AuthenticationType = SaslMechanism.Login,
                EncryptionType = EncryptionType.StartTLS
            });
            temp_list.Add(new MailServerSettings
            {
                Url = mbox.SmtpServer,
                Port = 465,
                AccountName = email,
                AccountPass = password,
                AuthenticationType = SaslMechanism.Login,
                EncryptionType = EncryptionType.SSL
            });
            temp_list.Add(new MailServerSettings
            {
                Url = mbox.SmtpServer,
                Port = 25,
                AccountName = email,
                AccountPass = password,
                AuthenticationType = SaslMechanism.CramMd5,
                EncryptionType = EncryptionType.None
            });
            temp_list.Add(new MailServerSettings
            {
                Url = mbox.SmtpServer,
                Port = 587,
                AccountName = email,
                AccountPass = password,
                AuthenticationType = SaslMechanism.CramMd5,
                EncryptionType = EncryptionType.None
            });
            temp_list.Add(new MailServerSettings
            {
                Url = mbox.SmtpServer,
                Port = 25,
                AccountName = email,
                AccountPass = password,
                AuthenticationType = SaslMechanism.Login,
                EncryptionType = EncryptionType.None
            });
            temp_list.Add(new MailServerSettings
            {
                Url = mbox.SmtpServer,
                Port = 587,
                AccountName = email,
                AccountPass = password,
                AuthenticationType = SaslMechanism.Login,
                EncryptionType = EncryptionType.None
            });
            return temp_list;
        }

        private static string FormatLoginFromDb(string format, MailAddress address)
        {
            return format.Replace("%EMAILADDRESS%", address.Address)
                         .Replace("%EMAILLOCALPART%", address.User)
                         .Replace("%EMAILDOMAIN%", address.Host.ToLowerInvariant())
                         .Replace("%EMAILHOSTNAME%", Path.GetFileNameWithoutExtension(address.Host.ToLowerInvariant()));
        }

        // Documentation in unit tests
        public static string GetLoginFormatFrom(MailAddress address, string username)
        {
            var address_lower = address.Address.ToLower();
            var username_lower = username.ToLower();
            var mailparts = address_lower.Split('@');

            var localpart = mailparts[0];
            var domain = mailparts[1];
            var host_name_variant_1 = Path.GetFileNameWithoutExtension(domain);
            var host_name_variant_2 = domain.Split('.')[0];

            var result_format = username_lower.Replace(address_lower, "%EMAILADDRESS%");
            int pos = result_format.IndexOf(localpart, StringComparison.InvariantCulture);
            if (pos >= 0)
            {
                result_format = result_format.Substring(0, pos) + "%EMAILLOCALPART%" + result_format.Substring(pos + localpart.Length);
            }
            result_format = result_format.Replace(domain, "%EMAILDOMAIN%");
            if (host_name_variant_1 != null)
                result_format = result_format.Replace(host_name_variant_1, "%EMAILHOSTNAME%");
            result_format = result_format.Replace(host_name_variant_2, "%EMAILHOSTNAME%");

            return result_format == username_lower ? "" : result_format;
        }

        private static string FormatServerFromDb(string format, string host)
        {
            return format.Replace("%EMAILDOMAIN%", host);
        }

        private const int HardcodedLoginTimeForMsMail = 900;
        private static int GetLoginDelayTime(MailBox mail_box)
        {
            //Todo: This hardcode inserted because pop3.live.com doesn't support CAPA command.
            //Right solution for that collision type:
            //1) Create table in DB: mail_login_delays. With REgexs and delays
            //1.1) Example of mail_login_delays data:
            //    .*@outlook.com    900
            //    .*@hotmail.com    900
            //    .*                30
            //1.2) Load this table to aggregator cache. Update it on changing.
            //1.3) Match email addreess of account with regexs from mail_login_delays
            //1.4) If email matched then set delay from that record.
            if (mail_box.Server == "pop3.live.com")
                return HardcodedLoginTimeForMsMail;

            return mail_box.ServerLoginDelay < MailBox.DefaultServerLoginDelay
                       ? MailBox.DefaultServerLoginDelay
                       : mail_box.ServerLoginDelay;
        }

        #endregion
    }
}