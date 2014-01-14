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

using ASC.Xmpp.Core.protocol.Base;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.browse
{
    /// <summary>
    ///   Summary description for BrowseItem.
    /// </summary>
    public class BrowseItem : Item
    {
        /*
		<item version="0.6.0" name="Public Conferencing" jid="conference.myjabber.net" type="public" category="conference"> 
			<ns>http://jabber.org/protocol/muc</ns> 
		</item>
		*/

        public BrowseItem()
        {
            Namespace = Uri.IQ_BROWSE;
        }

        public string Category
        {
            get { return GetAttribute("category"); }
            set { SetAttribute("category", value); }
        }

        public string Version
        {
            get { return GetAttribute("version"); }
            set { SetAttribute("version", value); }
        }

        public string Type
        {
            get { return GetAttribute("type"); }
            set { SetAttribute("type", value); }
        }

        /// <summary>
        ///   Gets all advertised namespaces of this item
        /// </summary>
        /// <returns> string array that contains the advertised namespaces </returns>
        public string[] GetNamespaces()
        {
            ElementList elements = SelectElements("ns");
            var nss = new string[elements.Count];

            int i = 0;
            foreach (Element ns in elements)
            {
                nss[i] = ns.Value;
                i++;
            }

            return nss;
        }
    }
}