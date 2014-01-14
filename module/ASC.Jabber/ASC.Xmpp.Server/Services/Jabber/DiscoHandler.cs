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
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.iq.disco;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Session;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Utils;

namespace ASC.Xmpp.Server.Services.Jabber
{
	class DiscoHandler : ServiceDiscoHandler
	{
		private IXmppSender xmppSender;

		private XmppSessionManager sessionManager;


		public DiscoHandler(Jid jid)
			: base(jid)
		{

		}

		public override void OnRegister(IServiceProvider serviceProvider)
		{
			sessionManager = (XmppSessionManager)serviceProvider.GetService(typeof(XmppSessionManager));
			xmppSender = (IXmppSender)serviceProvider.GetService(typeof(IXmppSender));
			base.OnRegister(serviceProvider);
		}

		public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (iq.Type == IqType.result || iq.Type == IqType.error)
			{
				IdleWatcher.StopWatch(iq.Id);

				var session = context.SessionManager.GetSession(iq.From);
				if (session != null && iq.Query is DiscoInfo)
				{
					session.ClientInfo.SetDiscoInfo((DiscoInfo)iq.Query);
				}
				if (iq.HasTo)
				{
					session = context.SessionManager.GetSession(iq.To);
					if (session != null) context.Sender.SendTo(session, iq);
					return null;
				}
			}
			else if (iq.HasTo && iq.To.HasUser)
			{
				return GetUserDisco(stream, iq, context);
			}
			return base.HandleIQ(stream, iq, context);
		}

		private IQ GetUserDisco(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (iq.To.HasResource)
			{
				var session = context.SessionManager.GetSession(iq.To);

				if (session != null && iq.Query is DiscoInfo)
				{
					var discoInfo = session.ClientInfo.GetDiscoInfo(((DiscoInfo)iq.Query).Node);
					if (discoInfo != null)
					{
						iq.Query = discoInfo;
						return ToResult(iq);
					}
				}

				if (session == null) return XmppStanzaError.ToRecipientUnavailable(iq);
				context.Sender.SendTo(session, iq);
				IdleWatcher.StartWatch(UniqueId.CreateNewId(), TimeSpan.FromSeconds(4.5f), IQLost, iq);
			}
			else
			{
				if (iq.Query is DiscoInfo && context.UserManager.IsUserExists(iq.To))
				{
					((DiscoInfo)iq.Query).AddIdentity(new DiscoIdentity("registered", "account"));
					return ToResult(iq);
				}
				else if (iq.Query is DiscoItems)
				{
					foreach (var s in context.SessionManager.GetBareJidSessions(iq.To))
					{
						((DiscoItems)iq.Query).AddDiscoItem(new DiscoItem() { Jid = s.Jid });
					}
					return ToResult(iq);
				}
				return XmppStanzaError.ToServiceUnavailable(iq);
			}
			return null;
		}

		private void IQLost(object sender, TimeoutEventArgs e)
		{
			var iq = (IQ)e.Data;
			var session = sessionManager.GetSession(iq.From);
			if (session != null) xmppSender.SendTo(session, XmppStanzaError.ToServiceUnavailable(iq));
		}

		private IQ ToResult(IQ iq)
		{
			if (!iq.Switched) iq.SwitchDirection();
			iq.Type = IqType.result;
			return iq;
		}
	}
}