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

using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.iq.vcard;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Services.Jabber
{
	[XmppHandler(typeof(Vcard))]
	class VCardHandler : XmppStanzaHandler
	{
		public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (!iq.HasTo) iq.To = iq.From;
			if (iq.Type == IqType.get) return GetVCard(stream, iq, context);
			else if (iq.Type == IqType.set) return SetVCard(stream, iq, context);
			else return XmppStanzaError.ToBadRequest(iq);
		}

		private IQ SetVCard(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (iq.From != iq.To) return XmppStanzaError.ToForbidden(iq);

			var answer = new IQ(IqType.result);
			answer.Id = iq.Id;
			answer.To = iq.From;
			answer.From = iq.To;
			context.StorageManager.VCardStorage.SetVCard(iq.To, iq.Vcard);
			answer.Vcard = iq.Vcard;
			return answer;
		}

		private IQ GetVCard(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			var answer = new IQ(IqType.result);
			answer.Id = iq.Id;
			answer.To = iq.From;
			answer.From = iq.To;

			answer.Vcard = iq.To.HasUser ?
				context.StorageManager.VCardStorage.GetVCard(iq.To) :
				GetServiceVcard(iq.To, context);

			if (answer.Vcard == null) return XmppStanzaError.ToNotFound(iq);
			return answer;
		}

		private Vcard GetServiceVcard(Jid jid, XmppHandlerContext context)
		{
			var serviceManager = (XmppServiceManager)context.ServiceProvider.GetService(typeof(XmppServiceManager));
			var service = serviceManager.GetService(jid);
			return service != null ? service.Vcard : null;
		}
	}
}