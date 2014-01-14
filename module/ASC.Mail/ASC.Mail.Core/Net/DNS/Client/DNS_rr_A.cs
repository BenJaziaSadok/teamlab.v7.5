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

namespace ASC.Mail.Net.Dns.Client
{
    #region usings

    using System;
    using System.Net;

    #endregion

    /// <summary>
    /// A record class.
    /// </summary>
    [Serializable]
    public class DNS_rr_A : DNS_rr_base
    {
        #region Members

        private readonly IPAddress m_IP;

        #endregion

        #region Properties

        /// <summary>
        /// Gets host IP address.
        /// </summary>
        public IPAddress IP
        {
            get { return m_IP; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="ip">IP address.</param>
        /// <param name="ttl">TTL value.</param>
        public DNS_rr_A(IPAddress ip, int ttl) : base(QTYPE.A, ttl)
        {
            m_IP = ip;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses resource record from reply data.
        /// </summary>
        /// <param name="reply">DNS server reply data.</param>
        /// <param name="offset">Current offset in reply data.</param>
        /// <param name="rdLength">Resource record data length.</param>
        /// <param name="ttl">Time to live in seconds.</param>
        public static DNS_rr_A Parse(byte[] reply, ref int offset, int rdLength, int ttl)
        {
            // IPv4 = byte byte byte byte

            byte[] ip = new byte[rdLength];
            Array.Copy(reply, offset, ip, 0, rdLength);
            offset += rdLength;

            return new DNS_rr_A(new IPAddress(ip), ttl);
        }

        #endregion
    }
}