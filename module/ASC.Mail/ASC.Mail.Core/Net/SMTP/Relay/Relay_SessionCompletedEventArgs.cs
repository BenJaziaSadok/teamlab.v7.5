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

namespace ASC.Mail.Net.SMTP.Relay
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// This class provides data for <b>Relay_Server.SessionCompleted</b> event.
    /// </summary>
    public class Relay_SessionCompletedEventArgs
    {
        #region Members

        private readonly Exception m_pException;
        private readonly Relay_Session m_pSession;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="session">Relay session what completed processing.</param>
        /// <param name="exception">Exception what happened or null if relay completed successfully.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>session</b> is null.</exception>
        public Relay_SessionCompletedEventArgs(Relay_Session session, Exception exception)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            m_pSession = session;
            m_pException = exception;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets Exception what happened or null if relay completed successfully.
        /// </summary>
        public Exception Exception
        {
            get { return m_pException; }
        }

        /// <summary>
        /// Gets relay session what completed processing.
        /// </summary>
        public Relay_Session Session
        {
            get { return m_pSession; }
        }

        #endregion
    }
}