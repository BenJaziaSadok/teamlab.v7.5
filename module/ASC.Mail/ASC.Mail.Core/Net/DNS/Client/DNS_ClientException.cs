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
    /// DNS client exception.
    /// </summary>
    public class DNS_ClientException : Exception
    {
        #region Members

        private readonly RCODE m_RCode;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rcode">DNS server returned error code.</param>
        public DNS_ClientException(RCODE rcode)
        {
            m_RCode = rcode;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets DNS server returned error code.
        /// </summary>
        public RCODE ErrorCode
        {
            get { return m_RCode; }
        }

        #endregion
    }
}