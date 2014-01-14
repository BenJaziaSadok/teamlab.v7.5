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
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Storage.Interface;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Services.Jabber
{
	[XmppHandler(typeof(Message))]
	class MessageHandler : XmppStanzaHandler
	{
		public override void HandleMessage(XmppStream stream, Message message, XmppHandlerContext context)
		{
			if (!message.HasTo || message.To.IsServer)
			{
				context.Sender.SendTo(stream, XmppStanzaError.ToServiceUnavailable(message));
				return;
			}

			var session = context.SessionManager.GetSession(message.To);
			if (session != null)
			{
				context.Sender.SendTo(session, message);
			}
			else
			{
				StoreOffline(message, context.StorageManager.OfflineStorage);
			}
		}

		private void StoreOffline(Message message, IOfflineStore offlineStore)
		{
			if ((message.Type == MessageType.normal || message.Type == MessageType.chat) && !string.IsNullOrEmpty(message.To.User))
			{
				offlineStore.SaveOfflineMessages(message);
			}
		}
	}
}