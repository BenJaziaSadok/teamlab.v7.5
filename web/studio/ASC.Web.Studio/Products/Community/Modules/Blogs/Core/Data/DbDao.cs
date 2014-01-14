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

#region Usings

using System;
using System.Data;
using ASC.Common.Data;
using ASC.Common.Data.Sql;

#endregion

namespace ASC.Blogs.Core.Data
{
    public class DbDao
    {
        private readonly DbManager db;


        protected DbDao(DbManager db, int tenant)
        {
            if (db == null) throw new ArgumentNullException("db");

            this.db = db;
            Tenant = tenant;
        }

        public int Tenant { get; private set; }

        public DbManager Db { get { return db; } }

        public IDbConnection OpenConnection()
        {
            return db.Connection;
        }

        protected SqlQuery Query(string table)
        {
            return new SqlQuery(table).Where(GetTenantColumnName(table), Tenant);
        }

        protected SqlUpdate Update(string table)
        {
            return new SqlUpdate(table).Where(GetTenantColumnName(table), Tenant);
        }

        protected SqlDelete Delete(string table)
        {
            return new SqlDelete(table).Where(GetTenantColumnName(table), Tenant);
        }

        protected SqlInsert Insert(string table)
        {
            return new SqlInsert(table, true).InColumns(TenantColumnName).Values(Tenant);
        }

        protected string TenantColumnName { get { return "Tenant"; } }
        
        protected string GetTenantColumnName(string table)
        {
            return String.Format("{0}.{1}", table, TenantColumnName);
        }
    }
}
