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

namespace ASC.Mail.Net.RTP
{
    /// <summary>
    /// This class holds known RTCP packet types.
    /// </summary>
    public class RTCP_PacketType
    {
        #region Members

        /// <summary>
        /// Application specifiec data.
        /// </summary>
        public const int APP = 204;

        /// <summary>
        /// BYE.
        /// </summary>
        public const int BYE = 203;

        /// <summary>
        /// Receiver report.
        /// </summary>
        public const int RR = 201;

        /// <summary>
        /// Session description.
        /// </summary>
        public const int SDES = 202;

        /// <summary>
        /// Sender report.
        /// </summary>
        public const int SR = 200;

        #endregion
    }
}