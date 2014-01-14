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

using System.Linq;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;

namespace ASC.Xmpp.Server.Services.Muc2.Room
{
    using System;
    using System.Collections.Generic;
    using Handler;
    using Member;


    internal class MucRoomMemberCollection : List<MucRoomMember>
    {
        private readonly XmppServiceManager manager;

        public MucRoomMemberCollection(XmppServiceManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            this.manager = manager;
        }

        internal event MemberActionDelegate MemberAdded = null;

        private void InvokeMemberAdded(MucRoomMember member)
        {
            MemberActionDelegate evt = MemberAdded;
            if (evt != null)
            {
                evt(member);
            }
        }

        internal event MemberActionDelegate MemberRemoved = null;

        private void InvokeMemberRemoved(MucRoomMember member)
        {
            MemberActionDelegate evt = MemberRemoved;
            if (evt != null)
            {
                evt(member);
            }
        }

        internal new void Add(MucRoomMember member)
        {
            if (!Contains(member))
            {
                base.Add(member);
                manager.RegisterService(member);
                InvokeMemberAdded(member);
            }
            else
            {
                throw new Exceptions.MucMemberExistsException();
            }
        }

        internal new void Remove(MucRoomMember member)
        {
            if (Contains(member))
            {
                base.Remove(member);
                InvokeMemberRemoved(member);
                manager.UnregisterService(member.Jid);
            }
        }

        internal new void Clear()
        {
            foreach (MucRoomMember member in this)
            {
                manager.UnregisterService(member.Jid);
            }
            base.Clear();
        }

        internal MucRoomMember FindByJid(Jid jid)
        {
            return Find((x) => x.Jid.Equals(jid));
        }

        internal MucRoomMember FindByRealJid(Jid jid)
        {
            return Find((x) => jid.Equals(x.RealJid));
        }

        internal MucRoomMember this[string jid]
        {
            get
            {
                return FindByJid(new Jid(jid));
            }
        }
        internal MucRoomMember this[Jid jid]
        {
            get
            {
                return FindByJid(jid);
            }
        }

        public void RebindAddress(MucRoomMember member, Jid address)
        {
            manager.UnregisterService(member.Jid);
            member.Jid = address;
            manager.RegisterService(member);
        }

        public MucRoomMember FindByNick(string nickname)
        {
            foreach (MucRoomMember member in this)
            {
                if (member != null && member.Nick != null && member.Nick.Equals(nickname, StringComparison.OrdinalIgnoreCase))
                {
                    return member;
                }
            }
            return null;
        }

        public MucRoomMember FindByBareJid(Jid jidBare)
        {
            return this.Where(member => member != null && member.RealJid != null && member.RealJid.Bare.Equals(jidBare.Bare, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
        }
    }
}