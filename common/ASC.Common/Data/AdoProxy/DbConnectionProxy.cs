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
using System.Data;

namespace ASC.Common.Data.AdoProxy
{
    class DbConnectionProxy : IDbConnection
    {
        private readonly IDbConnection connection;
        private readonly ProxyContext context;
        private bool disposed;

        
        public DbConnectionProxy(IDbConnection connection, ProxyContext ctx)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (ctx == null) throw new ArgumentNullException("ctx");

            this.connection = connection;
            context = ctx;
        }


        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            using (ExecuteHelper.Begin(dur => context.FireExecuteEvent(this, String.Format("BeginTransaction({0})", il), dur)))
            {
                return new DbTransactionProxy(connection.BeginTransaction(il), context);
            }
        }

        public IDbTransaction BeginTransaction()
        {
            using (ExecuteHelper.Begin(dur => context.FireExecuteEvent(this, "BeginTransaction", dur)))
            {
                return new DbTransactionProxy(connection.BeginTransaction(), context);
            }
        }

        public void ChangeDatabase(string databaseName)
        {
            connection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            connection.Close();
        }

        public string ConnectionString
        {
            get { return connection.ConnectionString; }
            set { connection.ConnectionString = value; }
        }

        public int ConnectionTimeout
        {
            get { return connection.ConnectionTimeout; }
        }

        public IDbCommand CreateCommand()
        {
            return new DbCommandProxy(connection.CreateCommand(), context);
        }

        public string Database
        {
            get { return connection.Database; }
        }

        public void Open()
        {
            using (ExecuteHelper.Begin(dur => context.FireExecuteEvent(this, "Open", dur)))
            {
                connection.Open();
            }
        }

        public ConnectionState State
        {
            get { return connection.State; }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    using (ExecuteHelper.Begin(dur => context.FireExecuteEvent(this, "Dispose", dur)))
                    {
                        connection.Dispose();
                    }
                }
                disposed = true;
            }
        }

        ~DbConnectionProxy()
        {
            Dispose(false);
        }
    }
}