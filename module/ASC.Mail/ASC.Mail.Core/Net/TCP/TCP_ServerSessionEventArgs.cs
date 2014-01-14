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

    #endregion

    /// <summary>
    /// This class provides data to .... .
    /// </summary>
    public class TCP_ServerSessionEventArgs<T> : EventArgs where T : TCP_ServerSession, new()
    {
        #region Members

        private readonly TCP_Server<T> m_pServer;
        private readonly T m_pSession;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">TCP server.</param>
        /// <param name="session">TCP server session.</param>
        internal TCP_ServerSessionEventArgs(TCP_Server<T> server, T session)
        {
            m_pServer = server;
            m_pSession = session;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets TCP server.
        /// </summary>
        public TCP_Server<T> Server
        {
            get { return m_pServer; }
        }

        /// <summary>
        /// Gets TCP server session.
        /// </summary>
        public T Session
        {
            get { return m_pSession; }
        }

        #endregion
    }
}