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

namespace ASC.Mail.Net
{
    #region usings

    using System;

    #endregion

    #region public enum ReadReplyCode

    /// <summary>
    /// Reply reading return codes.
    /// </summary>
    public enum ReadReplyCode
    {
        /// <summary>
        /// Read completed successfully.
        /// </summary>
        Ok = 0,

        /// <summary>
        /// Read timed out.
        /// </summary>
        TimeOut = 1,

        /// <summary>
        /// Maximum allowed Length exceeded.
        /// </summary>
        LengthExceeded = 2,

        /// <summary>
        /// Connected client closed connection.
        /// </summary>
        SocketClosed = 3,

        /// <summary>
        /// UnKnown error, eception raised.
        /// </summary>
        UnKnownError = 4,
    }

    #endregion

    /// <summary>
    /// Summary description for ReadException.
    /// </summary>
    public class ReadException : Exception
    {
        #region Members

        private readonly ReadReplyCode m_ReadReplyCode;

        #endregion

        #region Properties

        /// <summary>
        /// Gets read error.
        /// </summary>
        public ReadReplyCode ReadReplyCode
        {
            get { return m_ReadReplyCode; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public ReadException(ReadReplyCode code, string message) : base(message)
        {
            m_ReadReplyCode = code;
        }

        #endregion
    }
}