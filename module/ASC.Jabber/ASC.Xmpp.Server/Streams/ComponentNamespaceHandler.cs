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
using System.Text;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.component;
using ASC.Xmpp.Server.Handler;
using Uri = ASC.Xmpp.Core.protocol.Uri;

namespace ASC.Xmpp.Server.Streams
{
	[XmppHandler(typeof(Handshake))]
	class ComponentNamespaceHandler : IXmppStreamStartHandler
	{
		public string Namespace
		{
			get { return Uri.ACCEPT; }
		}

		public void StreamStartHandle(XmppStream xmppStream, Stream stream, XmppHandlerContext context)
		{
			var streamHeader = new StringBuilder();
			streamHeader.AppendLine("<?xml version='1.0' encoding='UTF-8'?>");
			streamHeader.AppendFormat("<stream:{0} xmlns:{0}='{1}' xmlns='{2}' ", Uri.PREFIX, Uri.STREAM, Uri.ACCEPT);
			streamHeader.AppendFormat("from='{0}' id='{1}' version='1.0'>", stream.To, xmppStream.Id);

			context.Sender.SendTo(xmppStream, streamHeader.ToString());
		}

		public void OnRegister(IServiceProvider serviceProvider)
		{

		}

		public void OnUnregister(IServiceProvider serviceProvider)
		{

		}
	}
}