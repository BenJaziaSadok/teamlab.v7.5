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
using System.Data;
using ASC.Core.Tenants;
using ASC.Data.Backup.Tasks.Data;

namespace ASC.Data.Backup.Tasks.Modules
{
    internal class TenantsModuleSpecifics : ModuleSpecificsBase
    {
        private readonly TableInfo[] _tables = new[]
            {
                new TableInfo("tenants_quota") {TenantColumn = "tenant"},
                new TableInfo("tenants_tariff") {AutoIncrementColumn = "id", TenantColumn = "tenant"},
                new TableInfo("tenants_tenants") {AutoIncrementColumn = "id", TenantColumn = "id"},
                new TableInfo("tenants_quotarow") {TenantColumn = "tenant", InsertMethod = InsertMethod.Replace},
                new TableInfo("core_user") {TenantColumn = "tenant", GuidIDColumn = "id"},
                new TableInfo("core_group") {TenantColumn = "tenant", GuidIDColumn = "id"},
            };

        private readonly RelationInfo[] _tableRelations = new[]
            {
                new RelationInfo("tenants_tenants", "id", "tenants_quota", "tenant"),
                new RelationInfo("tenants_tenants", "id", "tenants_tariff", "tenant"),
                new RelationInfo("tenants_tenants", "id", "tenants_tariff", "tariff", x => Convert.ToInt32(x["tariff"]) > 0),
                new RelationInfo("core_user", "id", "tenants_tenants", "owner_id", null, null, RelationImportance.Low) 
            };

        public override string ConnectionStringName
        {
            get { return "core"; }
        }

        public override ModuleName ModuleName
        {
            get { return ModuleName.Tenants; }
        }

        public override IEnumerable<TableInfo> Tables
        {
            get { return _tables; }
        }

        public override IEnumerable<RelationInfo> TableRelations
        {
            get { return _tableRelations; }
        }

        protected override bool TryPrepareRow(IDbConnection connection, ColumnMapper columnMapper, TableInfo table, DataRowInfo row, out Dictionary<string, object> preparedRow)
        {
            if (table.Name == "tenants_tenants" && string.IsNullOrEmpty(Convert.ToString(row["payment_id"])))
            {
                var oldTenantID = Convert.ToInt32(row["id"]);
                row["payment_id"] = Core.CoreContext.Configuration.GetKey(oldTenantID);
            }
            return base.TryPrepareRow(connection, columnMapper, table, row, out preparedRow);
        }

        protected override bool TryPrepareValue(IDbConnection connection, ColumnMapper columnMapper, TableInfo table, string columnName, ref object value)
        {
            //we insert tenant as suspended so it can't be accessed before restore operation is finished
            if (table.Name.Equals("tenants_tenants", StringComparison.InvariantCultureIgnoreCase) && 
                columnName.Equals("status", StringComparison.InvariantCultureIgnoreCase))
            {
                value = (int)TenantStatus.Restoring;
                return true;
            }

            if (table.Name.Equals("tenants_quotarow", StringComparison.InvariantCultureIgnoreCase) &&
                columnName.Equals("last_modified", StringComparison.InvariantCultureIgnoreCase))
            {
                value = DateTime.UtcNow;
                return true;
            }

            if ((table.Name == "core_user" || table.Name == "core_group") && columnName == "last_modified")
            {
                value = DateTime.UtcNow.AddMinutes(2);
                return true;
            }

            return base.TryPrepareValue(connection, columnMapper, table, columnName, ref value);
        }
    }
}
