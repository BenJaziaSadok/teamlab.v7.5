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

namespace ASC.Xmpp.Core.protocol.extensions.jivesoftware.phone
{
    public class PhoneAction : Element
    {
        #region << Constructors >>

        /// <summary>
        /// </summary>
        public PhoneAction()
        {
            TagName = "phone-action";
            Namespace = Uri.JIVESOFTWARE_PHONE;
        }

        public PhoneAction(ActionType type) : this()
        {
            Type = type;
        }

        public PhoneAction(ActionType type, string extension) : this(type)
        {
            Extension = extension;
        }

        public PhoneAction(ActionType type, Jid jid) : this(type)
        {
            Jid = jid;
        }

        #endregion

        /*
         * Actions are sent by the client to perform tasks such as dialing, checking for messages, etc. Actions are sent as IQ's (type set), as with the following child stanza:
         * 
         * <phone-action xmlns="http://jivesoftware.com/xmlns/phone" type="DIAL">
         *    <extension>5035555555</extension>
         * </phone-action>
         *          
         * Currently supported types are DIAL and FORWARD.
         * In most implementations, issuing a dial command will cause the user's phone to ring.
         * Once the user picks up, the specified extension will be dialed.
         * 
         * Dialing can also be performed by jid too. The jid must be dialed must be mapped on the server to an extension
         * 
         * <phone-action type="DIAL">
         *  <jid>andrew@jivesoftware.com</jid>
         * </phone-action>
         * 
         * Issuing a action wth a type FORWARD should transfer a call that has already been 
         * established to a third party. The FORWARD type requires an extension or jid child element
         *
         *  <phone-action xmlns="http://jivesoftware.com/xmlns/phone" type="FORWARD">
         *      <extension>5035555555</extension>
         *  </phone-action>
         *
         */

        public ActionType Type
        {
            set { SetAttribute("type", value.ToString()); }
            get { return (ActionType) GetAttributeEnum("type", typeof (ActionType)); }
        }

        public string Extension
        {
            get { return GetTag("extension"); }
            set { SetTag("extension", value); }
        }

        public Jid Jid
        {
            get { return new Jid(GetTag("jid")); }
            set { SetTag("jid", value.ToString()); }
        }
    }
}