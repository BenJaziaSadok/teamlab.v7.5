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
    /// Holds POP3_Message info (ID,Size,...).
    /// </summary>
    public class POP3_Message
    {
        #region Members

        private string m_ID = ""; // Holds message ID.
        private POP3_MessageCollection m_pOwner;
        private string m_UID = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets message ID.
        /// </summary>
        public string ID
        {
            get { return m_ID; }

            set { m_ID = value; }
        }

        /// <summary>
        /// Gets or sets message UID. This UID is reported in UIDL command.
        /// </summary>
        public string UID
        {
            get { return m_UID; }

            set { m_UID = value; }
        }

        /// <summary>
        /// Gets or sets message size in bytes.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets message state flag.
        /// </summary>
        public bool MarkedForDelete { get; set; }

        /// <summary>
        /// Gets or sets user data for message.
        /// </summary>
        public object Tag { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="onwer">Owner collection.</param>
        /// <param name="id">Message ID.</param>
        /// <param name="uid">Message UID.</param>
        /// <param name="size">Message size in bytes.</param>
        internal POP3_Message(POP3_MessageCollection onwer, string id, string uid, long size)
        {
            m_pOwner = onwer;
            m_ID = id;
            m_UID = uid;
            Size = size;
        }

        #endregion
    }
}