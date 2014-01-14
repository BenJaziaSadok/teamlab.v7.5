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

namespace ASC.SocialMedia
{
    class BaseDao : IDisposable
    {
        protected DbManager DbManager
        {
            get;
            private set;
        }

        protected int TenantID
        {
            get;
            private set;
        }

        protected BaseDao(int tenantID, String storageKey)
        {
            TenantID = tenantID;
            DbManager = new DbManager(storageKey);
        }

        public void Dispose()
        {
            DbManager.Dispose();
        }

        protected List<object[]> ExecList(ISqlInstruction sql)
        {
            return DbManager.ExecuteList(sql);
        }

        protected List<object[]> ExecList(string sql)
        {
            return DbManager.ExecuteList(sql);
        }

        protected T ExecScalar<T>(ISqlInstruction sql)
        {
            return DbManager.ExecuteScalar<T>(sql);
        }

        protected int ExecNonQuery(ISqlInstruction sql)
        {
            return DbManager.ExecuteNonQuery(sql);
        }

        protected SqlQuery Query(string table)
        {
            return new SqlQuery(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected SqlDelete Delete(string table)
        {
            return new SqlDelete(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected SqlInsert Insert(string table)
        {
            return new SqlInsert(table, true).InColumns(GetTenantColumnName(table)).Values(TenantID);
        }

        protected SqlUpdate Update(string table)
        {
            return new SqlUpdate(table).Where(GetTenantColumnName(table), TenantID);
        }

        protected string GetTenantColumnName(string table)
        {
            var tenant = "tenant_id";
            if (!table.Contains(" ")) return tenant;
            return table.Substring(table.IndexOf(" ")).Trim() + "." + tenant;
        }

        protected static Guid ToGuid(object guid)
        {
            var str = guid as string;
            return !string.IsNullOrEmpty(str) ? new Guid(str) : Guid.Empty;
        }
    }
}
