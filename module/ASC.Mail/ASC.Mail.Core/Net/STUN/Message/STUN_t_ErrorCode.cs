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

namespace ASC.Mail.Net.STUN.Message
{
    /// <summary>
    /// This class implements STUN ERROR-CODE. Defined in RFC 3489 11.2.9.
    /// </summary>
    public class STUN_t_ErrorCode
    {
        #region Members

        private string m_ReasonText = "";

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="code">Error code.</param>
        /// <param name="reasonText">Reason text.</param>
        public STUN_t_ErrorCode(int code, string reasonText)
        {
            Code = code;
            m_ReasonText = reasonText;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets error code.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Gets reason text.
        /// </summary>
        public string ReasonText
        {
            get { return m_ReasonText; }

            set { m_ReasonText = value; }
        }

        #endregion
    }
}