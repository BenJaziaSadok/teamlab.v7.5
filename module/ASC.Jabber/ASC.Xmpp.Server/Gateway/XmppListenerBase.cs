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
using System.Threading;
using ASC.Collections;

namespace ASC.Xmpp.Server.Gateway
{
    public abstract class XmppListenerBase : IXmppListener
    {
        private IDictionary<string, IXmppConnection> connections = new SynchronizedDictionary<string, IXmppConnection>();

        private ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        protected bool Started
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            set;
        }

        public void Start()
        {
            lock (this)
            {
                if (Started) return;

                connections.Clear();

                Started = true;
                DoStart();
            }
        }

        public void Stop()
        {
            lock (this)
            {
                if (!Started) return;

                Started = false;
                DoStop();

                var keys = new string[connections.Keys.Count];
                connections.Keys.CopyTo(keys, 0);
                foreach (var key in keys)
                {
                    if (connections.ContainsKey(key)) connections[key].Close();
                }
                connections.Clear();
            }
        }

        public IXmppConnection GetXmppConnection(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId)) return null;
            try
            {
                locker.EnterReadLock();
                return connections.ContainsKey(connectionId) ? connections[connectionId] : null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public event EventHandler<XmppConnectionOpenEventArgs> OpenXmppConnection;

        protected void AddNewXmppConnection(IXmppConnection xmppConnection)
        {
            if (xmppConnection == null) throw new ArgumentNullException("xmppConnection");

            try
            {
                locker.EnterWriteLock();
                connections.Add(xmppConnection.Id, xmppConnection);
                xmppConnection.Closed += XmppConnectionClosed;
            }
            finally
            {
                locker.ExitWriteLock();
            }

            var handler = OpenXmppConnection;
            if (handler != null) handler(this, new XmppConnectionOpenEventArgs(xmppConnection));

            xmppConnection.BeginReceive();
        }

        private void XmppConnectionClosed(object sender, XmppConnectionCloseEventArgs e)
        {
            try
            {
                locker.EnterWriteLock();

                var connection = (IXmppConnection)sender;
                connection.Closed -= XmppConnectionClosed;
                connections.Remove(connection.Id);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        #region IConfigurable Members

        public abstract void Configure(IDictionary<string, string> properties);

        #endregion

        protected abstract void DoStart();

        protected abstract void DoStop();
    }
}