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
    /// This class provided data for <b cref="SMTP_Session.MessageStoringCompleted">SMTP_Session.MessageStoringCompleted</b> event.
    /// </summary>
    public class SMTP_e_MessageStored : EventArgs
    {
        #region Members

        private readonly SMTP_Session m_pSession;
        private readonly Stream m_pStream;
        private SMTP_Reply m_pReply;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="session">Owner SMTP server session.</param>
        /// <param name="stream">Message stream.</param>
        /// <param name="reply">SMTP server reply.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>session</b>, <b>stream</b> or <b>reply</b> is null reference.</exception>
        public SMTP_e_MessageStored(SMTP_Session session, Stream stream, SMTP_Reply reply)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (reply == null)
            {
                throw new ArgumentNullException("reply");
            }

            m_pSession = session;
            m_pStream = stream;
            m_pReply = reply;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets SMTP server reply.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null reference passed.</exception>
        public SMTP_Reply Reply
        {
            get { return m_pReply; }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Reply");
                }

                m_pReply = value;
            }
        }

        /// <summary>
        /// Gets owner SMTP session.
        /// </summary>
        public SMTP_Session Session
        {
            get { return m_pSession; }
        }

        /// <summary>
        /// Gets message stream where message has stored.
        /// </summary>
        public Stream Stream
        {
            get { return m_pStream; }
        }

        #endregion
    }
}