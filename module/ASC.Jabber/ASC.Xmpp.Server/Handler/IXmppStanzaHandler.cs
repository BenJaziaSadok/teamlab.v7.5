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

using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Handler
{
	public interface IXmppStanzaHandler : IXmppHandler
	{
		IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context);

		void HandleMessage(XmppStream stream, Message message, XmppHandlerContext context);

		void HandlePresence(XmppStream stream, Presence presence, XmppHandlerContext context);
	}
}
