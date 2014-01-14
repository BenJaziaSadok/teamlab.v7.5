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

    #endregion

    /// <summary>
    /// NS record class.
    /// </summary>
    [Serializable]
    public class DNS_rr_NS : DNS_rr_base
    {
        #region Members

        private readonly string m_NameServer = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets name server name.
        /// </summary>
        public string NameServer
        {
            get { return m_NameServer; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="nameServer">Name server name.</param>
        /// <param name="ttl">TTL value.</param>
        public DNS_rr_NS(string nameServer, int ttl) : base(QTYPE.NS, ttl)
        {
            m_NameServer = nameServer;
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
        public static DNS_rr_NS Parse(byte[] reply, ref int offset, int rdLength, int ttl)
        {
            // Name server name

            string name = "";
            if (Dns_Client.GetQName(reply, ref offset, ref name))
            {
                return new DNS_rr_NS(name, ttl);
            }
            else
            {
                throw new ArgumentException("Invalid NS resource record data !");
            }
        }

        #endregion
    }
}