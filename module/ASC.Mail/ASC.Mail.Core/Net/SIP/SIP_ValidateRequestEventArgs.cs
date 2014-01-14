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

namespace ASC.Mail.Net.SIP.Stack
{
    #region usings

    using System;
    using System.Net;

    #endregion

    /// <summary>
    /// This class provides data for SIP_Stack.ValidateRequest event.
    /// </summary>
    public class SIP_ValidateRequestEventArgs : EventArgs
    {
        #region Members

        private readonly IPEndPoint m_pRemoteEndPoint;
        private readonly SIP_Request m_pRequest;

        #endregion

        #region Properties

        /// <summary>
        /// Gets incoming SIP request.
        /// </summary>
        public SIP_Request Request
        {
            get { return m_pRequest; }
        }

        /// <summary>
        /// Gets IP end point what made request.
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get { return m_pRemoteEndPoint; }
        }

        /// <summary>
        /// Gets or sets response code. Value null means SIP stack will handle it.
        /// </summary>
        public string ResponseCode { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="request">Incoming SIP request.</param>
        /// <param name="remoteEndpoint">IP end point what made request.</param>
        public SIP_ValidateRequestEventArgs(SIP_Request request, IPEndPoint remoteEndpoint)
        {
            m_pRequest = request;
            m_pRemoteEndPoint = remoteEndpoint;
        }

        #endregion
    }
}