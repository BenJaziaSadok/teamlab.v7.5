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

namespace ASC.Mail.Net.TCP
{
    #region usings

    using System;
    using System.Net;
    using System.Security.Principal;
    using IO;

    #endregion

    /// <summary>
    /// This is base class for TCP_Client and TCP_ServerSession.
    /// </summary>
    public abstract class TCP_Session : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets session authenticated user identity , returns null if not authenticated.
        /// </summary>
        public virtual GenericIdentity AuthenticatedUserIdentity
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the time when session was connected.
        /// </summary>
        public abstract DateTime ConnectTime { get; }

        /// <summary>
        /// Gets session ID.
        /// </summary>
        public abstract string ID { get; }

        /// <summary>
        /// Gets if this session is authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return AuthenticatedUserIdentity != null; }
        }

        /// <summary>
        /// Gets if session is connected.
        /// </summary>
        public abstract bool IsConnected { get; }

        /// <summary>
        /// Gets if this session TCP connection is secure connection.
        /// </summary>
        public virtual bool IsSecureConnection
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the last time when data was sent or received.
        /// </summary>
        public abstract DateTime LastActivity { get; }

        /// <summary>
        /// Gets session local IP end point.
        /// </summary>
        public abstract IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// Gets session remote IP end point.
        /// </summary>
        public abstract IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Gets TCP stream which must be used to send/receive data through this session.
        /// </summary>
        public abstract SmartStream TcpStream { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Disconnects session.
        /// </summary>
        public abstract void Disconnect();

        #endregion
    }
}