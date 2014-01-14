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
using ASC.Xmpp.Core.protocol.component;

using ASC.Xmpp.Core.utils;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Authorization
{
	[XmppHandler(typeof(Handshake))]
	class AuthHandshakeHandler : XmppStreamHandler
	{
		private string password;

		public AuthHandshakeHandler(string password)
		{
			this.password = password;
		}

		public override void ElementHandle(XmppStream stream, Element element, XmppHandlerContext context)
		{
			if (stream.Authenticated) return;

			var handshake = (Handshake)element;
			string digest = handshake.Digest;
			string hash = Hash.Sha1Hash(stream.Id + password);
			if (string.Compare(hash, digest, StringComparison.OrdinalIgnoreCase) == 0)
			{
				context.Sender.SendTo(stream, new Handshake()); //TODO: auth with sha1
				//stream.Authenticated = true;
			}
			else
			{
				context.Sender.SendToAndClose(stream, XmppStreamError.NotAuthorized);
			}
		}
	}
}