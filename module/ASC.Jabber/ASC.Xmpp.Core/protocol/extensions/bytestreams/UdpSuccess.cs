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
        <message 
            from='proxy.host3' 
            to='target@host2/bar' 
            id='initiate'>
            <udpsuccess xmlns='http://jabber.org/protocol/bytestreams' dstaddr='Value of Hash'/>
        </message>
    */

    public class UdpSuccess : Element
    {
        public UdpSuccess(string dstaddr)
        {
            TagName = "udpsuccess";
            Namespace = Uri.BYTESTREAMS;

            DestinationAddress = dstaddr;
        }

        public string DestinationAddress
        {
            get { return GetAttribute("dstaddr"); }
            set { SetAttribute("dstaddr", value); }
        }
    }
}