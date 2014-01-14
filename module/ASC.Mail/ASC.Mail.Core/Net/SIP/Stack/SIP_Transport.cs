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

namespace ASC.Mail.Net.SIP.Stack
{
    /// <summary>
    /// This class holds SIP transports. Defined in RFC 3261.
    /// </summary>
    public class SIP_Transport
    {
        #region Constants

        /// <summary>
        /// TCP protocol.
        /// </summary>
        public const string TCP = "TCP";

        /// <summary>
        /// TCP + SSL protocol.
        /// </summary>
        public const string TLS = "TLS";

        /// <summary>
        /// UDP protocol.
        /// </summary>
        public const string UDP = "UDP";

        #endregion
    }
}