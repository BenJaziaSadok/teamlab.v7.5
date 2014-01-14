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
using ASC.Core.Users;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.iq.roster;
using ASC.Xmpp.Server;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Storage.Interface;

namespace ASC.Xmpp.Host
{
    class ASCRosterStore : DbRosterStore, IRosterStore
    {
        #region IRosterStore Members

        public override List<UserRosterItem> GetRosterItems(Jid rosterJid)
        {
            try
            {
                ASCContext.SetCurrentTenant(rosterJid.Server);
                var items = GetASCRosterItems(rosterJid);
                items.AddRange(base.GetRosterItems(rosterJid));
                SortRoster(items);
                return items;
            }
            catch (Exception e)
            {
                throw new JabberException("Could not get roster items.", e);
            }
        }

        public override UserRosterItem GetRosterItem(Jid rosterJid, Jid itemJid)
        {
            try
            {
                ASCContext.SetCurrentTenant(rosterJid.Server);
                var u = ASCContext.UserManager.GetUserByUserName(itemJid.User);
                return !string.IsNullOrEmpty(u.UserName) ?
                    ToUserRosterItem(u, itemJid.Server) :
                    base.GetRosterItem(rosterJid, itemJid);
            }
            catch (Exception e)
            {
                throw new JabberException("Could not get roster item.", e);
            }
        }

        public override UserRosterItem SaveRosterItem(Jid rosterJid, UserRosterItem item)
        {
            if (item == null) throw new ArgumentNullException("item");

            ASCContext.SetCurrentTenant(rosterJid.Server);
            if (IsASCRosterItem(rosterJid, item.Jid)) throw new JabberException(ErrorCode.Forbidden);

            return base.SaveRosterItem(rosterJid, item);
        }

        public override void RemoveRosterItem(Jid rosterJid, Jid itemJid)
        {
            ASCContext.SetCurrentTenant(rosterJid.Server);
            if (IsASCRosterItem(rosterJid, itemJid)) throw new JabberException(ErrorCode.Forbidden);

            base.RemoveRosterItem(rosterJid, itemJid);
        }

        #endregion

        private List<UserRosterItem> GetASCRosterItems(Jid jid)
        {
            var items = new List<UserRosterItem>();
            foreach (var u in ASCContext.UserManager.GetUsers())
            {
                if (string.IsNullOrEmpty(u.UserName) || string.Compare(jid.User, u.UserName, true) == 0) continue;
                items.Add(ToUserRosterItem(u, jid.Server));
            }
            //add server
            items.Add(new UserRosterItem(new Jid(jid.Server)) { Name = jid.Server, Subscribtion = SubscriptionType.both, Ask = AskType.NONE, });
            return items;
        }

        private bool IsASCRosterItem(Jid rosterJid, Jid itemJid)
        {
            return ASCContext.UserManager.IsUserNameExists(itemJid.User);
        }

        private void SortRoster(List<UserRosterItem> roster)
        {
            roster.Sort((x, y) => string.Compare(!string.IsNullOrEmpty(x.Name) ? x.Name : x.Jid.ToString(), !string.IsNullOrEmpty(y.Name) ? y.Name : y.Jid.ToString(), true));
        }

        private UserRosterItem ToUserRosterItem(UserInfo u, string server)
        {
            var item = new UserRosterItem(new Jid(u.UserName + "@" + server))
            {
                Name = UserFormatter.GetUserName(u),
                Subscribtion = SubscriptionType.both,
                Ask = AskType.NONE,
            };
            foreach (var g in ASCContext.UserManager.GetUserGroups(u.ID))
            {
                item.Groups.Add(g.Name);
            }
            return item;
        }
    }
}