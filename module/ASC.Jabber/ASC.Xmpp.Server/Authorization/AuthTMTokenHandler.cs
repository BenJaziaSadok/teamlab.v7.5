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

using ASC.Xmpp.Core.protocol.sasl;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Authorization
{
	[XmppHandler(typeof(TMToken))]
	class AuthTMTokenHandler : XmppStreamHandler
	{
		public override void ElementHandle(XmppStream stream, Element element, XmppHandlerContext context)
		{
			if (stream.Authenticated) return;

			var user = context.AuthManager.RestoreUserToken(((TMToken)element).Value);
			if (!string.IsNullOrEmpty(user))
			{
				stream.Authenticate(user);
				context.Sender.ResetStream(stream);
				context.Sender.SendTo(stream, new Success());
			}
			else
			{
				context.Sender.SendToAndClose(stream, XmppFailureError.NotAuthorized);
			}
		}
	}
}