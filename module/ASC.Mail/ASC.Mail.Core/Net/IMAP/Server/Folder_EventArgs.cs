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
    /// Provides data for IMAP events.
    /// </summary>
    public class Mailbox_EventArgs
    {
        #region Members

        private readonly string m_Folder = "";
        private readonly string m_NewFolder = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets folder.
        /// </summary>
        public string Folder
        {
            get { return m_Folder; }
        }

        /// <summary>
        /// Gets new folder name, this is available for rename only.
        /// </summary>
        public string NewFolder
        {
            get { return m_NewFolder; }
        }

        /// <summary>
        /// Gets or sets custom error text, which is returned to client.
        /// </summary>
        public string ErrorText { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="folder"></param>
        public Mailbox_EventArgs(string folder)
        {
            m_Folder = folder;
        }

        /// <summary>
        /// Folder rename constructor.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="newFolder"></param>
        public Mailbox_EventArgs(string folder, string newFolder)
        {
            m_Folder = folder;
            m_NewFolder = newFolder;
        }

        #endregion
    }
}