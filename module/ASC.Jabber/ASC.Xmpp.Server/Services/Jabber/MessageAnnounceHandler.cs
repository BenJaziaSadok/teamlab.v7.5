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
using System.Linq;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.extensions.nickname;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Services.Jabber
{
    [XmppHandler(typeof(Message))]
    class MessageAnnounceHandler : XmppStanzaHandler
    {
        public const string ANNOUNCE = "announce";

        public const string ONLINE = "online";

        public const string ONLINEBROADCAST = "onlinebroadcast";

        public const string SERVICE = "service";

        public const string MOTD = "motd";

        public override void HandleMessage(XmppStream stream, Message message, XmppHandlerContext context)
        {
            if (!message.HasTo || !message.To.HasResource) return;

            string[] commands = message.To.Resource.Split('/');
            if (commands.Length == 1 && commands[0] == ANNOUNCE)
            {
                Announce(stream, message, context);
            }
            else if (commands.Length == 2 && commands[1] == ONLINE)
            {
                AnnounceOnline(stream, message, context);
            }
            else if (commands.Length == 2 && commands[1] == ONLINEBROADCAST)
            {
                AnnounceOnlineBroadcast(stream, message, context);
            }
            else if (commands.Length == 2 && commands[1] == SERVICE)
            {
                AnnounceService(stream, message, context);
            }
            else
            {
                context.Sender.SendTo(stream, XmppStanzaError.ToServiceUnavailable(message));
            }
        }

        private void Announce(XmppStream stream, Message message, XmppHandlerContext context)
        {
            var userName = GetUser(message);
            message.Body = string.Format("{0} announces {1}", userName, message.Body);
            var offlineMessages = new List<Message>();

            foreach (var user in context.UserManager.GetUsers(stream.Domain))
            {
                message.To = user.Jid;
                var session = context.SessionManager.GetSession(message.To);
                if (session != null)
                {
                    context.Sender.SendTo(session, message);
                }
                else
                {
                    offlineMessages.Add(message);
                }
            }
            context.StorageManager.OfflineStorage.SaveOfflineMessages(offlineMessages.ToArray());
        }

        private void AnnounceOnline(XmppStream stream, Message message, XmppHandlerContext context)
        {
            foreach (var session in context.SessionManager.GetSessions().Where(x => x.Available))
            {
                message.To = session.Jid;
                context.Sender.SendTo(session, message);
            }
        }

        private void AnnounceOnlineBroadcast(XmppStream stream, Message message, XmppHandlerContext context)
        {
            string user = GetUser(message);
            message.Body = string.Format("{0} says:\r\n{1}", user, message.Body);
            AnnounceService(stream, message, context);
        }

        private void AnnounceService(XmppStream stream, Message message, XmppHandlerContext context)
        {
            message.From = new Jid(stream.Domain);
            message.Nickname = null;
            AnnounceOnline(stream, message, context);
        }

        private string GetUser(Message message)
        {
            var nick = message.SelectSingleElement<Nickname>();
            return nick != null ? nick.Value : message.From.User;
        }
    }
}