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
using System.Collections.Generic;
using System.ServiceModel;
using System.Text.RegularExpressions;
using ASC.Core;
using ASC.Core.Notify.Jabber;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.extensions.commands;
using ASC.Xmpp.Core.protocol.x;
using ASC.Xmpp.Server;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Storage;

namespace ASC.Xmpp.Host
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single)]
    public class JabberService : IJabberService
    {
        private readonly XmppServer xmppServer;


        public static XmppServer CurrentXmppServer
        {
            get;
            private set;
        }


        public JabberService(XmppServer xmppServer)
        {
            this.xmppServer = xmppServer;
            CurrentXmppServer = xmppServer;
        }


        public bool IsUserAvailable(string username, int tenantId)
        {
            var session = xmppServer.SessionManager.GetSession(GetJid(username, tenantId));
            return session != null && session.Available;
        }

        public int GetNewMessagesCount(string userName, int tenantId)
        {
            return xmppServer.StorageManager.OfflineStorage.GetOfflineMessagesCount(GetJid(userName, tenantId));
        }

        public string GetUserToken(string userName, int tenantId)
        {
            return xmppServer.AuthManager.GetUserToken(userName);
        }

        public IDictionary<string, string> GetClientConfiguration(int tenantId)
        {
            var t = CoreContext.TenantManager.GetTenant(tenantId);
            return new Dictionary<string, string>
            {
                { "Domain", t != null ? t.TenantDomain : null },
                { "BoshUri", XmppRuntimeInfo.BoshUri.ToString() },
                { "Port", XmppRuntimeInfo.Port.ToString() },
            };
        }

        public void SendMessage(string to, string subject, string text, int tenantId)
        {
            if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(text)) return;

            var jid = GetJid(to, tenantId);
            text = ModifyJabberNotification(text);
            var message = new Message(
                jid,
                new Jid(GetTenantDomain(tenantId)),
                MessageType.chat,
                text,
                subject);

            var session = xmppServer.SessionManager.GetSession(jid);
            if (session != null)
            {
                var sender = (IXmppSender)xmppServer.GetService(typeof(IXmppSender));
                sender.SendTo(session, message);
            }
            else
            {
                xmppServer.StorageManager.OfflineStorage.SaveOfflineMessages(message);
            }
            var archive = xmppServer.StorageManager.GetStorage<DbMessageArchive>("archive");
            if (archive != null) archive.SaveMessages(message);
        }

        public void SendCommand(string from, string to, string command, int tenantId)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to) || string.IsNullOrEmpty(command)) return;

            var iq = new IQ(IqType.set, GetJid(from, tenantId), GetJid(to, tenantId))
            {
                Query = new Command(command)
            };

            var session = xmppServer.SessionManager.GetSession(GetJid(to, tenantId));
            if (session != null)
            {
                var sender = (IXmppSender)xmppServer.GetService(typeof(IXmppSender));
                sender.SendTo(session, iq);
            }
        }


        private Jid GetJid(string userName, int tenant)
        {
            return new Jid(userName, GetTenantDomain(tenant), null);
        }

        private string GetTenantDomain(int tenant)
        {
            var t = CoreContext.TenantManager.GetTenant(tenant);
            if (t == null) throw new Exception(string.Format("Tenant with id {0} not found.", tenant));
            return t.TenantDomain;
        }

        private static string ModifyJabberNotification(string message)
        {
            message = message.Replace("\r\n", "\n");
            message = message.Trim('\n', '\r');
            message = Regex.Replace(message, "\n{3,}", "\n\n");
            return message;
        }
    }
}