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

namespace ASC.Xmpp.Core.protocol.client
{
    /// <summary>
    ///   Enumeration that represents the type of a message
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        ///   This in a normal message, much like an email. You dont expect a fast
        /// </summary>
        normal = -1,

        /// <summary>
        ///   a error messages
        /// </summary>
        error,

        /// <summary>
        ///   is for chat like messages, person to person. Send this if you expect a fast reply. reply or no reply at all.
        /// </summary>
        chat,

        /// <summary>
        ///   is used for sending/receiving messages from/to a chatroom (IRC style chats)
        /// </summary>
        groupchat,

        /// <summary>
        ///   Think of this as a news broadcast, or RRS Feed, the message will normally have a URL and Description Associated with it.
        /// </summary>
        headline
    }
}