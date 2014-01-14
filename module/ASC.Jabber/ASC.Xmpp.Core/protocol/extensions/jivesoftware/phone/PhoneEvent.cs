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
    /* 
     *
     *
     * <message from="x" id="1137178511.247" to="y">
     *      <phone-event xmlns="http://jivesoftware.com/xmlns/phone" callID="1137178511.247" type="ON_PHONE" device="SIP/3001">
     *          <callerID></callerID>
     *      </phone-event>
     * </message>
     * 
     */

    /// <summary>
    ///   Events are sent to the user when their phone is ringing, when a call ends, etc. This packet is send within a message packet (subelement of message)
    /// </summary>
    public class PhoneEvent : Element
    {
        #region << Constructors >>

        public PhoneEvent()
        {
            TagName = "phone-event";
            Namespace = Uri.JIVESOFTWARE_PHONE;
        }

        public PhoneEvent(PhoneStatusType status) : this()
        {
            Type = status;
        }

        public PhoneEvent(PhoneStatusType status, string device) : this(status)
        {
            Device = device;
        }

        public PhoneEvent(PhoneStatusType status, string device, string id) : this(status, device)
        {
            CallId = id;
        }

        public PhoneEvent(PhoneStatusType status, string device, string id, string callerId) : this(status, device, id)
        {
            CallerId = callerId;
        }

        #endregion

        public string CallId
        {
            get { return GetAttribute("callID"); }
            set { SetAttribute("callID", value); }
        }

        public string Device
        {
            get { return GetAttribute("device"); }
            set { SetAttribute("device", value); }
        }

        public PhoneStatusType Type
        {
            set { SetAttribute("type", value.ToString()); }
            get { return (PhoneStatusType) GetAttributeEnum("type", typeof (PhoneStatusType)); }
        }

        public string CallerId
        {
            get { return GetTag("callerID"); }
            set { SetTag("callerID", value); }
        }

        public string CallerIdName
        {
            get { return GetTag("callerIDName"); }
            set { SetTag("callerIDName", value); }
        }
    }
}