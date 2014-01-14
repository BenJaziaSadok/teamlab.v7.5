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
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.iq.disco;
using ASC.Xmpp.Core.protocol.iq.vcard;
using ASC.Xmpp.Server.Authorization;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Services.Jabber
{
    class JabberService : XmppServiceBase
    {
        private MessageAnnounceHandler messageAnnounceHandler;

        public override Vcard Vcard
        {
            get { return new Vcard() { Fullname = Name, Description = "© 2008-2011 Assensio System SIA", Url = "http://teamlab.com" }; }
        }

        public override void Configure(IDictionary<string, string> properties)
        {
            DiscoInfo.AddIdentity(new DiscoIdentity("server", Name, "im"));
            
            Handlers.Add(new ClientNamespaceHandler());
            Handlers.Add(new AuthDigestMD5Handler());
            Handlers.Add(new AuthTMTokenHandler());
            Handlers.Add(new BindSessionHandler());
            Handlers.Add(new RosterHandler());
            Handlers.Add(new VCardHandler());
            Handlers.Add(new VerTimePingHandler());
            Handlers.Add(new PrivateHandler());
            Handlers.Add(new PresenceHandler());
            Handlers.Add(new MessageHandler());
            Handlers.Add(new MessageArchiveHandler());
            Handlers.Add(new LastHandler());
            Handlers.Add(new RegisterHandler());
            Handlers.Add(new TransferHandler());
            Handlers.Add(new CommandHandler());
            Handlers.Add(new OfflineProvider(Jid));
            Handlers.Add(new DiscoHandler(Jid));
            messageAnnounceHandler = new MessageAnnounceHandler();
        }

        protected override void OnRegisterCore(XmppHandlerManager handlerManager, XmppServiceManager serviceManager, IServiceProvider serviceProvider)
        {
            var jid = new Jid(Jid.ToString());
            jid.Resource = MessageAnnounceHandler.ANNOUNCE;
            handlerManager.AddXmppHandler(jid, messageAnnounceHandler);
        }

        protected override void OnUnregisterCore(XmppHandlerManager handlerManager, XmppServiceManager serviceManager, IServiceProvider serviceProvider)
        {
            handlerManager.RemoveXmppHandler(messageAnnounceHandler);
        }
    }
}