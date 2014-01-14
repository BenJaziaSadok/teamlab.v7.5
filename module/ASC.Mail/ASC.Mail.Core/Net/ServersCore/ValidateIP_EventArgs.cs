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

namespace ASC.Mail.Net
{
    #region usings

    using System.Net;

    #endregion

    /// <summary>
    /// Provides data for the ValidateIPAddress event for servers.
    /// </summary>
    public class ValidateIP_EventArgs
    {
        #region Members

        private readonly IPEndPoint m_pLocalEndPoint;
        private readonly IPEndPoint m_pRemoteEndPoint;
        private string m_ErrorText = "";
        private bool m_Validated = true;

        #endregion

        #region Properties

        /// <summary>
        /// IP address of computer, which is sending mail to here.
        /// </summary>
        public string ConnectedIP
        {
            get { return m_pRemoteEndPoint.Address.ToString(); }
        }

        /// <summary>
        /// Gets local endpoint.
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get { return m_pLocalEndPoint; }
        }

        /// <summary>
        /// Gets remote endpoint.
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get { return m_pRemoteEndPoint; }
        }

        /// <summary>
        /// Gets or sets if IP is allowed access.
        /// </summary>
        public bool Validated
        {
            get { return m_Validated; }

            set { m_Validated = value; }
        }

        /// <summary>
        /// Gets or sets user data what is stored to session.Tag property.
        /// </summary>
        public object SessionTag { get; set; }

        /// <summary>
        /// Gets or sets error text what is sent to connected socket. NOTE: This is only used if Validated = false.
        /// </summary>
        public string ErrorText
        {
            get { return m_ErrorText; }

            set { m_ErrorText = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="localEndPoint">Server IP.</param>
        /// <param name="remoteEndPoint">Connected client IP.</param>
        public ValidateIP_EventArgs(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint)
        {
            m_pLocalEndPoint = localEndPoint;
            m_pRemoteEndPoint = remoteEndPoint;
        }

        #endregion
    }
}