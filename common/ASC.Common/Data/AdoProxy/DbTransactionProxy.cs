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
    class DbTransactionProxy : IDbTransaction
    {
        private bool disposed;
        private readonly ProxyContext context;
        public readonly IDbTransaction transaction;

        public DbTransactionProxy(IDbTransaction transaction, ProxyContext ctx)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (ctx == null) throw new ArgumentNullException("ctx");

            this.transaction = transaction;
            context = ctx;
        }


        public void Commit()
        {
            using (ExecuteHelper.Begin(dur => context.FireExecuteEvent(this, "Commit", dur)))
            {
                transaction.Commit();
            }
        }

        public IDbConnection Connection
        {
            get { return transaction.Connection; }
        }

        public IsolationLevel IsolationLevel
        {
            get { return transaction.IsolationLevel; }
        }

        public void Rollback()
        {
            using (ExecuteHelper.Begin(dur => context.FireExecuteEvent(this, "Rollback", dur)))
            {
                transaction.Rollback();
            }
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
                    transaction.Dispose();
                }
                disposed = true;
            }
        }

        ~DbTransactionProxy()
        {
            Dispose(false);
        }
    }
}