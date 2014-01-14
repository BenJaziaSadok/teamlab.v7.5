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

namespace ASC.Xmpp.Core.protocol.iq.time
{
    // Send:<iq type='get' id='MX_7' to='jfrankel@coversant.net/SoapBox'>
    //			<time xmlns='urn:jabber:time'/>
    //		</iq>
    //
    // Recv:<iq from="jfrankel@coversant.net/SoapBox" id="MX_7" to="gnauck@myjabber.net/Office" type="result">
    //			<time xmlns='urn:jabber:time'>
    //				<tzo>-06:00</tzo>
    //				<utc>2006-12-19T17:58:35Z</utc>
    //			</time>
    //		</iq>

    public class EntityTime : Element
    {
        public EntityTime()
        {
            TagName = "time";
            Namespace = Uri.ENTITY_TIME;
        }

        /// <summary>
        ///   The entity's numeric time zone offset from UTC. The format MUST conform to the Time Zone Definition (TZD) specified in XEP-0082.
        /// </summary>
        public string Tzo
        {
            get { return GetTag("tzo"); }
            set { SetTag("tzo", value); }
        }

        /// <summary>
        ///   The UTC time according to the responding entity. The format MUST conform to the dateTime profile specified in XEP-0082 and MUST be expressed in UTC.
        /// </summary>
        public string Utc
        {
            get { return GetTag("utc"); }
            set { SetTag("utc", value); }
        }
    }
}