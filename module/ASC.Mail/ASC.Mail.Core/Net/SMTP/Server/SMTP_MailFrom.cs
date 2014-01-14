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
    /// This class holds MAIL FROM: command value.
    /// </summary>
    public class SMTP_MailFrom
    {
        #region Members

        private readonly string m_Body;
        private readonly string m_ENVID;
        private readonly string m_Mailbox = "";
        private readonly string m_RET;
        private readonly int m_Size = -1;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="mailbox">Mailbox value.</param>
        /// <param name="size">SIZE parameter value.</param>
        /// <param name="body">BODY parameter value.</param>
        /// <param name="ret">DSN RET parameter value.</param>
        /// <param name="envid">DSN ENVID parameter value.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>mailbox</b> is null reference.</exception>
        public SMTP_MailFrom(string mailbox, int size, string body, string ret, string envid)
        {
            if (mailbox == null)
            {
                throw new ArgumentNullException("mailbox");
            }

            m_Mailbox = mailbox;
            m_Size = size;
            m_Body = body;
            m_RET = ret;
            m_ENVID = envid;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets MAIL FROM: BODY parameter value. Value null means not specified.
        /// Defined in RFC 1652.
        /// </summary>
        public string Body
        {
            get { return m_Body; }
        }

        /// <summary>
        /// Gets DSN ENVID parameter value. Value null means not specified.
        /// Defined in RFC 1891.
        /// </summary>
        public string ENVID
        {
            get { return m_ENVID; }
        }

        /// <summary>
        /// Gets SMTP "mailbox" value. Actually this is just email address.
        /// This value can be "" if "null reverse-path".
        /// </summary>
        public string Mailbox
        {
            get { return m_Mailbox; }
        }

        /// <summary>
        /// Gets DSN RET parameter value. Value null means not specified.
        /// RET specifies whether message or headers should be included in any failed DSN issued for message transmission.
        /// Defined in RFC 1891.
        /// </summary>
        public string RET
        {
            get { return m_RET; }
        }

        /// <summary>
        /// Gets MAIL FROM: SIZE parameter value. Value -1 means not specified.
        /// Defined in RFC 1870.
        /// </summary>
        public int Size
        {
            get { return m_Size; }
        }

        #endregion
    }
}