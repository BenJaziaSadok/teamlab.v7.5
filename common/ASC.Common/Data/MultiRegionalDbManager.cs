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
using System.Data;
using System.Linq;
using System.Web;
using ASC.Common.Data.Sql;

namespace ASC.Common.Data
{
    public class MultiRegionalDbManager : IDbManager
    {
        private readonly List<DbManager> databases;

        private readonly DbManager localDb;

        private volatile bool disposed;


        public string DatabaseId { get; private set; }

        public IDbConnection Connection
        {
            get { return localDb.Connection; }
        }


        public MultiRegionalDbManager(string dbid)
        {
            var cmp = StringComparison.InvariantCultureIgnoreCase;
            DatabaseId = dbid;
            databases = ConfigurationManager.ConnectionStrings.OfType<ConnectionStringSettings>()
                                            .Where(c => c.Name.Equals(dbid, cmp) || c.Name.StartsWith(dbid + ".", cmp))
                                            .Select(
                                                c =>
                                                HttpContext.Current != null
                                                    ? DbManager.FromHttpContext(c.Name)
                                                    : new DbManager(c.Name))
                                            .ToList();
            localDb = databases.SingleOrDefault(db => db.DatabaseId.Equals(dbid, cmp));
        }

        public MultiRegionalDbManager(IEnumerable<DbManager> databases)
        {
            this.databases = databases.ToList();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (disposed) return;
                disposed = true;
                databases.ForEach(db => db.Dispose());
            }
        }


        public static MultiRegionalDbManager FromHttpContext(string databaseId)
        {
            return new MultiRegionalDbManager(databaseId);
        }


        public List<object[]> ExecuteList(string sql, params object[] parameters)
        {
            return databases.SelectMany(db => db.ExecuteList(sql, parameters)).ToList();
        }

        public List<object[]> ExecuteList(ISqlInstruction sql)
        {
            return databases.SelectMany(db => db.ExecuteList(sql)).ToList();
        }

        public List<object[]> ExecuteListWithRegion(ISqlInstruction sql)
        {
            return databases.SelectMany(db => db.ExecuteList(sql)
                                                .Select(oldArray =>
                                                    {
                                                        var newArray = new object[oldArray.Count() + 1];
                                                        oldArray.CopyTo(newArray, 0);
                                                        newArray[oldArray.Count()] =
                                                            db.DatabaseId.IndexOf('.') > -1
                                                                ? db.DatabaseId.Substring(db.DatabaseId.IndexOf('.') + 1)
                                                                : "";

                                                        return newArray;
                                                    })).ToList();
        }

        public List<T> ExecuteList<T>(ISqlInstruction sql, Converter<IDataRecord, T> converter)
        {
            return databases.SelectMany(db => db.ExecuteList<T>(sql, converter)).ToList();
        }

        public T ExecuteScalar<T>(string sql, params object[] parameters)
        {
            return localDb.ExecuteScalar<T>(sql, parameters);
        }

        public T ExecuteScalar<T>(ISqlInstruction sql)
        {
            return localDb.ExecuteScalar<T>(sql);
        }

        public int ExecuteNonQuery(string sql, params object[] parameters)
        {
            return localDb.ExecuteNonQuery(sql, parameters);
        }

        public int ExecuteNonQuery(ISqlInstruction sql)
        {
            return localDb.ExecuteNonQuery(sql);
        }

        public int ExecuteNonQuery(string region, ISqlInstruction sql)
        {
            var db = string.IsNullOrEmpty(region) ? localDb : databases.FirstOrDefault(x => x.DatabaseId.EndsWith(region));
            if (db != null) return db.ExecuteNonQuery(sql);
            return -1;
        }

        public int ExecuteBatch(IEnumerable<ISqlInstruction> batch)
        {
            return localDb.ExecuteBatch(batch);
        }

        public IDbTransaction BeginTransaction()
        {
            return localDb.BeginTransaction();
        }
    }
}