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

namespace ASC.Xmpp.Core.protocol.extensions.jivesoftware.phone
{
    /// <summary>
    ///   Events are sent to the user when their phone is ringing, when a call ends, etc. As with presence, pubsub should probably be the mechanism used for sending this information, but message packets are used to send events for the time being
    /// </summary>
    public enum PhoneStatusType
    {
        RING,
        DIALED,
        ON_PHONE,
        HANG_UP
    }
}