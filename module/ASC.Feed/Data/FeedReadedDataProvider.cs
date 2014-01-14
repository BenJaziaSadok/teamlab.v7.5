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
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;

namespace ASC.Feed.Data
{
    public class FeedReadedDataProvider
    {
        private const string dbId = Constants.FeedDbId;


        public DateTime GetTimeReaded()
        {
            return GetTimeReaded(GetUser(), "all", GetTenant());
        }

        public DateTime GetTimeReaded(string module)
        {
            return GetTimeReaded(GetUser(), module, GetTenant());
        }

        public DateTime GetTimeReaded(Guid user, string module, int tenant)
        {
            var query = new SqlQuery("feed_readed")
                .SelectMax("timestamp")
                .Where("tenant_id", tenant)
                .Where("user_id", user.ToString())
                .Where("module", module);

            using (var db = GetDb())
            {
                return db.ExecuteScalar<DateTime>(query);
            }
        }

        public void SetTimeReaded()
        {
            SetTimeReaded(GetUser(), DateTime.UtcNow, "all", GetTenant());
        }

        public void SetTimeReaded(string module)
        {
            SetTimeReaded(GetUser(), DateTime.UtcNow, module, GetTenant());
        }

        public void SetTimeReaded(Guid user)
        {
            SetTimeReaded(user, DateTime.UtcNow, "all", GetTenant());
        }

        public void SetTimeReaded(Guid user, DateTime time, string module, int tenant)
        {
            if (string.IsNullOrEmpty(module)) return;

            var query = new SqlInsert("feed_readed", true)
                .InColumns("user_id", "timestamp", "module", "tenant_id")
                .Values(user.ToString(), time, module, tenant);

            using (var db = GetDb())
            {
                db.ExecuteNonQuery(query);
            }
        }

        public IEnumerable<string> GetReadedModules(DateTime fromTime)
        {
            return GetReadedModules(GetUser(), GetTenant(), fromTime);
        }

        public IEnumerable<string> GetReadedModules(Guid user, int tenant, DateTime fromTime)
        {
            var query = new SqlQuery("feed_readed")
                .Select("module")
                .Where("tenant_id", tenant)
                .Where("user_id", user)
                .Where(Exp.Gt("timestamp", fromTime));

            using (var db = GetDb())
            {
                return db.ExecuteList(query).ConvertAll(r => (string)r[0]);
            }
        }


        private static DbManager GetDb()
        {
            return new DbManager(dbId);
        }

        private static int GetTenant()
        {
            return CoreContext.TenantManager.GetCurrentTenant().TenantId;
        }

        private static Guid GetUser()
        {
            return SecurityContext.CurrentAccount.ID;
        }
    }
}