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
using System.Data;
using ASC.Common.Data.Sql;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;

using ASC.Xmpp.Core.utils;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Storage.Interface;

namespace ASC.Xmpp.Server.Storage
{
    public class DbPrivateStore : DbStoreBase, IPrivateStore
    {
        protected override SqlCreate[] GetCreateSchemaScript()
        {
            var t1 = new SqlCreate.Table("jabber_private", true)
                .AddColumn("jid", DbType.String, 255, true)
                .AddColumn("tag", DbType.String, 255, true)
                .AddColumn("namespace", DbType.String, 255, true)
                .AddColumn("element", DbType.String, UInt16.MaxValue)
                .PrimaryKey("jid", "tag", "namespace");
            return new[] { t1 };
        }

        
        #region IPrivateStore Members

        public Element GetPrivate(Jid jid, Element element)
        {
            CheckArgs(jid, element);

            var elementStr = ExecuteScalar<string>(new SqlQuery("jabber_private").Select("element").Where("jid", jid.Bare).Where("tag", element.TagName).Where("namespace", element.Namespace));
            return !string.IsNullOrEmpty(elementStr) ? ElementSerializer.DeSerializeElement<Element>(elementStr) : null;
        }

        public void SetPrivate(Jid jid, Element element)
        {
            CheckArgs(jid, element);

            ExecuteNonQuery(
                new SqlInsert("jabber_private", true)
                .InColumnValue("jid", jid.Bare)
                .InColumnValue("tag", element.TagName)
                .InColumnValue("namespace", element.Namespace)
                .InColumnValue("element", ElementSerializer.SerializeElement(element)));
        }

        #endregion

        private void CheckArgs(Jid jid, Element element)
        {
            if (jid == null) throw new ArgumentNullException("jid");
            if (element == null) throw new ArgumentNullException("element");
            if (string.IsNullOrEmpty(element.TagName)) throw new ArgumentNullException("element.TagName");
            if (string.IsNullOrEmpty(element.Namespace)) throw new ArgumentNullException("element.Namespace");
        }
    }
}