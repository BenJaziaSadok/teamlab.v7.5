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
    public class Name : Element
    {
        #region << Constructors >>

        public Name()
        {
            TagName = "N";
            Namespace = Uri.VCARD;
        }

        public Name(string family, string given, string middle) : this()
        {
            Family = family;
            Given = given;
            Middle = middle;
        }

        #endregion

        // <N>
        //	<FAMILY>Saint-Andre<FAMILY>
        //	<GIVEN>Peter</GIVEN>
        //	<MIDDLE/>
        // </N>

        public string Family
        {
            get { return GetTag("FAMILY"); }
            set { SetTag("FAMILY", value); }
        }

        public string Given
        {
            get { return GetTag("GIVEN"); }
            set { SetTag("GIVEN", value); }
        }

        public string Middle
        {
            get { return GetTag("MIDDLE"); }
            set { SetTag("MIDDLE", value); }
        }
    }
}