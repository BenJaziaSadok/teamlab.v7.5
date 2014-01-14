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

namespace ASC.Mail.Net.SMTP.Server
{
    #region usings

    using System;
    using System.IO;

    #endregion

    /// <summary>
    /// This class provided data for <b cref="SMTP_Session.GetMessageStream">SMTP_Session.GetMessageStream</b> event.
    /// </summary>
    public class SMTP_e_Message : EventArgs
    {
        #region Members

        private readonly SMTP_Session m_pSession;
        private Stream m_pStream;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="session">Owner SMTP server session.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>session</b> is null reference.</exception>
        public SMTP_e_Message(SMTP_Session session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            m_pSession = session;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets owner SMTP session.
        /// </summary>
        public SMTP_Session Session
        {
            get { return m_pSession; }
        }

        /// <summary>
        /// Gets or stes stream where to store incoming message.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null reference is passed.</exception>
        public Stream Stream
        {
            get { return m_pStream; }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Stream");
                }

                m_pStream = value;
            }
        }

        #endregion
    }
}