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
using System.IO;
using System.Text;
using System.Xml;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Session;
using ASC.Xmpp.Server.Streams;
using log4net;
using Uri = ASC.Xmpp.Core.protocol.Uri;

namespace ASC.Xmpp.Server.Gateway
{
    class XmppSender : IXmppSender
    {
        private XmppGateway gateway;

        private static readonly ILog log = LogManager.GetLogger("ASC.Xmpp.Server.Messages");

        private XmppXMLSchemaValidator validator = new XmppXMLSchemaValidator();

        private const string SEND_FORMAT = "Xmpp stream: connection {0}, namespace {1}\r\n\r\n(S) -------------------------------------->>\r\n{2}\r\n";

        public XmppSender(XmppGateway gateway)
        {
            if (gateway == null) throw new ArgumentNullException("gateway");

            this.gateway = gateway;
        }

        #region IXmppSender Members

        public void SendTo(XmppSession to, Node node)
        {
            if (to == null) throw new ArgumentNullException("to");
            SendTo(to.Stream, node);
        }

        public void SendTo(XmppStream to, Node node)
        {
            if (to == null) throw new ArgumentNullException("to");
            if (node == null) throw new ArgumentNullException("node");

            var connection = GetXmppConnection(to.ConnectionId);
            if (connection != null)
            {
                log.DebugFormat(SEND_FORMAT, to.ConnectionId, to.Namespace, node.ToString(Formatting.Indented));
                validator.ValidateNode(node, to, null);
                connection.Send(node, Encoding.UTF8);
            }
        }

        public void SendTo(XmppStream to, string text)
        {
            if (to == null) throw new ArgumentNullException("to");
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException("text");

            var connection = GetXmppConnection(to.ConnectionId);
            if (connection != null)
            {
                log.DebugFormat(SEND_FORMAT, to.ConnectionId, to.Namespace, text);
                connection.Send(text, Encoding.UTF8);
            }
        }

        public void CloseStream(XmppStream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            var connection = GetXmppConnection(stream.ConnectionId);
            if (connection != null)
            {
                connection.Close();
            }
        }

        public void SendToAndClose(XmppStream to, Node node)
        {
            try
            {
                SendTo(to, node);
                SendTo(to, string.Format("</stream:{0}>", Uri.PREFIX));
            }
            finally
            {
                CloseStream(to);
            }
        }

        public void ResetStream(XmppStream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            var connection = GetXmppConnection(stream.ConnectionId);
            if (connection != null)
            {
                connection.Reset();
            }
        }

        public IXmppConnection GetXmppConnection(string connectionId)
        {
            return gateway.GetXmppConnection(connectionId);
        }

        public bool Broadcast(ICollection<XmppSession> sessions, Node node)
        {
            if (sessions == null) throw new ArgumentNullException("sessions");
            foreach (var session in sessions)
            {
                try
                {
                    SendTo(session, node);
                }
                catch (Exception ex)
                {
                    if (ex is IOException || ex is ObjectDisposedException)
                    {
                        // ignore
                    }
                    else
                    {
                        log.ErrorFormat("Can not send to {0} in broadcast: {1}", session, ex);
                    }
                }
            }
            return 0 < sessions.Count;
        }

        #endregion
    }
}