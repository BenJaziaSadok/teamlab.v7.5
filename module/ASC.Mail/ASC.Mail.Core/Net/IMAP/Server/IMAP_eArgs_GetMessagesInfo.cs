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
    /// Provides data to event GetMessagesInfo.
    /// </summary>
    public class IMAP_eArgs_GetMessagesInfo
    {
        #region Members

        private readonly IMAP_SelectedFolder m_pFolderInfo;
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
        /// Gets folder info.
        /// </summary>
        public IMAP_SelectedFolder FolderInfo
        {
            get { return m_pFolderInfo; }
        }

        /// <summary>
        /// Gets or sets custom error text, which is returned to client. Null value means no error.
        /// </summary>
        public string ErrorText { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IMAP_eArgs_GetMessagesInfo(IMAP_Session session, IMAP_SelectedFolder folder)
        {
            m_pSession = session;
            m_pFolderInfo = folder;
        }

        #endregion
    }
}