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
    //	<!-- Email address property. Default type is INTERNET. -->
    //	<!ELEMENT EMAIL (
    //	HOME?, 
    //	WORK?, 
    //	INTERNET?, 
    //	PREF?, 
    //	X400?, 
    //	USERID
    //	)>
    public enum EmailType
    {
        NONE = -1,
        HOME,
        WORK,
        INTERNET,
        X400,
    }

    /// <summary>
    /// </summary>
    public class Email : Element
    {
        #region << Constructors >>

        public Email()
        {
            TagName = "EMAIL";
            Namespace = Uri.VCARD;
        }

        /// <summary>
        /// </summary>
        /// <param name="type"> Type of the new Email Adress </param>
        /// <param name="address"> Email Adress </param>
        /// <param name="prefered"> Is this adressed prefered </param>
        public Email(EmailType type, string userid, bool prefered) : this()
        {
            Type = type;
            IsPrefered = prefered;
            UserId = userid;
        }

        #endregion

        // <EMAIL><INTERNET/><PREF/><USERID>stpeter@jabber.org</USERID></EMAIL>

        public EmailType Type
        {
            get { return (EmailType) HasTagEnum(typeof (EmailType)); }
            set
            {
                if (value != EmailType.NONE)
                    SetTag(value.ToString());
            }
        }

        /// <summary>
        ///   Is this the prefered contact adress?
        /// </summary>
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

        /// <summary>
        ///   The email Adress
        /// </summary>
        public string UserId
        {
            get { return GetTag("USERID"); }
            set { SetTag("USERID", value); }
        }
    }
}