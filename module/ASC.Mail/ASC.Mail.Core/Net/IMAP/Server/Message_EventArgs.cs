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
    /// Provides data for message related events.
    /// </summary>
    public class Message_EventArgs
    {
        #region Members

        private readonly string m_CopyLocation = "";
        private readonly string m_Folder = "";
        private readonly bool m_HeadersOnly;
        private readonly IMAP_Message m_pMessage;

        #endregion

        #region Properties

        /// <summary>
        /// Gets IMAP folder.
        /// </summary>
        public string Folder
        {
            get { return m_Folder; }
        }

        /// <summary>
        /// Gets IMAP message info.
        /// </summary>
        public IMAP_Message Message
        {
            get { return m_pMessage; }
        }

        /// <summary>
        /// Gets message new location. NOTE: this is available for copy command only.
        /// </summary>
        public string CopyLocation
        {
            get { return m_CopyLocation; }
        }

        /// <summary>
        /// Gets or sets message data. NOTE: this is available for GetMessage and StoreMessage event only.
        /// </summary>
        public byte[] MessageData { get; set; }

        /// <summary>
        /// Gets if message headers or full message wanted. NOTE: this is available for GetMessage event only.
        /// </summary>
        public bool HeadersOnly
        {
            get { return m_HeadersOnly; }
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
        /// <param name="folder">IMAP folder which message is.</param>
        /// <param name="msg"></param>
        public Message_EventArgs(string folder, IMAP_Message msg)
        {
            m_Folder = folder;
            m_pMessage = msg;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="folder">IMAP folder which message is.</param>
        /// <param name="msg"></param>
        /// <param name="copyLocation"></param>
        public Message_EventArgs(string folder, IMAP_Message msg, string copyLocation)
        {
            m_Folder = folder;
            m_pMessage = msg;
            m_CopyLocation = copyLocation;
        }

        /// <summary>
        /// GetMessage constructor.
        /// </summary>
        /// <param name="folder">IMAP folder which message is.</param>
        /// <param name="msg"></param>
        /// <param name="headersOnly">Specifies if messages headers or full message is needed.</param>
        public Message_EventArgs(string folder, IMAP_Message msg, bool headersOnly)
        {
            m_Folder = folder;
            m_pMessage = msg;
            m_HeadersOnly = headersOnly;
        }

        #endregion
    }
}