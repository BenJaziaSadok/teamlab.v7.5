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
    public enum TelephoneLocation
    {
        NONE = -1,
        HOME,
        WORK
    }

    public enum TelephoneType
    {
        NONE = -1,
        VOICE,
        FAX,
        PAGER,
        MSG,
        CELL,
        VIDEO,
        BBS,
        MODEM,
        ISDN,
        PCS,
        PREF,
        NUMBER
    }

    /// <summary>
    ///   Zusammenfassung f�r Telephone.
    /// </summary>
    public class Telephone : Element
    {
        #region << Constructors >>

        public Telephone()
        {
            TagName = "TEL";
            Namespace = Uri.VCARD;
        }

        public Telephone(TelephoneLocation loc, TelephoneType type, string number) : this()
        {
            if (loc != TelephoneLocation.NONE)
                Location = loc;

            if (type != TelephoneType.NONE)
                Type = type;

            Number = number;
        }

        #endregion

        //	<TEL><VOICE/><WORK/><NUMBER>303-308-3282</NUMBER></TEL>
        //	<TEL><FAX/><WORK/><NUMBER/></TEL>
        //	<TEL><MSG/><WORK/><NUMBER/></TEL>

        public string Number
        {
            get { return GetTag("NUMBER"); }
            set { SetTag("NUMBER", value); }
        }

        public TelephoneLocation Location
        {
            get { return (TelephoneLocation) HasTagEnum(typeof (TelephoneLocation)); }
            set { SetTag(value.ToString()); }
        }

        public TelephoneType Type
        {
            get { return (TelephoneType) HasTagEnum(typeof (TelephoneType)); }
            set { SetTag(value.ToString()); }
        }
    }
}