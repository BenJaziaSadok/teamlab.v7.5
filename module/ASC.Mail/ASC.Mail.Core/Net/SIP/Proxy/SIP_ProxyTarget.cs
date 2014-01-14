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
    using Stack;

    #endregion

    /// <summary>
    /// Represents SIP proxy target in the SIP proxy "target set". Defined in RFC 3261 16.
    /// </summary>
    public class SIP_ProxyTarget
    {
        #region Members

        private readonly SIP_Flow m_pFlow;
        private readonly SIP_Uri m_pTargetUri;

        #endregion

        #region Properties

        /// <summary>
        /// Gets target URI.
        /// </summary>
        public SIP_Uri TargetUri
        {
            get { return m_pTargetUri; }
        }

        /// <summary>
        /// Gets data flow. Value null means that new flow must created.
        /// </summary>
        public SIP_Flow Flow
        {
            get { return m_pFlow; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="targetUri">Target request-URI.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>targetUri</b> is null reference.</exception>
        public SIP_ProxyTarget(SIP_Uri targetUri) : this(targetUri, null) {}

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="targetUri">Target request-URI.</param>
        /// <param name="flow">Data flow to try for forwarding..</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>targetUri</b> is null reference.</exception>
        public SIP_ProxyTarget(SIP_Uri targetUri, SIP_Flow flow)
        {
            if (targetUri == null)
            {
                throw new ArgumentNullException("targetUri");
            }

            m_pTargetUri = targetUri;
            m_pFlow = flow;
        }

        #endregion
    }
}