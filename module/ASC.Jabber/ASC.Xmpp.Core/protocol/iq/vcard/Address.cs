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
    public enum AddressLocation
    {
        NONE = -1,
        HOME,
        WORK
    }

    /// <summary>
    /// </summary>
    public class Address : Element
    {
        //		<!-- Structured address property. Address components with
        //		multiple values must be specified as a comma separated list
        //		of values. -->
        //		<!ELEMENT ADR (
        //		HOME?, 
        //		WORK?, 
        //		POSTAL?, 
        //		PARCEL?, 
        //		(DOM | INTL)?, 
        //		PREF?, 
        //		POBOX?, 
        //		EXTADR?, 
        //		STREET?, 
        //		LOCALITY?, 
        //		REGION?, 
        //		PCODE?, 
        //		CTRY?
        //		)>

        // <ADR>
        //	<WORK/>
        //	<EXTADD>Suite 600</EXTADD>
        //	<STREET>1899 Wynkoop Street</STREET>
        //	<LOCALITY>Denver</LOCALITY>
        //	<REGION>CO</REGION>
        //	<PCODE>80202</PCODE>
        //	<CTRY>USA</CTRY>
        // </ADR>
        public Address()
        {
            TagName = "ADR";
            Namespace = Uri.VCARD;
        }

        public Address(AddressLocation loc, string extra, string street, string locality, string region,
                       string postalcode, string country, bool prefered) : this()
        {
            Location = loc;
            ExtraAddress = extra;
            Street = street;
            Locality = locality;
            Region = region;
            PostalCode = postalcode;
            Country = country;
            IsPrefered = prefered;
        }

        public AddressLocation Location
        {
            get { return (AddressLocation) HasTagEnum(typeof (AddressLocation)); }
            set { SetTag(value.ToString()); }
        }

        public bool IsPrefered
        {
            get { return HasTag("PREF"); }
            set
            {
                if (value)
                    SetTag("PREF");
                else
                    RemoveTag("PREF");
            }
        }

        public string ExtraAddress
        {
            get { return GetTag("EXTADD"); }
            set { SetTag("EXTADD", value); }
        }

        public string Street
        {
            get { return GetTag("STREET"); }
            set { SetTag("STREET", value); }
        }

        public string Locality
        {
            get { return GetTag("LOCALITY"); }
            set { SetTag("LOCALITY", value); }
        }

        public string Region
        {
            get { return GetTag("REGION"); }
            set { SetTag("REGION", value); }
        }

        public string PostalCode
        {
            get { return GetTag("PCODE"); }
            set { SetTag("PCODE", value); }
        }

        public string Country
        {
            get { return GetTag("CTRY"); }
            set { SetTag("CTRY", value); }
        }
    }
}