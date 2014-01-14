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

using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.Base;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.x.muc.iq;
using Error = ASC.Xmpp.Core.protocol.client.Error;

namespace ASC.Xmpp.Server.Services.Muc2
{
    using Handler;
    using Helpers;
    using Room.Settings;
    using Utils;

    [XmppHandler(typeof(Stanza))]
    internal class MucStanzaHandler : XmppStanzaHandler
    {
        public MucService Service { get; set; }

        internal MucStanzaHandler(MucService service)
        {
            Service = service;
        }

        public override IQ HandleIQ(Streams.XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            Unique unique = (Unique)iq.SelectSingleElement(typeof(Unique));
            if (unique != null)
            {
                // Gen unique id
                unique.Value = UniqueId.CreateNewId(16);
                iq.Type = IqType.result;
                iq.SwitchDirection();
                return iq;
            }
            iq.SwitchDirection();
            iq.Type = IqType.error;
            iq.Error = new Error(ErrorType.cancel, ErrorCondition.ItemNotFound);
            return iq;
        }

        private bool IsUserAbleToCreateRooms(Jid @from)
        {
            return true;//TODO: configuration
        }

        public override void HandlePresence(ASC.Xmpp.Server.Streams.XmppStream stream, Presence presence, XmppHandlerContext context)
        {
            //Presence to open new room
            if (IsUserAbleToCreateRooms(presence.From) && MucHelpers.IsJoinRequest(presence))
            {
                //Register
                Service.CreateRoom(new Jid(presence.To.Bare), null);
                Service.HandlerManager.ProcessStreamElement(presence, stream);//Forward to room
            }
            else
            {
                //Return error
                presence.Type = PresenceType.error;
                presence.Error = new Error(ErrorType.cancel, ErrorCondition.NotAllowed);
                presence.SwitchDirection();
                context.Sender.SendTo(stream, presence);
            }
        }

        public override void HandleMessage(ASC.Xmpp.Server.Streams.XmppStream stream, Message msg, XmppHandlerContext context)
        {
            msg.SwitchDirection();
            msg.Type = MessageType.error;
            msg.Error = new Error(ErrorType.cancel, ErrorCondition.ItemNotFound);
            context.Sender.SendTo(stream, msg);

        }
    }
}