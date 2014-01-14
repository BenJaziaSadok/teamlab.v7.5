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

namespace ASC.Xmpp.Core.protocol.iq.version
{
    // Send:<iq type='get' id='MX_6' to='jfrankel@coversant.net/SoapBox'>
    //			<query xmlns='jabber:iq:version'></query>
    //		</iq>
    //
    // Recv:<iq from="jfrankel@coversant.net/SoapBox" id="MX_6" to="gnauck@myjabber.net/Office" type="result">
    //			<query xmlns="jabber:iq:version">
    //				<name>SoapBox</name>
    //				<version>2.1.2 beta</version>
    //				<os>Windows NT 5.1 (en-us)</os>
    //			</query>
    //		</iq> 


    /// <summary>
    ///   Zusammenfassung f�r Version.
    /// </summary>
    public class Version : Element
    {
        public Version()
        {
            TagName = "query";
            Namespace = Uri.IQ_VERSION;
        }

        public string Name
        {
            set { SetTag("name", value); }
            get { return GetTag("name"); }
        }

        public string Ver
        {
            set { SetTag("version", value); }
            get { return GetTag("version"); }
        }

        public string Os
        {
            set { SetTag("os", value); }
            get { return GetTag("os"); }
        }
    }
}