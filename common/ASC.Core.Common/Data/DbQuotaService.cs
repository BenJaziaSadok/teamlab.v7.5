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
using System.Configuration;
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Tenants;

namespace ASC.Core.Data
{
    public class DbQuotaService : DbBaseService, IQuotaService
    {
        private const string tenants_quota = "tenants_quota";
        internal const string tenants_quotarow = "tenants_quotarow";


        public DbQuotaService(ConnectionStringSettings connectionString)
            : base(connectionString, "tenant")
        {
        }


        public IEnumerable<TenantQuota> GetTenantQuotas()
        {
            return GetTenantQuotas(Exp.Empty);
        }

        public TenantQuota GetTenantQuota(int id)
        {
            return GetTenantQuotas(Exp.Eq("tenant", id))
                .SingleOrDefault();
        }

        private IEnumerable<TenantQuota> GetTenantQuotas(Exp where)
        {
            var q = new SqlQuery(tenants_quota)
                .Select("tenant", "name", "max_file_size", "max_total_size", "active_users", "features", "price", "price2", "avangate_id", "visible")
                .Where(where);

            return ExecList(q)
                .ConvertAll(r => new TenantQuota(Convert.ToInt32(r[0]))
                {
                    Name = (string)r[1],
                    MaxFileSize = GetInBytes(Convert.ToInt64(r[2])),
                    MaxTotalSize = GetInBytes(Convert.ToInt64(r[3])),
                    ActiveUsers = Convert.ToInt32(r[4]) != 0 ? Convert.ToInt32(r[4]) : int.MaxValue,
                    Features = (string)r[5],
                    Price = Convert.ToDecimal(r[6]),
                    Price2 = Convert.ToDecimal(r[7]),
                    AvangateId = (string)r[8],
                    Visible = Convert.ToBoolean(r[9]),
                });
        }

        
        public TenantQuota SaveTenantQuota(TenantQuota quota)
        {
            if (quota == null) throw new ArgumentNullException("quota");

            var i = Insert(tenants_quota, quota.Id)
                .InColumnValue("name", quota.Name)
                .InColumnValue("max_file_size", quota.MaxFileSize / 1024 / 1024) // save in MB
                .InColumnValue("max_total_size", quota.MaxTotalSize / 1024 / 1024) // save in MB
                .InColumnValue("active_users", quota.ActiveUsers)
                .InColumnValue("features", quota.Features)
                .InColumnValue("price", quota.Price)
                .InColumnValue("price2", quota.Price2)
                .InColumnValue("avangate_id", quota.AvangateId)
                .InColumnValue("visible", quota.Visible);

            ExecNonQuery(i);
            return quota;
        }

        public void RemoveTenantQuota(int id)
        {
            var d = Delete(tenants_quota, id);
            ExecNonQuery(d);
        }


        public void SetTenantQuotaRow(TenantQuotaRow row, bool exchange)
        {
            if (row == null) throw new ArgumentNullException("row");

            ExecAction(db =>
            {
                var counter = db.ExecScalar<long>(Query(tenants_quotarow, row.Tenant)
                    .Select("counter")
                    .Where("path", row.Path));

                db.ExecNonQuery(Insert(tenants_quotarow, row.Tenant)
                    .InColumnValue("path", row.Path)
                    .InColumnValue("counter", exchange ? counter + row.Counter : row.Counter)
                    .InColumnValue("tag", row.Tag)
                    .InColumnValue("last_modified", DateTime.UtcNow));
            });
        }

        public IEnumerable<TenantQuotaRow> FindTenantQuotaRows(TenantQuotaRowQuery query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var q = new SqlQuery(tenants_quotarow).Select("tenant", "path", "counter", "tag");
            if (query.Tenant != Tenant.DEFAULT_TENANT)
            {
                q.Where("tenant", query.Tenant);
            }
            if (!string.IsNullOrEmpty(query.Path))
            {
                q.Where("path", query.Path);
            }
            if (query.LastModified != default(DateTime))
            {
                q.Where(Exp.Ge("last_modified", query.LastModified));
            }

            return ExecList(q)
                .ConvertAll(r => new TenantQuotaRow
                {
                    Tenant = Convert.ToInt32(r[0]),
                    Path = (string)r[1],
                    Counter = Convert.ToInt64(r[2]),
                    Tag = (string)r[3],
                });
        }


        private long GetInBytes(long bytes)
        {
            const long MB = 1024 * 1024;
            return bytes < MB ? bytes * MB : bytes;
        }
    }
}
