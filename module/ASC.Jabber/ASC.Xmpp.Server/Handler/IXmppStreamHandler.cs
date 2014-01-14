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

using System.Collections.Generic;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Streams;


namespace ASC.Xmpp.Server.Handler
{
	public interface IXmppStreamHandler : IXmppHandler
	{
		void ElementHandle(XmppStream stream, Element element, XmppHandlerContext context);

		void StreamEndHandle(XmppStream stream, ICollection<Node> notSendedBuffer, XmppHandlerContext context);
	}
}
