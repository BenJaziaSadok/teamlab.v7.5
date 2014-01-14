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
    class ProxyContext
    {
        private readonly Action<ExecutedEventArgs> executedEvent;


        public ProxyContext(Action<ExecutedEventArgs> executedEvent)
        {
            if (executedEvent == null)
            {
                throw new ArgumentNullException("executedEvent");
            }
            this.executedEvent = executedEvent;
        }


        public void FireExecuteEvent(IDbCommand cmd, string method, TimeSpan duration)
        {
            executedEvent(new ExecutedEventArgs("Command." + method, duration, cmd));
        }

        public void FireExecuteEvent(IDbConnection conn, string method, TimeSpan duration)
        {
            executedEvent(new ExecutedEventArgs("Connection." + method, duration));
        }

        public void FireExecuteEvent(IDbTransaction tx, string method, TimeSpan duration)
        {
            executedEvent(new ExecutedEventArgs("Transaction." + method, duration));
        }
    }
}
