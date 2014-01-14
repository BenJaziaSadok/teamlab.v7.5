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

namespace ASC.Xmpp.Core.protocol.extensions.bytestreams
{
    /*
        <iq type='result' 
            from='target@host2/bar' 
            to='initiator@host1/foo' 
            id='initiate'>
          <query xmlns='http://jabber.org/protocol/bytestreams'>
            <streamhost-used jid='proxy.host3'/>
          </query>
        </iq>
    */

    /// <summary>
    ///   The <streamhost-used /> element indicates the StreamHost connected to. This element has a single attribute for the JID of the StreamHost to which the Target connected. This element MUST NOT contain any content node. The "jid" attribute specifies the full JID of the StreamHost. This attribute MUST be present, and MUST be a valid JID for use with an &lt;iq/&gt;.
    /// </summary>
    public class StreamHostUsed : Element
    {
        public StreamHostUsed()
        {
            TagName = "streamhost-used";
            Namespace = Uri.BYTESTREAMS;
        }

        public StreamHostUsed(Jid jid) : this()
        {
            Jid = jid;
        }

        /// <summary>
        ///   Jid of the streamhost
        /// </summary>
        public Jid Jid
        {
            get
            {
                if (HasAttribute("jid"))
                    return new Jid(GetAttribute("jid"));
                else
                    return null;
            }
            set
            {
                if (value != null)
                    SetAttribute("jid", value.ToString());
                else
                    RemoveAttribute("jid");
            }
        }
    }
}