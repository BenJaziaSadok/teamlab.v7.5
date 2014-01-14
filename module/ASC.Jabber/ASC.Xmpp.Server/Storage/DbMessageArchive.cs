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
using ASC.Common.Data.Sql.Expressions;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;

using ASC.Xmpp.Core.utils;

namespace ASC.Xmpp.Server.Storage
{
    public class DbMessageArchive : DbStoreBase
    {
        private IDictionary<string, bool> loggingCache = new SynchronizedDictionary<string, bool>();

        protected override SqlCreate[] GetCreateSchemaScript()
        {
            var t1 = new SqlCreate.Table("jabber_archive", true)
                .AddColumn(new SqlCreate.Column("id", DbType.Int32).NotNull(true).PrimaryKey(true).Autoincrement(true))
                .AddColumn("jid", DbType.String, 255, true)
                .AddColumn("stamp", DbType.DateTime, true)
                .AddColumn("message", DbType.String, MESSAGE_COLUMN_LEN)
                .AddIndex("jabber_archive_jid", "jid");

            var t2 = new SqlCreate.Table("jabber_archive_switch", true)
                .AddColumn(new SqlCreate.Column("id", DbType.String, 255).NotNull(true).PrimaryKey(true));

            return new[] { t1, t2 };
        }


        public void SetMessageLogging(Jid from, Jid to, bool logging)
        {
            if (from == null) throw new ArgumentNullException("from");
            if (to == null) throw new ArgumentNullException("to");

            var key = GetKey(from, to);
            if (logging)
            {
                ExecuteNonQuery(new SqlDelete("jabber_archive_switch").Where("id", key));
                loggingCache.Remove(key);
            }
            else
            {
                ExecuteNonQuery(new SqlInsert("jabber_archive_switch", true).InColumnValue("id", key));
                loggingCache[key] = false;
            }
        }

        public bool GetMessageLogging(Jid from, Jid to)
        {
            if (from == null) throw new ArgumentNullException("from");
            if (to == null) throw new ArgumentNullException("to");

            return !loggingCache.ContainsKey(GetKey(from, to));
        }


        public void SaveMessages(params Message[] messages)
        {
            if (messages == null) throw new ArgumentNullException("message");
            if (messages.Length == 0) return;

            var batch = new List<ISqlInstruction>(messages.Length);
            foreach (var m in messages)
            {
                if (string.IsNullOrEmpty(m.Body) && string.IsNullOrEmpty(m.Subject) && string.IsNullOrEmpty(m.Thread) && m.Html == null)
                {
                    continue;
                }

                batch.Add(new SqlInsert("jabber_archive")
                    .InColumnValue("jid", GetKey(m.From, m.To))
                    .InColumnValue("stamp", DateTime.UtcNow)
                    .InColumnValue("message", ElementSerializer.SerializeElement(m)));
            }
            ExecuteBatch(batch);
        }

        public Message[] GetMessages(Jid from, Jid to, DateTime start, DateTime end, int count)
        {
            if (from == null) throw new ArgumentNullException("from");
            if (to == null) throw new ArgumentNullException("to");

            var q = new SqlQuery("jabber_archive")
                .Select("message")
                .Where("jid", GetKey(from, to))
                .Where(Exp.Between("stamp", start, end))
                .OrderBy("id", false);
            if (0 < count && count < int.MaxValue) q.SetMaxResults(count);

            var messages = ExecuteList(q).ConvertAll(r => ElementSerializer.DeSerializeElement<Message>((string)r[0]));
            messages.Reverse();
            return messages.ToArray();
        }

        public void RemoveMessages(Jid from, Jid to)
        {
            ExecuteNonQuery(new SqlDelete("jabber_archive").Where("jid", GetKey(from, to)));
        }


        private string GetKey(Jid from, Jid to)
        {
            return string.Compare(from.Bare, to.Bare) < 0 ? string.Format("{0}|{1}", from.Bare, to.Bare) : string.Format("{1}|{0}", from.Bare, to.Bare);
        }
    }
}