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
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Core.Tenants;

namespace ASC.FullTextIndex.Service
{
    class TenantsProvider
    {
        private readonly string dbid;
        private readonly int userActivityDays;
        private DateTime last;


        public TenantsProvider(string dbid, int userActivityDays)
        {
            this.dbid = dbid;
            this.userActivityDays = userActivityDays;
        }

        public List<Tenant> GetTenants()
        {
            var result = new List<Tenant>();
            var tenants = CoreContext.TenantManager.GetTenants();

            if (last == DateTime.MinValue)
            {
                // first start
                if (userActivityDays == 0)
                {
                    // not use user_activity
                    result.AddRange(tenants);
                }
                else
                {
                    // use user_activity
                    using (var db = new DbManager(dbid))
                    {
                        var q = new SqlQuery("webstudio_uservisit")
                            .Select("tenantid")
                            .Where(Exp.Ge("visitdate", DateTime.UtcNow.Date.AddDays(-userActivityDays)))
                            .GroupBy(1);
                        var ids = db
                            .ExecuteList(q)
                            .ConvertAll(r => Convert.ToInt32(r[0]));
                        result.AddRange(tenants.FindAll(t => ids.Contains(t.TenantId)));
                    }
                }
            }
            else
            {
                using (var db = new DbManager(dbid))
                {
                    var q = new SqlQuery("webstudio_uservisit")
                        .Select("tenantid")
                        .Where(Exp.Eq("visitdate", DateTime.UtcNow.Date))
                        .Where(Exp.Ge("lastvisittime", last.AddHours(-1)))
                        .GroupBy(1);
                    var ids = db
                        .ExecuteList(q)
                        .ConvertAll(r => Convert.ToInt32(r[0]));
                    result.AddRange(tenants.FindAll(t => ids.Contains(t.TenantId)));
                }
            }
            last = DateTime.UtcNow;

            result.RemoveAll(t => t.Status != TenantStatus.Active);
            return result;
        }
    }
}
