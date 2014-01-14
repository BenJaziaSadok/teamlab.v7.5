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
using ASC.Common.Data;
using ASC.Common.Data.Sql;

namespace ASC.Core.Data
{
    public class DbExecuter : IDbExecuter
    {
        private readonly string dbid;

        private readonly DbManager dbManager;

        private readonly bool innerCall;

        public DbExecuter(ConnectionStringSettings connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException("connectionString");

            dbid = connectionString.Name;
            if (!DbRegistry.IsDatabaseRegistered(dbid))
            {
                DbRegistry.RegisterDatabase(dbid, connectionString);
            }
        }

        private DbExecuter(DbManager db)
        {
            dbManager = db;
            innerCall = true;
        }

        public T ExecScalar<T>(ISqlInstruction sql)
        {
            if (innerCall)
            {
                return dbManager.ExecuteScalar<T>(sql);
            }
            else
            {
                using (var db = new DbManager(dbid))
                {
                    return db.ExecuteScalar<T>(sql);
                }
            }
        }

        public int ExecNonQuery(ISqlInstruction sql)
        {
            if (innerCall)
            {
                return dbManager.ExecuteNonQuery(sql);
            }
            else
            {
                using (var db = new DbManager(dbid))
                {
                    return db.ExecuteNonQuery(sql);
                }
            }
        }

        public List<object[]> ExecList(ISqlInstruction sql)
        {
            if (innerCall)
            {
                return dbManager.ExecuteList(sql);
            }
            else
            {
                using (var db = new DbManager(dbid))
                {
                    return db.ExecuteList(sql);
                }
            }
        }

        public void ExecBatch(IEnumerable<ISqlInstruction> batch)
        {
            if (innerCall)
            {
                dbManager.ExecuteBatch(batch);
            }
            else
            {
                using (var db = new DbManager(dbid))
                {
                    db.ExecuteBatch(batch);
                }
            }
        }

        public void ExecAction(Action<IDbExecuter> action)
        {
            if (innerCall)
            {
                action(this);
            }
            else
            {
                using (var db = new DbManager(dbid))
                using (var tx = db.BeginTransaction())
                {
                    action(new DbExecuter(db));
                    tx.Commit();
                }
            }
        }
    }
}
