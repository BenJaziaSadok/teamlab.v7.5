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

namespace ASC.Mail.Net.IMAP.Server
{
    /// <summary>
    /// Provides data for GetUserQuota event.
    /// </summary>
    public class IMAP_eArgs_GetQuota
    {
        #region Members

        private readonly IMAP_Session m_pSession;

        #endregion

        #region Properties

        /// <summary>
        /// Gets current IMAP session.
        /// </summary>
        public IMAP_Session Session
        {
            get { return m_pSession; }
        }

        /// <summary>
        /// Gets user name.
        /// </summary>
        public string UserName
        {
            get { return m_pSession.UserName; }
        }

        /// <summary>
        /// Gets or sets maximum mailbox size.
        /// </summary>
        public long MaxMailboxSize { get; set; }

        /// <summary>
        /// Gets or sets current mailbox size.
        /// </summary>
        public long MailboxSize { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="session">Owner IMAP session.</param>
        public IMAP_eArgs_GetQuota(IMAP_Session session)
        {
            m_pSession = session;
        }

        #endregion
    }
}