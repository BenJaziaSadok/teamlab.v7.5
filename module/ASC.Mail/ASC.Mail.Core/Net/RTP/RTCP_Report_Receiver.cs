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
    /// This class holds receiver report info.
    /// </summary>
    public class RTCP_Report_Receiver
    {
        #region Members

        private readonly uint m_CumulativePacketsLost;
        private readonly uint m_DelaySinceLastSR;
        private readonly uint m_ExtHigestSeqNumber;
        private readonly uint m_FractionLost;
        private readonly uint m_Jitter;
        private readonly uint m_LastSR;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the fraction of RTP data packets from source SSRC lost since the previous SR or 
        /// RR packet was sent.
        /// </summary>
        public uint FractionLost
        {
            get { return m_FractionLost; }
        }

        /// <summary>
        /// Gets total number of RTP data packets from source SSRC that have
        /// been lost since the beginning of reception.
        /// </summary>
        public uint CumulativePacketsLost
        {
            get { return m_CumulativePacketsLost; }
        }

        /// <summary>
        /// Gets extended highest sequence number received.
        /// </summary>
        public uint ExtendedSequenceNumber
        {
            get { return m_ExtHigestSeqNumber; }
        }

        /// <summary>
        /// Gets an estimate of the statistical variance of the RTP data packet
        /// interarrival time, measured in timestamp units and expressed as an
        /// unsigned integer.
        /// </summary>
        public uint Jitter
        {
            get { return m_Jitter; }
        }

        /// <summary>
        /// Gets when last sender report(SR) was recieved.
        /// </summary>
        public uint LastSR
        {
            get { return m_LastSR; }
        }

        /// <summary>
        /// Gets delay since last sender report(SR) was received.
        /// </summary>
        public uint DelaySinceLastSR
        {
            get { return m_DelaySinceLastSR; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rr">RTCP RR report.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>rr</b> is null reference.</exception>
        internal RTCP_Report_Receiver(RTCP_Packet_ReportBlock rr)
        {
            if (rr == null)
            {
                throw new ArgumentNullException("rr");
            }

            m_FractionLost = rr.FractionLost;
            m_CumulativePacketsLost = rr.CumulativePacketsLost;
            m_ExtHigestSeqNumber = rr.ExtendedHighestSeqNo;
            m_Jitter = rr.Jitter;
            m_LastSR = rr.LastSR;
            m_DelaySinceLastSR = rr.DelaySinceLastSR;
        }

        #endregion
    }
}