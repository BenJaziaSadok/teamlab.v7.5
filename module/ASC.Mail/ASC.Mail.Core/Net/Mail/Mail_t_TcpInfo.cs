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

namespace ASC.Mail.Net.Mail
{
    #region usings

    using System;
    using System.Net;

    #endregion

    /// <summary>
    /// Represents Received: header "TCP-info" value. Defined in RFC 5321. 4.4.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 5321 4.4.
    ///     TCP-info        = address-literal / ( Domain FWS address-literal )
    ///     address-literal = "[" ( IPv4-address-literal / IPv6-address-literal / General-address-literal ) "]"
    /// </code>
    /// </remarks>
    public class Mail_t_TcpInfo
    {
        #region Members

        private readonly string m_HostName;
        private readonly IPAddress m_pIP;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="ip">IP address.</param>
        /// <param name="hostName">Host name.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>ip</b> is null reference.</exception>
        public Mail_t_TcpInfo(IPAddress ip, string hostName)
        {
            if (ip == null)
            {
                throw new ArgumentNullException("ip");
            }

            m_pIP = ip;
            m_HostName = hostName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets host value. Value null means not specified.
        /// </summary>
        public string HostName
        {
            get { return m_HostName; }
        }

        /// <summary>
        /// Gets IP address.
        /// </summary>
        public IPAddress IP
        {
            get { return m_pIP; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(m_HostName))
            {
                return "[" + m_pIP + "]";
            }
            else
            {
                return m_HostName + " [" + m_pIP + "]";
            }
        }

        #endregion
    }
}