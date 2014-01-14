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

namespace ASC.Mail.Net.SIP.Stack
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// This class provides data for <b>SIP_ServerTransaction.ResponseSent</b> method.
    /// </summary>
    public class SIP_ResponseSentEventArgs : EventArgs
    {
        #region Members

        private readonly SIP_Response m_pResponse;
        private readonly SIP_ServerTransaction m_pTransaction;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="transaction">Server transaction.</param>
        /// <param name="response">SIP response.</param>
        /// <exception cref="ArgumentNullException">Is raised when any of the arguments is null.</exception>
        public SIP_ResponseSentEventArgs(SIP_ServerTransaction transaction, SIP_Response response)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            m_pTransaction = transaction;
            m_pResponse = response;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets response which was sent.
        /// </summary>
        public SIP_Response Response
        {
            get { return m_pResponse; }
        }

        /// <summary>
        /// Gets server transaction which sent response.
        /// </summary>
        public SIP_ServerTransaction ServerTransaction
        {
            get { return m_pTransaction; }
        }

        #endregion
    }
}