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

namespace ASC.Xmpp.Core.protocol.extensions.multicast
{
    public class Addresses : Element
    {
        public Addresses()
        {
            TagName = "addresses";
            Namespace = Uri.ADDRESS;
        }

        public Address AddAddress(Address address)
        {
            AddChild(address);
            return address;
        }

        public Jid[] GetAddressList()
        {
            ElementList nl = SelectElements("address");
            var addresses = new Jid[nl.Count];

            int i = 0;
            foreach (Element e in nl)
            {
                addresses[i] = ((Address) e).Jid;
                i++;
            }
            return addresses;
        }

        public void RemoveAllBcc()
        {
            foreach (Address address in GetAddresses())
            {
                if (address.Type == AddressType.bcc)
                {
                    address.Remove();
                }
            }
        }

        public Address[] GetAddresses()
        {
            ElementList nl = SelectElements("address");
            var addresses = new Address[nl.Count];

            int i = 0;
            foreach (Element e in nl)
            {
                addresses[i] = (Address) e;
                i++;
            }
            return addresses;
        }
    }
}