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

namespace ASC.Mail.Net.SMTP.Client
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// SMTP client exception.
    /// </summary>
    public class SMTP_ClientException : Exception
    {
        #region Members

        private readonly string m_ResponseText = "";
        private readonly int m_StatusCode = 500;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="responseLine">SMTP server response line.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>responseLine</b> is null.</exception>
        public SMTP_ClientException(string responseLine) : base(responseLine)
        {
            if (responseLine == null)
            {
                throw new ArgumentNullException("responseLine");
            }

            string[] code_text = responseLine.Split(new[] {' ', '-'}, 2);
            try
            {
                m_StatusCode = Convert.ToInt32(code_text[0]);
            }
            catch {}
            if (code_text.Length == 2)
            {
                m_ResponseText = code_text[1];
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets if it is permanent SMTP(5xx) error.
        /// </summary>
        public bool IsPermanentError
        {
            get
            {
                if (m_StatusCode >= 500 && m_StatusCode <= 599)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets SMTP server response text after status code.
        /// </summary>
        public string ResponseText
        {
            get { return m_ResponseText; }
        }

        /// <summary>
        /// Gets SMTP status code.
        /// </summary>
        public int StatusCode
        {
            get { return m_StatusCode; }
        }

        #endregion
    }
}