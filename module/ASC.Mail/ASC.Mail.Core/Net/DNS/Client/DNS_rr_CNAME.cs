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
    /// CNAME record class.
    /// </summary>
    [Serializable]
    public class DNS_rr_CNAME : DNS_rr_base
    {
        #region Members

        private readonly string m_Alias = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets alias.
        /// </summary>
        public string Alias
        {
            get { return m_Alias; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="alias">Alias.</param>
        /// <param name="ttl">TTL value.</param>
        public DNS_rr_CNAME(string alias, int ttl) : base(QTYPE.CNAME, ttl)
        {
            m_Alias = alias;
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
        public static DNS_rr_CNAME Parse(byte[] reply, ref int offset, int rdLength, int ttl)
        {
            string name = "";
            if (Dns_Client.GetQName(reply, ref offset, ref name))
            {
                return new DNS_rr_CNAME(name, ttl);
            }
            else
            {
                throw new ArgumentException("Invalid CNAME resource record data !");
            }
        }

        #endregion
    }
}