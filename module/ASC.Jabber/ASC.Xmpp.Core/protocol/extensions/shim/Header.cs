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

namespace ASC.Xmpp.Core.protocol.extensions.shim
{
    /// <summary>
    ///   JEP-0131: Stanza Headers and Internet Metadata (SHIM)
    /// </summary>
    public class Header : Element
    {
        #region << Constructors >>

        public Header()
        {
            TagName = "header";
            Namespace = Uri.SHIM;
        }

        public Header(string name, string val) : this()
        {
            Name = name;
            Value = val;
        }

        #endregion

        // <headers xmlns='http://jabber.org/protocol/shim'>
        //	 <header name='In-Reply-To'>123456789@capulet.com</header>
        // <header name='Keywords'>shakespeare,&lt;xmpp/&gt;</header>
        // </headers>

        public string Name
        {
            get { return GetAttribute("name"); }
            set { SetAttribute("name", value); }
        }
    }
}