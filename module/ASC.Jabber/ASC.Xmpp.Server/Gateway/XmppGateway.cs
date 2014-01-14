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
using ASC.Collections;
using ASC.Xmpp.Server.Statistics;
using log4net;

namespace ASC.Xmpp.Server.Gateway
{
	class XmppGateway : IXmppReceiver
	{
		private object syncRoot = new object();

		private bool started = false;

        private IDictionary<string, IXmppListener> listeners = new SynchronizedDictionary<string, IXmppListener>();

        private IDictionary<string, string> connectionListenerMap = new SynchronizedDictionary<string, string>();

		private readonly static ILog log = LogManager.GetLogger(typeof(XmppGateway));


		public void AddXmppListener(IXmppListener listener)
		{
			lock (syncRoot)
			{
				try
				{
					if (started) throw new InvalidOperationException();
					if (listener == null) throw new ArgumentNullException("listener");

					listeners.Add(listener.Name, listener);
					listener.OpenXmppConnection += OpenXmppConnection;

					log.DebugFormat("Add listener '{0}'", listener.Name);
				}
				catch (Exception e)
				{
					log.ErrorFormat("Error add listener '{0}': {1}", listener.Name, e);
					throw;
				}
			}
		}

		public void RemoveXmppListener(string name)
		{
			lock (syncRoot)
			{
				try
				{
					if (started) throw new InvalidOperationException();
					if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

					if (listeners.ContainsKey(name))
					{
						var listener = listeners[name];
						listener.OpenXmppConnection -= OpenXmppConnection;
						listeners.Remove(name);

						log.DebugFormat("Remove listener '{0}'", listener.Name);
					}
				}
				catch (Exception e)
				{
					log.ErrorFormat("Error remove listener '{0}': {1}", name, e);
					throw;
				}
			}
		}

		public void Start()
		{
			lock (syncRoot)
			{
				foreach (var listener in listeners.Values)
				{
					try
					{
						listener.Start();
						log.DebugFormat("Started listener '{0}'", listener.Name);
					}
					catch (Exception e)
					{
						log.ErrorFormat("Error start listener '{0}': {1}", listener.Name, e);
					}
				}
				started = true;
			}
		}

		public void Stop()
		{
			lock (syncRoot)
			{
				foreach (var listener in listeners.Values)
				{
					try
					{
						listener.Stop();
						log.DebugFormat("Stopped listener '{0}'", listener.Name);
					}
					catch (Exception e)
					{
						log.ErrorFormat("Error stop listener '{0}': {1}", listener.Name, e);
					}
				}
				started = false;
			}

			log.DebugFormat("Net statistics: read bytes {0}, write bytes {1}", NetStatistics.GetReadBytes(), NetStatistics.GetWriteBytes());
		}

		public IXmppConnection GetXmppConnection(string connectionId)
		{
			if (string.IsNullOrEmpty(connectionId)) return null;

			string listenerName = null;
			if (!connectionListenerMap.TryGetValue(connectionId, out listenerName) || listenerName == null) return null;

			IXmppListener listener = null;
			if (!listeners.TryGetValue(listenerName, out listener) || listener == null) return null;

			return listener.GetXmppConnection(connectionId);
		}


		public event EventHandler<XmppStreamStartEventArgs> XmppStreamStart;

		public event EventHandler<XmppStreamEndEventArgs> XmppStreamEnd;

		public event EventHandler<XmppStreamEventArgs> XmppStreamElement;


		private void OpenXmppConnection(object sender, XmppConnectionOpenEventArgs e)
		{
			connectionListenerMap[e.XmppConnection.Id] = ((IXmppListener)sender).Name;

			e.XmppConnection.Closed += XmppConnectionClose;
			e.XmppConnection.XmppStreamEnd += XmppConnectionXmppStreamEnd;
			e.XmppConnection.XmppStreamElement += XmppConnectionXmppStreamElement;
			e.XmppConnection.XmppStreamStart += XmppConnectionXmppStreamStart;
		}

		private void XmppConnectionClose(object sender, XmppConnectionCloseEventArgs e)
		{
			var connection = (IXmppConnection)sender;

			connection.XmppStreamStart -= XmppConnectionXmppStreamStart;
			connection.XmppStreamElement -= XmppConnectionXmppStreamElement;
			connection.XmppStreamEnd -= XmppConnectionXmppStreamEnd;
			connection.Closed -= XmppConnectionClose;
			connectionListenerMap.Remove(connection.Id);
		}


		private void XmppConnectionXmppStreamStart(object sender, XmppStreamStartEventArgs e)
		{
			var handler = XmppStreamStart;
			if (handler != null) handler(this, e);
		}

		private void XmppConnectionXmppStreamElement(object sender, XmppStreamEventArgs e)
		{
			var handler = XmppStreamElement;
			if (handler != null) handler(this, e);
		}

		private void XmppConnectionXmppStreamEnd(object sender, XmppStreamEndEventArgs e)
		{
			var handler = XmppStreamEnd;
			if (handler != null) handler(this, e);
		}
	}
}
