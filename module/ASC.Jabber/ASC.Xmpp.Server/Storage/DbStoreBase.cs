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
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Xmpp.Common.Configuration;
using ASC.Xmpp.Server.Utils;

namespace ASC.Xmpp.Server.Storage
{
    public abstract class DbStoreBase : IConfigurable, IDisposable
    {
        protected static readonly int MESSAGE_COLUMN_LEN = (int)Math.Pow(2, 24) - 1;

        private readonly object syncRoot = new object();
        private DbManager db;


        public virtual void Configure(IDictionary<string, string> properties)
        {
            var dbid = string.Empty;

            if (properties.ContainsKey("connectionString"))
            {
                dbid = GetType().FullName;
                var connectionString = properties["connectionString"];
                if (!DbRegistry.IsDatabaseRegistered(dbid))
                {
                    DbRegistry.RegisterDatabase(dbid, "System.Data.SQLite", connectionString);
                }
                CreateDbFolderIfNotExists(connectionString);
            }
            else if (properties.ContainsKey("connectionStringName"))
            {
                dbid = properties["connectionStringName"];
                var connectionString = ConfigurationManager.ConnectionStrings[dbid];
                if (connectionString == null)
                {
                    throw new ConfigurationErrorsException("Can not find connection string with name " + dbid);
                }
                if (!DbRegistry.IsDatabaseRegistered(dbid))
                {
                    DbRegistry.RegisterDatabase(dbid, connectionString);
                }
                CreateDbFolderIfNotExists(connectionString.ConnectionString);
            }
            else
            {
                throw new ConfigurationErrorsException("Can not create database connection: no connectionString or connectionStringName properties.");
            }

            db = new DbManager(dbid, false);

            if (!properties.ContainsKey("generateSchema") || Convert.ToBoolean(properties["generateSchema"]))
            {
                var creates = GetCreateSchemaScript();
                if (creates != null && 0 < creates.Length)
                {
                    foreach (var c in creates)
                    {
                        db.ExecuteNonQuery(c);
                    }
                }
            }
        }

        public void Dispose()
        {
            lock (syncRoot)
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }


        protected virtual SqlCreate[] GetCreateSchemaScript()
        {
            return new SqlCreate[0];
        }


        protected List<object[]> ExecuteList(ISqlInstruction sql)
        {
            lock (syncRoot)
            {
                try
                {
                    return db.ExecuteList(sql);
                }
                catch (DbException)
                {
                    db.Dispose();
                    db = new DbManager(db.DatabaseId, false);
                    return db.ExecuteList(sql);
                }
            }
        }

        protected T ExecuteScalar<T>(ISqlInstruction sql)
        {
            lock (syncRoot)
            {
                try
                {
                    return db.ExecuteScalar<T>(sql);
                }
                catch (DbException)
                {
                    db.Dispose();
                    db = new DbManager(db.DatabaseId, false);
                    return db.ExecuteScalar<T>(sql);
                }
            }
        }

        protected int ExecuteNonQuery(ISqlInstruction sql)
        {
            lock (syncRoot)
            {
                try
                {
                    return db.ExecuteNonQuery(sql);
                }
                catch (DbException)
                {
                    db.Dispose();
                    db = new DbManager(db.DatabaseId, false);
                    return db.ExecuteNonQuery(sql);
                }
            }
        }

        protected int ExecuteBatch(IEnumerable<ISqlInstruction> batch)
        {
            lock (syncRoot)
            {
                try
                {
                    return db.ExecuteBatch(batch);
                }
                catch (DbException)
                {
                    db.Dispose();
                    db = new DbManager(db.DatabaseId, false);
                    return db.ExecuteBatch(batch);
                }
            }
        }


        private void CreateDbFolderIfNotExists(string connectionString)
        {
            if (connectionString.ToLower().Contains("data source="))
            {
                var dbDir = Path.GetDirectoryName(PathUtils.GetAbsolutePath(GetDbSQLitePath(connectionString)));
                if (!Directory.Exists(dbDir)) Directory.CreateDirectory(dbDir);
            }
        }

        private string GetDbSQLitePath(string connectionString)
        {
            return new SQLiteConnectionStringBuilder(connectionString).DataSource;
        }
    }
}