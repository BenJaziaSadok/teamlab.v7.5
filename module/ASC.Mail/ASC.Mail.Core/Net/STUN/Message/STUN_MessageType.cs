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

namespace ASC.Mail.Net.STUN.Message
{
    /// <summary>
    /// This enum specifies STUN message type.
    /// </summary>
    public enum STUN_MessageType
    {
        /// <summary>
        /// STUN message is binding request.
        /// </summary>
        BindingRequest = 0x0001,

        /// <summary>
        /// STUN message is binding request response.
        /// </summary>
        BindingResponse = 0x0101,

        /// <summary>
        /// STUN message is binding requesr error response.
        /// </summary>
        BindingErrorResponse = 0x0111,

        /// <summary>
        /// STUN message is "shared secret" request.
        /// </summary>
        SharedSecretRequest = 0x0002,

        /// <summary>
        /// STUN message is "shared secret" request response.
        /// </summary>
        SharedSecretResponse = 0x0102,

        /// <summary>
        /// STUN message is "shared secret" request error response.
        /// </summary>
        SharedSecretErrorResponse = 0x0112,
    }
}