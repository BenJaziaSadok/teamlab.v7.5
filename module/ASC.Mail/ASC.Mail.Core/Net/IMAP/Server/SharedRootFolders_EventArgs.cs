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
    /// Summary description for SharedRootFolders_EventArgs.
    /// </summary>
    public class SharedRootFolders_EventArgs
    {
        #region Members

        private readonly IMAP_Session m_pSession;

        #endregion

        #region Properties

        /// <summary>
        /// Gets reference to smtp session.
        /// </summary>
        public IMAP_Session Session
        {
            get { return m_pSession; }
        }

        /// <summary>
        /// Gets or sets users shared root folders. Ususaly there is only one root folder 'Shared Folders'.
        /// </summary>
        public string[] SharedRootFolders { get; set; }

        /// <summary>
        /// Gets or sets public root folders. Ususaly there is only one root folder 'Public Folders'.
        /// </summary>
        public string[] PublicRootFolders { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SharedRootFolders_EventArgs(IMAP_Session session)
        {
            m_pSession = session;
        }

        #endregion
    }
}