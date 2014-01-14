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
        <streamhost 
            jid='proxy.host3' 
            host='24.24.24.1' 
            zeroconf='_jabber.bytestreams'/>
        <xs:element name='streamhost'>
            <xs:complexType>
              <xs:simpleContent>
                <xs:extension base='empty'>
                  <xs:attribute name='jid' type='xs:string' use='required'/>
                  <xs:attribute name='host' type='xs:string' use='required'/>
                  <xs:attribute name='zeroconf' type='xs:string' use='optional'/>
                  <xs:attribute name='port' type='xs:string' use='optional'/>
                </xs:extension>
              </xs:simpleContent>
            </xs:complexType>
        </xs:element>
    */

    public class StreamHost : Element
    {
        public StreamHost()
        {
            TagName = "streamhost";
            Namespace = Uri.BYTESTREAMS;
        }

        public StreamHost(Jid jid, string host) : this()
        {
            Jid = jid;
            Host = host;
        }

        public StreamHost(Jid jid, string host, int port) : this(jid, host)
        {
            Port = port;
        }

        public StreamHost(Jid jid, string host, int port, string zeroconf) : this(jid, host, port)
        {
            Zeroconf = zeroconf;
        }

        /// <summary>
        ///   a port associated with the hostname or IP address for SOCKS5 communications over TCP
        /// </summary>
        public int Port
        {
            get { return GetAttributeInt("port"); }
            set { SetAttribute("port", value); }
        }

        /// <summary>
        ///   the hostname or IP address of the StreamHost for SOCKS5 communications over TCP
        /// </summary>
        public string Host
        {
            get { return GetAttribute("host"); }
            set { SetAttribute("host", value); }
        }

        /// <summary>
        ///   The XMPP/Jabber id of the streamhost
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

        /// <summary>
        ///   a zeroconf [5] identifier to which an entity may connect, for which the service identifier and protocol name SHOULD be "_jabber.bytestreams".
        /// </summary>
        public string Zeroconf
        {
            get { return GetAttribute("zeroconf"); }
            set { SetAttribute("zeroconf", value); }
        }
    }
}