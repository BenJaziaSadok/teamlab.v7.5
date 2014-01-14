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
    /// This class provides data for ResponseReceived events.
    /// </summary>
    public class SIP_ResponseReceivedEventArgs : EventArgs
    {
        #region Members

        private readonly SIP_Response m_pResponse;
        private readonly SIP_Stack m_pStack;
        private readonly SIP_ClientTransaction m_pTransaction;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="stack">Reference to SIP stack.</param>
        /// <param name="transaction">Client transaction what response it is. This value can be null if no matching client response.</param>
        /// <param name="response">Received response.</param>
        internal SIP_ResponseReceivedEventArgs(SIP_Stack stack,
                                               SIP_ClientTransaction transaction,
                                               SIP_Response response)
        {
            m_pStack = stack;
            m_pResponse = response;
            m_pTransaction = transaction;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets client transaction which response it is. This value is null if no matching client transaction.
        /// If this core is staless proxy then it's allowed, otherwise core MUST discard that response.
        /// </summary>
        public SIP_ClientTransaction ClientTransaction
        {
            get { return m_pTransaction; }
        }

        /// <summary>
        /// Gets SIP dialog where Response belongs to. Returns null if Response doesn't belong any dialog.
        /// </summary>
        public SIP_Dialog Dialog
        {
            get { return m_pStack.TransactionLayer.MatchDialog(m_pResponse); }
        }

        /// <summary>
        /// Gets response received by SIP stack.
        /// </summary>
        public SIP_Response Response
        {
            get { return m_pResponse; }
        }

        #endregion
    }
}