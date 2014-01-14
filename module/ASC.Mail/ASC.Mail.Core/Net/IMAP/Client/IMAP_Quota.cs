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

namespace ASC.Mail.Net.IMAP.Client
{
    /// <summary>
    /// IMAP quota entry. Defined in RFC 2087.
    /// </summary>
    public class IMAP_Quota
    {
        #region Members

        private readonly long m_MaxMessages = -1;
        private readonly long m_MaxStorage = -1;
        private readonly long m_Messages = -1;
        private readonly string m_QuotaRootName = "";
        private readonly long m_Storage = -1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets quota root name.
        /// </summary>
        public string QuotaRootName
        {
            get { return m_QuotaRootName; }
        }

        /// <summary>
        /// Gets current messages count. Returns -1 if messages and maximum messages quota is not defined.
        /// </summary>
        public long Messages
        {
            get { return m_Messages; }
        }

        /// <summary>
        /// Gets maximum allowed messages count. Returns -1 if messages and maximum messages quota is not defined.
        /// </summary>
        public long MaximumMessages
        {
            get { return m_MaxMessages; }
        }

        /// <summary>
        /// Gets current storage in bytes. Returns -1 if storage and maximum storage quota is not defined.
        /// </summary>
        public long Storage
        {
            get { return m_Storage; }
        }

        /// <summary>
        /// Gets maximum allowed storage in bytes. Returns -1 if storage and maximum storage quota is not defined.
        /// </summary>
        public long MaximumStorage
        {
            get { return m_MaxStorage; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="quotaRootName">Quota root name.</param>
        /// <param name="messages">Number of current messages.</param>
        /// <param name="maxMessages">Number of maximum allowed messages.</param>
        /// <param name="storage">Current storage bytes.</param>
        /// <param name="maxStorage">Maximum allowed storage bytes.</param>
        public IMAP_Quota(string quotaRootName, long messages, long maxMessages, long storage, long maxStorage)
        {
            m_QuotaRootName = quotaRootName;
            m_Messages = messages;
            m_MaxMessages = maxMessages;
            m_Storage = storage;
            m_MaxStorage = maxStorage;
        }

        #endregion
    }
}