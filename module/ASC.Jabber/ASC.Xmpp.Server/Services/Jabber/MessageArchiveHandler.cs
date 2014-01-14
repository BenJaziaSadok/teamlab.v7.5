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
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.x;
using ASC.Xmpp.Core.protocol.x.tm.history;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Utils;
using log4net;

namespace ASC.Xmpp.Server.Services.Jabber
{
    [XmppHandler(typeof(Message))]
    [XmppHandler(typeof(History))]
    [XmppHandler(typeof(PrivateLog))]
    class MessageArchiveHandler : XmppStanzaHandler
    {
        private DbMessageArchive archiveStore;

        private static readonly int BUFFER_SIZE = 100;

        private List<Message> messageBuffer = new List<Message>(BUFFER_SIZE);

        private static readonly ILog log = LogManager.GetLogger(typeof(MessageArchiveHandler));


        public override void OnRegister(IServiceProvider serviceProvider)
        {
            archiveStore = ((StorageManager)serviceProvider.GetService(typeof(StorageManager))).GetStorage<DbMessageArchive>("archive");
        }

        public override void OnUnregister(IServiceProvider serviceProvider)
        {
            FlushMessageBuffer();
        }

        public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            if (iq.Query is PrivateLog && iq.Type == IqType.get) return GetPrivateLog(stream, iq, context);
            if (iq.Query is PrivateLog && iq.Type == IqType.set) return SetPrivateLog(stream, iq, context);
            if (iq.Query is PrivateLog && (iq.Type == IqType.result || iq.Type == IqType.error)) return null;
            if (iq.Query is History && iq.Type == IqType.get) return GetHistory(stream, iq, context);
            return XmppStanzaError.ToServiceUnavailable(iq);
        }

        public override void HandleMessage(XmppStream stream, Message message, XmppHandlerContext context)
        {
            if (!message.HasTo) return;

            if (archiveStore.GetMessageLogging(message.From, message.To))
            {
                if (!string.IsNullOrEmpty(message.Body) ||
                    !string.IsNullOrEmpty(message.Subject) ||
                    !string.IsNullOrEmpty(message.Thread) ||
                    message.Html != null)
                {
                    var flush = false;
                    lock (messageBuffer)
                    {
                        //Add xdelay
                        if (message.XDelay == null)
                        {
                            message.XDelay = new Delay();
                            message.XDelay.Stamp = DateTime.UtcNow;
                        }
                        messageBuffer.Add(message);

                        flush = BUFFER_SIZE <= messageBuffer.Count;
                    }
                    if (flush) FlushMessageBuffer();
                }
            }
        }


        private IQ GetPrivateLog(XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            if (!iq.HasTo) return XmppStanzaError.ToBadRequest(iq);

            var privateLog = (PrivateLog)iq.Query;
            privateLog.RemoveAllChildNodes();
            var logging = archiveStore.GetMessageLogging(iq.From, iq.To);
            privateLog.AddChild(new PrivateLogItem() { Jid = iq.To, Log = logging });

            iq.SwitchDirection();
            iq.Type = IqType.result;
            return iq;
        }

        private IQ SetPrivateLog(XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            var privateLog = (PrivateLog)iq.Query;
            foreach (var item in privateLog.SelectElements<PrivateLogItem>())
            {
                archiveStore.SetMessageLogging(iq.From, item.Jid, item.Log);
                var to = new Jid(item.Jid.Bare);
                var session = context.SessionManager.GetSession(to);
                if (session != null)
                {
                    var info = new IQ(IqType.set);
                    info.Id = UniqueId.CreateNewId();
                    info.From = iq.From;
                    info.To = session.Jid;
                    info.Query = new PrivateLog();
                    info.Query.AddChild(new PrivateLogItem() { Jid = iq.From, Log = item.Log });
                    context.Sender.SendTo(session, info);
                }
            }
            privateLog.RemoveAllChildNodes();

            iq.SwitchDirection();
            iq.Type = IqType.result;
            return iq;
        }

        private IQ GetHistory(XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            if (!iq.HasTo) return XmppStanzaError.ToServiceUnavailable(iq);

            FlushMessageBuffer();

            var history = (History)iq.Query;
            history.RemoveAllChildNodes();
            foreach (var m in archiveStore.GetMessages(iq.From, iq.To, history.From, history.To, history.Count))
            {
                history.AddChild(HistoryItem.FromMessage(m));
            }

            iq.Type = IqType.result;
            iq.SwitchDirection();
            return iq;
        }

        private void FlushMessageBuffer()
        {
            Message[] buffercopy = null;
            lock (messageBuffer)
            {
                buffercopy = messageBuffer.ToArray();
                messageBuffer.Clear();
            }

            if (buffercopy.Length == 0) return;

            try
            {
                archiveStore.SaveMessages(buffercopy);
                log.DebugFormat("Flush messages buffer, count: {0}", buffercopy.Length);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error flush messages buffer, count: {0}, exception: {1}", buffercopy.Length, ex);
            }
        }
    }
}