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
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Handler
{
	public class XmppStanzaHandler : IXmppStanzaHandler
	{
		public virtual IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			return null;
		}

		public virtual void HandleMessage(XmppStream stream, Message message, XmppHandlerContext context)
		{

		}

		public virtual void HandlePresence(XmppStream stream, Presence presence, XmppHandlerContext context)
		{

		}

		public virtual void OnRegister(IServiceProvider serviceProvider)
		{

		}

		public virtual void OnUnregister(IServiceProvider serviceProvider)
		{

		}
	}
}
