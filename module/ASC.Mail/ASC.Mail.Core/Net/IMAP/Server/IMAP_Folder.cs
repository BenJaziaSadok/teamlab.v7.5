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
    /// IMAP folder.
    /// </summary>
    public class IMAP_Folder
    {
        #region Members

        private readonly string m_Folder = "";
        private bool m_Selectable = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets IMAP folder name. Eg. Inbox, Inbox/myFolder, ... .
        /// </summary>
        public string Folder
        {
            get { return m_Folder; }
        }

        /// <summary>
        /// Gets or sets if folder is selectable (SELECT command can select this folder).
        /// </summary>
        public bool Selectable
        {
            get { return m_Selectable; }

            set { m_Selectable = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="folder">Full path to folder, path separator = '/'. Eg. Inbox/myFolder .</param>
        /// <param name="selectable">Gets or sets if folder is selectable(SELECT command can select this folder).</param>
        public IMAP_Folder(string folder, bool selectable)
        {
            m_Folder = folder;
            m_Selectable = selectable;
        }

        #endregion
    }
}