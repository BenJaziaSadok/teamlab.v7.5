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
using ASC.Xmpp.Core.protocol.Base;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.extensions.multicast;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Services.Multicast
{
	[XmppHandler(typeof(Stanza))]
	class MulticastHandler : XmppStanzaHandler
	{
		public override void HandleMessage(XmppStream stream, Message message, XmppHandlerContext context)
		{
			HandleMulticastStanza(stream, message, context);
		}

		public override void HandlePresence(XmppStream stream, Presence presence, XmppHandlerContext context)
		{
			HandleMulticastStanza(stream, presence, context);
		}


		private void HandleMulticastStanza(XmppStream stream, Stanza stanza, XmppHandlerContext context)
		{
			var addresses = stanza.SelectSingleElement<Addresses>();
            if (addresses != null)
            {
                var jids = addresses.GetAddressList();
                
                addresses.RemoveAllBcc();
                Array.ForEach(addresses.GetAddresses(), a => a.Delivered = true);

                var handlerManager = (XmppHandlerManager)context.ServiceProvider.GetService(typeof(XmppHandlerManager));
                foreach (var to in jids)
                {
                    stanza.To = to;
                    handlerManager.ProcessStreamElement(stanza, stream);
                }
            }
		}
	}
}