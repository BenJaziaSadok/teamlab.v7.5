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
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Server.Storage.Interface;
using ASC.Xmpp.Server.Users;
using ASC.Collections;

namespace ASC.Xmpp.Server.Storage
{
    public class DbUserStore : DbStoreBase, IUserStore
    {
        private object syncRoot = new object();

        private IDictionary<string, User> users;

        private IDictionary<string, User> Users
        {
            get
            {
                if (users == null)
                {
                    lock (syncRoot)
                    {
                        if (users == null) users = LoadFromDb();
                    }
                }
                return users;
            }
        }


        protected override SqlCreate[] GetCreateSchemaScript()
        {
            var t1 = new SqlCreate.Table("jabber_user", true)
                .AddColumn("jid", DbType.String, 255, true)
                .AddColumn("pwd", DbType.String, 255)
                .AddColumn(new SqlCreate.Column("admin", DbType.Int32).NotNull(true).Default(0))
                .PrimaryKey("jid");
            return new[] { t1 };
        }


        #region IUserStore Members

        public ICollection<User> GetUsers(string domain)
        {
            lock (syncRoot)
            {
                return Users.Values.Where(u => string.Compare(u.Jid.Server, domain, true) == 0).ToList();
            }
        }

        public User GetUser(Jid jid)
        {
            var bareJid = GetBareJid(jid);
            lock (syncRoot)
            {
                return Users.ContainsKey(bareJid) ? Users[bareJid] : null;
            }
        }

        public void SaveUser(User user)
        {
            if (user == null) throw new ArgumentNullException("user");
            lock (syncRoot)
            {
                var bareJid = GetBareJid(user.Jid);
                ExecuteNonQuery(new SqlInsert("jabber_user", true)
                    .InColumnValue("jid", bareJid)
                    .InColumnValue("pwd", user.Password)
                    .InColumnValue("admin", user.IsAdmin));
                Users[bareJid] = user;
            }
        }

        public void RemoveUser(Jid jid)
        {
            var bareJid = GetBareJid(jid);
            lock (syncRoot)
            {
                ExecuteNonQuery(new SqlDelete("jabber_user").Where("jid", bareJid));
                Users.Remove(bareJid);
            }
        }

        #endregion

        private IDictionary<string, User> LoadFromDb()
        {
            var d = ExecuteList(new SqlQuery("jabber_user").Select("jid", "pwd", "admin"))
                .ConvertAll(r => new User(new Jid((string)r[0]), (string)r[1], Convert.ToBoolean(r[2])))
                .ToDictionary(u => u.Jid.ToString());
            return new SynchronizedDictionary<string, User>(d);
        }

        private string GetBareJid(Jid jid)
        {
            if (jid == null) throw new ArgumentNullException("jid");
            return jid.Bare.ToLowerInvariant();
        }
    }
}