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

namespace ASC.Xmpp.Core.protocol.extensions.primary
{
    /// <summary>
    ///   http://www.jabber.org/jeps/inbox/primary.html
    /// </summary>
    public class Primary : Element
    {
        /*
		<presence from='juliet@capulet.com/balcony'>
			<status>I&apos;m back!</status>
			<p xmlns='http://jabber.org/protocol/primary'/>
		</presence>
		*/

        public Primary()
        {
            TagName = "p";
            Namespace = Uri.PRIMARY;
        }
    }
}