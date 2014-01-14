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

using ASC.Xmpp.Core.protocol.Base;
using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Server.Services.Muc2.Room.Member
{
    using Handler;

    [XmppHandler(typeof(Stanza))]
    internal class MucRoomMemberStanzaHandler : XmppStanzaHandler
    {
        public MucRoomMember Member { get; set; }

        internal MucRoomMemberStanzaHandler(MucRoomMember member)
        {
            Member = member;
        }

        public override IQ HandleIQ(ASC.Xmpp.Server.Streams.XmppStream stream, IQ iq, XmppHandlerContext context)
        {
            if (iq.Vcard!=null && iq.Type==IqType.get)
            {
                //Handle vcard
                iq.Vcard = Member.GetVcard();
                iq.Type = IqType.result;
                iq.SwitchDirection();
                return iq;
            }
            return base.HandleIQ(stream, iq, context);
        }

        public override void HandlePresence(Streams.XmppStream stream, Presence presence, XmppHandlerContext context)
        {
            if (presence.Type == PresenceType.available || presence.Type == PresenceType.unavailable)
            {
                if (!ReferenceEquals(Member.Stream, stream))
                {
                    //Set stream
                    Member.Stream = stream;
                    if (presence.Type == PresenceType.available)
                    {
                        //If stream changed then we should broadcast presences
                        Member.ReEnterRoom();
                    }
                }
                Member.ChangePesence(presence);
            }
            else
            {
                //Bad request                
                presence.SwitchDirection();
                presence.From = Member.RoomFrom;
                presence.Type = PresenceType.error;
                presence.Error = new Error(ErrorCondition.BadRequest);
                context.Sender.SendTo(stream, presence);
            }
        }

        public override void HandleMessage(Streams.XmppStream stream, Message msg, XmppHandlerContext context)
        {
            //Private msg
            if (msg.Type==MessageType.chat)
            {
                if (Member.ResolveRoomJid(msg.From)==null)
                {
                    //Error
                    msg.SwitchDirection();
                    msg.From = Member.RoomFrom;
                    msg.Type = MessageType.error;
                    msg.Error = new Error(ErrorCondition.ItemNotFound);
                    context.Sender.SendTo(stream, msg);
                }
                else
                {
                    //Send
                    msg.To = Member.RealJid;
                    msg.From = Member.ResolveRoomJid(msg.From);
                    Member.Send(msg);
                }

            }
            else
            {
                msg.SwitchDirection();
                msg.From = Member.RoomFrom;
                msg.Type = MessageType.error;
                msg.Error = new Error(ErrorCondition.BadRequest);
                context.Sender.SendTo(stream, msg);
            }
        }
    }
}