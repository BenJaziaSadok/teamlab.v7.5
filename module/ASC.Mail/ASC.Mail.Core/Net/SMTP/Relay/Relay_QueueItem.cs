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

namespace ASC.Mail.Net.SMTP.Relay
{
    #region usings

    using System.IO;

    #endregion

    /// <summary>
    /// Thsi class holds Relay_Queue queued item.
    /// </summary>
    public class Relay_QueueItem
    {
        #region Members

        private readonly string m_From = "";
        private readonly string m_MessageID = "";
        private readonly Stream m_pMessageStream;
        private readonly Relay_Queue m_pQueue;
        private readonly string m_To = "";

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="queue">Item owner queue.</param>
        /// <param name="from">Sender address.</param>
        /// <param name="to">Target recipient address.</param>
        /// <param name="messageID">Message ID.</param>
        /// <param name="message">Raw mime message. Message reading starts from current position.</param>
        /// <param name="tag">User data.</param>
        internal Relay_QueueItem(Relay_Queue queue,
                                 string from,
                                 string to,
                                 string messageID,
                                 Stream message,
                                 object tag)
        {
            m_pQueue = queue;
            m_From = from;
            m_To = to;
            m_MessageID = messageID;
            m_pMessageStream = message;
            Tag = tag;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets from address.
        /// </summary>
        public string From
        {
            get { return m_From; }
        }

        /// <summary>
        /// Gets message ID which is being relayed now.
        /// </summary>
        public string MessageID
        {
            get { return m_MessageID; }
        }

        /// <summary>
        /// Gets raw mime message which must be relayed.
        /// </summary>
        public Stream MessageStream
        {
            get { return m_pMessageStream; }
        }

        /// <summary>
        /// Gets this relay item owner queue.
        /// </summary>
        public Relay_Queue Queue
        {
            get { return m_pQueue; }
        }

        /// <summary>
        /// Gets or sets user data.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets target recipient.
        /// </summary>
        public string To
        {
            get { return m_To; }
        }

        #endregion
    }
}