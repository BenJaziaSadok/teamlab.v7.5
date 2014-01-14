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
using ASC.Xmpp.Server.Session;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Gateway
{
	public interface IXmppSender
	{
		void SendTo(XmppStream to, Node node);

		void SendTo(XmppStream to, string text);

		void SendTo(XmppSession to, Node node);

		void SendToAndClose(XmppStream to, Node node);

		bool Broadcast(ICollection<XmppSession> sessions, Node node);

		void CloseStream(XmppStream stream);

		void ResetStream(XmppStream stream);

		IXmppConnection GetXmppConnection(string connectionId);
	}
}
