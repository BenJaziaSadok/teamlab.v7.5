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

namespace ASC.Mail.Net.POP3.Server
{
    /// <summary>
    /// Provides data for the GetMessgesList event.
    /// </summary>
    public class GetMessagesInfo_EventArgs
    {
        #region Members

        private readonly POP3_MessageCollection m_pPOP3_Messages;
        private readonly POP3_Session m_pSession;
        private readonly string m_UserName = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets reference to pop3 session.
        /// </summary>
        public POP3_Session Session
        {
            get { return m_pSession; }
        }

        /// <summary>
        /// Gets referance to POP3 messages info.
        /// </summary>
        public POP3_MessageCollection Messages
        {
            get { return m_pPOP3_Messages; }
        }

        /// <summary>
        /// User Name.
        /// </summary>
        public string UserName
        {
            get { return m_UserName; }
        }

        /// <summary>
        /// Mailbox name.
        /// </summary>
        public string Mailbox
        {
            get { return m_UserName; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="session">Reference to pop3 session.</param>
        /// <param name="messages"></param>
        /// <param name="mailbox">Mailbox name.</param>
        public GetMessagesInfo_EventArgs(POP3_Session session, POP3_MessageCollection messages, string mailbox)
        {
            m_pSession = session;
            m_pPOP3_Messages = messages;
            m_UserName = mailbox;
        }

        #endregion
    }
}