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

namespace ASC.Xmpp.Core.protocol.iq.roster
{
    /// <summary>
    ///   Extension JEP-0083, delimiter for nested roster groups
    /// </summary>
    public class Delimiter : Element
    {
        /*
		3.1 Querying for the delimiter 
		All compliant clients SHOULD query for an existing delimiter at login.

		Example 1. Querying for the Delimiter
			
		CLIENT:																												 CLIENT:
		<iq type='get'
			 id='1'>
		<query xmlns='jabber:iq:private'>
			 <roster xmlns='roster:delimiter'/>
				  </query>
		</iq>

		SERVER:
		<iq type='result'
			 id='1'
		from='bill@shakespeare.lit/Globe'
		to='bill@shakespeare.lit/Globe'>
		<query xmlns='jabber:iq:private'>
			 <roster xmlns='roster:delimiter'>::</roster>
		</query>
		</iq>
		*/

        public Delimiter()
        {
            TagName = "roster";
            Namespace = Uri.ROSTER_DELIMITER;
        }

        public Delimiter(string delimiter) : this()
        {
            Value = delimiter;
        }
    }
}