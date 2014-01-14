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
using System.Net.Mail;
using System.Web;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Mail.Aggregator.DbSchema;
using NLog;
using System.Configuration;

namespace ASC.Mail.Aggregator
{
    public partial class MailBoxManager : IDisposable
    {
        #region -- Global Values --

        public const string ConnectionStringName = "mail";

        public const string MAIL_DAEMON_EMAIL = "mail-daemon@teamlab.com";

        private readonly static Logger _log = LogManager.GetLogger("MailBoxManager");

        private static TimeSpan _authErrorPeriod = TimeSpan.FromHours(1);

        private const string MODULE_NAME = "mailaggregator";

        private const string DATA_TAG = "666ceac1-4532-4f8c-9cba-8f510eca2fd1";

        private const string GoogleHost = "gmail.com";

        #endregion

        #region -- Mail Ticks --
        private class MailTicks
        {
            public MailTicks(long inactive_mail_ratio)
            {
                var mail_ratio = inactive_mail_ratio;
                if (mail_ratio > 50)
                {
                    k = -1;
                    mail_ratio = 100 - mail_ratio;
                }
                var window_double = 100.0 / mail_ratio;
                window = (long)Math.Round(window_double);
                shift = (long)Math.Round((window_double - window) * mail_ratio);
            }

            // return true for inactive users ticks
            public bool Tick()
            {
                var res = (0==(count + shift) % window);
                if (k < 0)
                    res = !res;

                if (++count == 100)
                    count = 0;
                return res;
            }

            private long count = 0;
            private long window;
            private long shift;
            private long k = 1;
        }

        private MailTicks ticks;
        #endregion

        #region -- Constructor --

        // inactive_mail_ratio(from 0 to 100) - Percentage of processed inactive boxes\
        //Todo: remove unused parameters
        public MailBoxManager(ConnectionStringSettings connectionString, long inactive_mail_ratio, TimeSpan _auth_error_period)
        {
            // active/inactive balance
            ticks = new MailTicks(inactive_mail_ratio);
            _authErrorPeriod = _auth_error_period;
        }

        public MailBoxManager(ConnectionStringSettings connectionString, long inactive_mail_ratio)
            : this(connectionString, inactive_mail_ratio, TimeSpan.FromHours(1))
        {
        }

        #endregion

        #region -- Methods --

        #region -- Public Methods --

        public DateTime GetMailboxFolderModifyDate(int mailbox_id, int folder_id)
        {
            using (var db = GetDb())
            {
                var date_string = db.ExecuteScalar<string>(
                    new SqlQuery(MailTable.name)
                    .SelectMax(MailTable.Columns.time_modified)
                    .Where(MailTable.Columns.id_mailbox, mailbox_id)
                    .Where(Exp.Eq(MailTable.Columns.folder, folder_id)));

                var date_time = new DateTime();

                return DateTime.TryParse(date_string, out date_time) ? date_time.ToUniversalTime() : new DateTime(1974, 1, 1).ToUniversalTime();
            }
        }

        #endregion

        #region -- Private Methods --

        private DbManager GetDb()
        {
            return new DbManager(ConnectionStringName);
        }

        private string EncryptPassword(string password)
        {
            return ASC.Security.Cryptography.InstanceCrypto.Encrypt(password);
        }

        private string DecryptPassword(string password)
        {
            return ASC.Security.Cryptography.InstanceCrypto.Decrypt(password);
        }

        private static MailMessageItem ConvertToMailMessageItem(object[] r, int tenant)
        {
            var now = TenantUtil.DateTimeFromUtc(CoreContext.TenantManager.GetTenant(tenant), DateTime.UtcNow);
            var date = TenantUtil.DateTimeFromUtc(CoreContext.TenantManager.GetTenant(tenant), (DateTime)r[7]);
            var isToday = (now.Year == date.Year && now.Date == date.Date);
            var isYesterday = (now.Year == date.Year && now.Date == date.Date.AddDays(1));

            return new MailMessageItem()
            {
                Id              = Convert.ToInt64(r[0]),
                From            = (string)r[1],
                To              = (string)r[2],
                ReplyTo         = (string)r[3],
                Subject         = (string)r[4],
                Important       = Convert.ToBoolean(r[5]),
                Date            = date,
                Size            = Convert.ToInt32(r[8]),
                HasAttachments  = Convert.ToBoolean(r[9]),
                IsNew           = Convert.ToBoolean(r[10]),
                IsAnswered      = Convert.ToBoolean(r[11]),
                IsForwarded     = Convert.ToBoolean(r[12]),
                IsFromCRM       = Convert.ToBoolean(r[13]),
                IsFromTL        = Convert.ToBoolean(r[14]),
                LabelsString    = (string)r[15],
                RestoreFolderId = r[16] != null ? Convert.ToInt32(r[16]) : -1,
                ChainId         = (string)(r[17] ?? ""),
                ChainLength     = r[17] == null ? 1 : Convert.ToInt32(r[18]),
                Folder          = Convert.ToInt32(r[19]),
                IsToday         = isToday,
                IsYesterday     = isYesterday
            };
        }

        private string ConvertToString(object obj)
        {
            if (obj == DBNull.Value || obj == null)
            {
                return null;
            }

            return Convert.ToString(obj);
        }

        private Exp GetUserWhere(string id_user, int id_tenant, string alias)
        {
            return Exp.Eq(alias + ".tenant", id_tenant) & Exp.Eq(alias + ".id_user", id_user.ToLowerInvariant());
        }

        private Exp GetUserWhere(string id_user, int id_tenant)
        {
            return Exp.Eq("tenant", id_tenant) & Exp.Eq("id_user", id_user.ToLowerInvariant());
        }

        private string GetAddress(MailAddress email)
        {
            return email.Address.ToLowerInvariant();
        }

        public static IDataStore GetDataStore(int id_tenant)
        {
            return StorageFactory.GetStorage(null, id_tenant.ToString(CultureInfo.InvariantCulture), MODULE_NAME, HttpContext.Current);
        }

        #endregion

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }

    public class ContactEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string contact1, string contact2)
        {
            var contact1_parts = contact1.Split('<');
            var contact2_parts = contact2.Split('<');

            return contact1_parts.Last().Replace(">", "") == contact2_parts.Last().Replace(">", "");
        }

        public int GetHashCode(string str)
        {
            var str_parts = str.Split('<');
            return str_parts.Last().Replace(">", "").GetHashCode();
        }
    }
}
