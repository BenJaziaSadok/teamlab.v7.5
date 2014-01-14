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

namespace ASC.Xmpp.Core.protocol.extensions.html
{
    /// <summary>
    ///   The Body Element of a XHTML message
    /// </summary>
    public class Body : Element
    {
        public Body()
        {
            TagName = "body";
            Namespace = Uri.XHTML;
        }

        /// <summary>
        /// </summary>
        public string InnerHtml
        {
            get
            {
                // Thats a HACK
                string xml = ToString();

                int start = xml.IndexOf(">");
                int end = xml.LastIndexOf("</" + TagName + ">");

                return xml.Substring(start + 1, end - start - 1);
            }
        }
    }
}