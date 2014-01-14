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

namespace ASC.Mail.Net.SIP.Proxy
{
    #region usings

    using System;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// This class provides data for SIP_ProxyCore.GetGateways event.
    /// </summary>
    public class SIP_GatewayEventArgs
    {
        #region Members

        private readonly List<SIP_Gateway> m_pGateways;
        private readonly string m_UriScheme = "";
        private readonly string m_UserName = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets URI scheme which gateways to get.
        /// </summary>
        public string UriScheme
        {
            get { return m_UriScheme; }
        }

        /// <summary>
        /// Gets authenticated user name.
        /// </summary>        
        public string UserName
        {
            get { return m_UserName; }
        }

        /*
        /// <summary>
        /// Gets or sets if specified user has 
        /// </summary>
        public bool IsForbidden
        {
            get{ return false; }

            set{ }
        }*/

        /// <summary>
        /// Gets gateways collection.
        /// </summary>
        public List<SIP_Gateway> Gateways
        {
            get { return m_pGateways; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="uriScheme">URI scheme which gateways to get.</param>
        /// <param name="userName">Authenticated user name.</param>
        /// <exception cref="ArgumentException">If any argument has invalid value.</exception>
        public SIP_GatewayEventArgs(string uriScheme, string userName)
        {
            if (string.IsNullOrEmpty(uriScheme))
            {
                throw new ArgumentException("Argument 'uriScheme' value can't be null or empty !");
            }

            m_UriScheme = uriScheme;
            m_UserName = userName;
            m_pGateways = new List<SIP_Gateway>();
        }

        #endregion
    }
}