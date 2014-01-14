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
    //			<query xmlns='jabber:iq:time'></query>
    //		</iq>
    //
    // Recv:<iq from="jfrankel@coversant.net/SoapBox" id="MX_7" to="gnauck@myjabber.net/Office" type="result">
    //			<query xmlns="jabber:iq:time">
    //				<utc>20050125T00:06:15</utc>
    //				<display>Tuesday, January 25, 2005 12:06:15 AM</display>	
    //				<tz>W. Europe Standard Time</tz>
    //			</query>
    //		</iq> 

    /// <summary>
    ///   Zusammenfassung f�r Time.
    /// </summary>
    public class Time : Element
    {
        public Time()
        {
            TagName = "query";
            Namespace = Uri.IQ_TIME;
        }


        public string Utc
        {
            get { return GetTag("utc"); }
            set { SetTag("utc", value); }
        }

        /// <summary>
        ///   Timezone
        /// </summary>
        //TODO: return System.TimeZone?
        public string Tz
        {
            get { return GetTag("tz"); }
            set { SetTag("tz", value); }
        }

        /// <summary>
        ///   Human-readable date/time.
        /// </summary>
        public string Display
        {
            get { return GetTag("display"); }
            set { SetTag("display", value); }
        }
    }
}