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

namespace ASC.Common.Data
{
    class DbNestedTransaction : IDbTransaction
    {
        private readonly IDbTransaction transaction;


        public DbNestedTransaction(IDbTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            this.transaction = transaction;
        }

        public IDbConnection Connection
        {
            get { return transaction.Connection; }
        }

        public IsolationLevel IsolationLevel
        {
            get { return transaction.IsolationLevel; }
        }

        public void Commit()
        {
        }

        public void Rollback()
        {
        }

        public void Dispose()
        {
        }
    }
}