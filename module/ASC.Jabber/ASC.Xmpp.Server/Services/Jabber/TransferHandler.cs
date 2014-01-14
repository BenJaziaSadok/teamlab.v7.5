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
using ASC.Xmpp.Core.protocol.extensions.bytestreams;
using ASC.Xmpp.Core.protocol.extensions.filetransfer;
using ASC.Xmpp.Core.protocol.extensions.ibb;
using ASC.Xmpp.Core.protocol.extensions.jivesoftware.phone;
using ASC.Xmpp.Core.protocol.extensions.si;
using ASC.Xmpp.Core.protocol.iq.jingle;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;

namespace ASC.Xmpp.Server.Services.Jabber
{
    //si
	[XmppHandler(typeof(SI))]
	
    //bytestreams
    [XmppHandler(typeof(Activate))]
	[XmppHandler(typeof(ByteStream))]
	[XmppHandler(typeof(StreamHost))]
	[XmppHandler(typeof(StreamHostUsed))]
	[XmppHandler(typeof(UdpSuccess))]

    //filetransfer
    [XmppHandler(typeof(File))]
    [XmppHandler(typeof(Range))]

    //ibb
    [XmppHandler(typeof(Base))]
    [XmppHandler(typeof(Close))]
    [XmppHandler(typeof(Data))]
    [XmppHandler(typeof(Open))]

    //livesoftware.phone
    [XmppHandler(typeof(PhoneAction))]
    [XmppHandler(typeof(PhoneEvent))]
    [XmppHandler(typeof(PhoneStatus))]

    //jingle
    [XmppHandler(typeof(GoogleJingle))]
    [XmppHandler(typeof(Jingle))]
    [XmppHandler(typeof(Core.protocol.iq.jingle.Server))]
    [XmppHandler(typeof(Stun))]
    class TransferHandler : XmppStanzaHandler
	{
		public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (!iq.HasTo || !iq.To.HasUser) return XmppStanzaError.ToServiceUnavailable(iq);

			var session = context.SessionManager.GetSession(iq.To);
			if (session != null) context.Sender.SendTo(session, iq);
			return null;
		}
	}
}