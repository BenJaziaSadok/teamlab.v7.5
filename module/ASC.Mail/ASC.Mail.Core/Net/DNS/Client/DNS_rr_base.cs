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
    /// Base class for DNS records.
    /// </summary>
    public abstract class DNS_rr_base
    {
        #region Members

        private readonly int m_TTL = -1;
        private readonly QTYPE m_Type = QTYPE.A;

        #endregion

        #region Properties

        /// <summary>
        /// Gets record type (A,MX,...).
        /// </summary>
        public QTYPE RecordType
        {
            get { return m_Type; }
        }

        /// <summary>
        /// Gets TTL (time to live) value in seconds.
        /// </summary>
        public int TTL
        {
            get { return m_TTL; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="recordType">Record type (A,MX, ...).</param>
        /// <param name="ttl">TTL (time to live) value in seconds.</param>
        public DNS_rr_base(QTYPE recordType, int ttl)
        {
            m_Type = recordType;
            m_TTL = ttl;
        }

        #endregion
    }
}