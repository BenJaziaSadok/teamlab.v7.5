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
    #region usings

    using System;

    #endregion

    /// <summary>
    /// This class provides data for RTP packet related events/methods.
    /// </summary>
    public class RTP_PacketEventArgs : EventArgs
    {
        #region Members

        private readonly RTP_Packet m_pPacket;

        #endregion

        #region Properties

        /// <summary>
        /// Gets RTP packet.
        /// </summary>
        public RTP_Packet Packet
        {
            get { return m_pPacket; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="packet">RTP packet.</param>
        public RTP_PacketEventArgs(RTP_Packet packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException("packet");
            }

            m_pPacket = packet;
        }

        #endregion
    }
}