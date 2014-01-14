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

    using AUTH;

    #endregion

    /// <summary>
    /// This class provides data for SIP_ProxyCore.Authenticate event.
    /// </summary>
    public class SIP_AuthenticateEventArgs
    {
        #region Members

        private readonly Auth_HttpDigest m_pAuth;

        #endregion

        #region Properties

        /// <summary>
        /// Gets authentication context.
        /// </summary>
        public Auth_HttpDigest AuthContext
        {
            get { return m_pAuth; }
        }

        /// <summary>
        /// Gets or sets if specified request is authenticated.
        /// </summary>
        public bool Authenticated { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="auth">Authentication context.</param>
        public SIP_AuthenticateEventArgs(Auth_HttpDigest auth)
        {
            m_pAuth = auth;
        }

        #endregion
    }
}