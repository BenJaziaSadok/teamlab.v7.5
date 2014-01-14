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
    /// <summary>
    /// HINFO record.
    /// </summary>
    public class DNS_rr_HINFO : DNS_rr_base
    {
        #region Members

        private readonly string m_CPU = "";
        private readonly string m_OS = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets host's CPU.
        /// </summary>
        public string CPU
        {
            get { return m_CPU; }
        }

        /// <summary>
        /// Gets host's OS.
        /// </summary>
        public string OS
        {
            get { return m_OS; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cpu">Host CPU.</param>
        /// <param name="os">Host OS.</param>
        /// <param name="ttl">TTL value.</param>
        public DNS_rr_HINFO(string cpu, string os, int ttl) : base(QTYPE.HINFO, ttl)
        {
            m_CPU = cpu;
            m_OS = os;
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
        public static DNS_rr_HINFO Parse(byte[] reply, ref int offset, int rdLength, int ttl)
        {
            /* RFC 1035 3.3.2. HINFO RDATA format

			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			/                      CPU                      /
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			/                       OS                      /
			+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
			
			CPU     A <character-string> which specifies the CPU type.

			OS      A <character-string> which specifies the operating
					system type.
					
					Standard values for CPU and OS can be found in [RFC-1010].

			*/

            // CPU
            string cpu = Dns_Client.ReadCharacterString(reply, ref offset);

            // OS
            string os = Dns_Client.ReadCharacterString(reply, ref offset);

            return new DNS_rr_HINFO(cpu, os, ttl);
        }

        #endregion
    }
}