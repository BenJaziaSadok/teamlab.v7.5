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
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Server.Authorization;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Services;
using ASC.Xmpp.Server.Session;
using ASC.Xmpp.Server.Statistics;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Users;

namespace ASC.Xmpp.Server
{
	public class XmppServer : IServiceProvider, IDisposable
	{
		private UserManager userManager;

		private XmppStreamManager streamManager;

		private XmppGateway gateway;

		private XmppSender sender;

		private XmppHandlerManager handlerManager;

		private XmppServiceManager serviceManager;


		public StorageManager StorageManager
		{
			get;
			private set;
		}

		public AuthManager AuthManager
		{
			get;
			private set;
		}

		public XmppSessionManager SessionManager
		{
			get;
			private set;
		}


		public XmppServer()
		{
			StorageManager = new StorageManager();
			userManager = new UserManager(StorageManager);
			AuthManager = new AuthManager();

			streamManager = new XmppStreamManager();
			SessionManager = new XmppSessionManager();

			gateway = new XmppGateway();
			sender = new XmppSender(gateway);

			serviceManager = new XmppServiceManager(this);
			handlerManager = new XmppHandlerManager(streamManager, gateway, sender, this);
		}

		public void AddXmppListener(IXmppListener listener)
		{
			gateway.AddXmppListener(listener);
		}

		public void RemoveXmppListener(string name)
		{
			gateway.RemoveXmppListener(name);
		}

		public void StartListen()
		{
			NetStatistics.Enabled = true;
			gateway.Start();
		}

		public void StopListen()
		{
			gateway.Stop();
		}

		public void RegisterXmppService(IXmppService service)
		{
			serviceManager.RegisterService(service);
		}

		public void UnregisterXmppService(Jid jid)
		{
			serviceManager.UnregisterService(jid);
		}

		public IXmppService GetXmppService(Jid jid)
		{
			return serviceManager.GetService(jid);
		}

		public void Dispose()
        {
            StorageManager.Dispose();
        }

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IXmppReceiver))
			{
				return gateway;
			}
			if (serviceType == typeof(IXmppSender))
			{
				return sender;
			}
			if (serviceType == typeof(XmppSessionManager))
			{
				return SessionManager;
			}
			if (serviceType == typeof(XmppStreamManager))
			{
				return streamManager;
			}
			if (serviceType == typeof(UserManager))
			{
				return userManager;
			}
			if (serviceType == typeof(StorageManager))
			{
				return StorageManager;
			}
			if (serviceType == typeof(XmppServiceManager))
			{
				return serviceManager;
			}
			if (serviceType == typeof(AuthManager))
			{
				return AuthManager;
			}
			if (serviceType == typeof(XmppHandlerManager))
			{
				return handlerManager;
			}
			return null;
		}
	}
}