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

namespace ASC.Xmpp.Core.protocol.iq.vcard
{
    /// <summary>
    /// </summary>
    public class Organization : Element
    {
        #region << Constructors >>

        public Organization()
        {
            TagName = "ORG";
            Namespace = Uri.VCARD;
        }

        public Organization(string name, string unit) : this()
        {
            Name = name;
            Unit = unit;
        }

        #endregion

        // <ORG>
        //	<ORGNAME>Jabber Software Foundation</ORGNAME>
        //	<ORGUNIT/>
        // </ORG>

        public string Name
        {
            get { return GetTag("ORGNAME"); }
            set { SetTag("ORGNAME", value); }
        }

        public string Unit
        {
            get { return GetTag("ORGUNIT"); }
            set { SetTag("ORGUNIT", value); }
        }
    }
}