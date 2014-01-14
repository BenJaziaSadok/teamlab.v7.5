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

namespace ASC.Mail.Net.SMTP.Server
{
    #region usings

    using System;

    #endregion

    /// <summary>
    ///  This class holds RCPT TO: command value.
    /// </summary>
    public class SMTP_RcptTo
    {
        #region Members

        private readonly string m_Mailbox = "";
        private readonly SMTP_Notify m_Notify = SMTP_Notify.NotSpecified;
        private readonly string m_ORCPT = "";

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="mailbox">Mailbox value.</param>
        /// <param name="notify">DSN NOTIFY parameter value.</param>
        /// <param name="orcpt">DSN ORCPT parameter value.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>mailbox</b> is null reference.</exception>
        public SMTP_RcptTo(string mailbox, SMTP_Notify notify, string orcpt)
        {
            if (mailbox == null)
            {
                throw new ArgumentNullException("mailbox");
            }

            m_Mailbox = mailbox;
            m_Notify = notify;
            m_ORCPT = orcpt;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets SMTP "mailbox" value. Actually this is just email address.
        /// </summary>
        public string Mailbox
        {
            get { return m_Mailbox; }
        }

        /// <summary>
        /// Gets DSN NOTIFY parameter value.
        /// This value specified when SMTP server should send delivery status notification.
        /// Defined in RFC 1891.
        /// </summary>
        public SMTP_Notify Notify
        {
            get { return m_Notify; }
        }

        /// <summary>
        /// Gets DSN ORCPT parameter value. Value null means not specified.
        /// This value specifies "original" recipient address where message is sent (has point only when message forwarded).
        /// Defined in RFC 1891.
        /// </summary>
        public string ORCPT
        {
            get { return m_ORCPT; }
        }

        #endregion
    }
}