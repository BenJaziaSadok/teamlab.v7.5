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

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.bind
{
    /// <summary>
    ///   Summary description for Bind.
    /// </summary>
    public class Bind : Element
    {
        // SENT: <iq id="jcl_1" type="set">
        //			<bind xmlns="urn:ietf:params:xml:ns:xmpp-bind"><resource>Exodus</resource></bind>
        //		 </iq>
        // RECV: <iq id='jcl_1' type='result'>
        //			<bind xmlns='urn:ietf:params:xml:ns:xmpp-bind'><jid>user@server.org/agsxmpp</jid></bind>
        //		 </iq>
        public Bind()
        {
            TagName = "bind";
            Namespace = Uri.BIND;
        }

        public Bind(string resource) : this()
        {
            Resource = resource;
        }

        public Bind(Jid jid) : this()
        {
            Jid = jid;
        }

        /// <summary>
        ///   The resource to bind
        /// </summary>
        public string Resource
        {
            get { return GetTag("resource"); }
            set { SetTag("resource", value); }
        }

        /// <summary>
        ///   The jid the server created
        /// </summary>
        public Jid Jid
        {
            get { return GetTagJid("jid"); }
            set { SetTag("jid", value.ToString()); }
        }
    }
}