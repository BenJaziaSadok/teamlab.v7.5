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
using System.Data;
using ASC.Collections;
using ASC.Common.Data.Sql;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.iq.roster;
using ASC.Xmpp.Server.Storage.Interface;
using UserRosterItemDic = System.Collections.Generic.Dictionary<ASC.Xmpp.Core.protocol.Jid, ASC.Xmpp.Server.Storage.UserRosterItem>;

namespace ASC.Xmpp.Server.Storage
{
    public class DbRosterStore : DbStoreBase, IRosterStore
    {
        private static readonly string GroupSeparator = "$@$;";

        private IDictionary<Jid, UserRosterItemDic> cache;

        private object syncRoot = new object();


        protected IDictionary<Jid, UserRosterItemDic> RosterItems
        {
            get
            {
                if (cache == null)
                {
                    lock (syncRoot)
                    {
                        if (cache == null) cache = LoadRosterItems();
                    }
                }
                return cache;
            }
        }

        protected override SqlCreate[] GetCreateSchemaScript()
        {
            var t1 = new SqlCreate.Table("jabber_roster", true)
                .AddColumn("jid", DbType.String, 255, true)
                .AddColumn("item_jid", DbType.String, 255, true)
                .AddColumn("name", DbType.String, 512)
                .AddColumn(new SqlCreate.Column("subscription", DbType.Int32).NotNull(true).Default(0))
                .AddColumn(new SqlCreate.Column("ask", DbType.Int32).NotNull(true).Default(0))
                .AddColumn("groups", DbType.String, UInt16.MaxValue)
                .PrimaryKey("jid", "item_jid");
            return new[] { t1 };
        }


        #region IRosterStore Members

        public virtual List<UserRosterItem> GetRosterItems(Jid rosterJid)
        {
            try
            {
                lock (syncRoot)
                {
                    var bareJid = new Jid(rosterJid.Bare.ToLowerInvariant());
                    if (RosterItems.ContainsKey(bareJid)) return new List<UserRosterItem>(RosterItems[bareJid].Values);
                    return new List<UserRosterItem>();
                }
            }
            catch (Exception e)
            {
                throw new JabberException("Could not get roster items.", e);
            }
        }

        public virtual List<UserRosterItem> GetRosterItems(Jid rosterJid, SubscriptionType subscriptionType)
        {
            return GetRosterItems(rosterJid).FindAll(i => { return i.Subscribtion == subscriptionType; });
        }

        public virtual UserRosterItem GetRosterItem(Jid rosterJid, Jid itemJid)
        {
            return GetRosterItems(rosterJid).Find(i => { return string.Compare(i.Jid.Bare, itemJid.Bare, true) == 0; });
        }

        public virtual UserRosterItem SaveRosterItem(Jid rosterJid, UserRosterItem item)
        {
            if (item == null) throw new ArgumentNullException("item");

            try
            {
                lock (syncRoot)
                {
                    if (string.IsNullOrEmpty(item.Name)) item.Name = item.Jid.Bare;
                    rosterJid = new Jid(rosterJid.Bare.ToLowerInvariant());

                    ExecuteNonQuery(new SqlInsert("jabber_roster", true)
                        .InColumnValue("jid", rosterJid.ToString())
                        .InColumnValue("item_jid", item.Jid.ToString())
                        .InColumnValue("name", item.Name)
                        .InColumnValue("subscription", (Int32)item.Subscribtion)
                        .InColumnValue("ask", (Int32)item.Ask)
                        .InColumnValue("groups", string.Join(GroupSeparator, item.Groups.ToArray())));

                    if (!RosterItems.ContainsKey(rosterJid)) RosterItems[rosterJid] = new UserRosterItemDic();
                    RosterItems[rosterJid][item.Jid] = item;

                    return item;
                }
            }
            catch (Exception e)
            {
                throw new JabberException("Could not save or update roster item.", e);
            }
        }

        public virtual void RemoveRosterItem(Jid rosterJid, Jid itemJid)
        {
            try
            {
                lock (syncRoot)
                {
                    rosterJid = new Jid(rosterJid.Bare.ToLowerInvariant());
                    itemJid = new Jid(itemJid.Bare.ToLowerInvariant());

                    if (RosterItems.ContainsKey(rosterJid) && RosterItems[rosterJid].ContainsKey(itemJid))
                    {
                        ExecuteNonQuery(new SqlDelete("jabber_roster").Where("jid", rosterJid.ToString()).Where("item_jid", itemJid.ToString()));
                        RosterItems[rosterJid].Remove(itemJid);
                    }
                }
            }
            catch (Exception e)
            {
                throw new JabberException("Could not remove roster item.", e);
            }
        }

        #endregion

        private IDictionary<Jid, UserRosterItemDic> LoadRosterItems()
        {
            var items = new SynchronizedDictionary<Jid, UserRosterItemDic>();

            ExecuteList(new SqlQuery("jabber_roster").Select("jid", "item_jid", "name", "subscription", "ask", "groups"))
                .ForEach(r =>
                {
                    var item = new UserRosterItem(new Jid((string)r[1]))
                    {
                        Name = r[2] as string,
                        Subscribtion = (SubscriptionType)Convert.ToInt32(r[3]),
                        Ask = (AskType)Convert.ToInt32(r[4]),
                    };
                    if (r[5] != null) item.Groups.AddRange(((string)r[5]).Split(new[] { GroupSeparator }, StringSplitOptions.RemoveEmptyEntries));

                    var jid = new Jid((string)r[0]);
                    if (!items.ContainsKey(jid)) items[jid] = new UserRosterItemDic();
                    items[jid][item.Jid] = item;
                });

            return items;
        }
    }
}