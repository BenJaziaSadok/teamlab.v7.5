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

namespace ASC.Xmpp.Core.protocol.iq.oob
{
    //	<iq type="set" to="horatio@denmark" from="sailor@sea" id="i_oob_001">
    //		<query xmlns="jabber:iq:oob">
    //			<url>http://denmark/act4/letter-1.html</url>
    //			<desc>There's a letter for you sir.</desc>
    //		</query>
    // </iq>	

    /// <summary>
    ///   Zusammenfassung f�r Oob.
    /// </summary>
    public class Oob : Element
    {
        public Oob()
        {
            TagName = "query";
            Namespace = Uri.IQ_OOB;
        }

        public string Url
        {
            set { SetTag("url", value); }
            get { return GetTag("url"); }
        }

        public string Description
        {
            set { SetTag("desc", value); }
            get { return GetTag("desc"); }
        }
    }
}