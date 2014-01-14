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

using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Handler
{
	public interface IXmppStreamStartHandler : IXmppHandler
	{
		string Namespace
		{
			get;
		}

		void StreamStartHandle(XmppStream xmppStream, Stream stream, XmppHandlerContext context);
	}
}
