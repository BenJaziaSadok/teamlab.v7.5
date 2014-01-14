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
    /// <summary>
    /// This enum holds SIP transaction states. Defined in RFC 3261.
    /// </summary>
    public enum SIP_TransactionState
    {
        /// <summary>
        /// Client transaction waits <b>Start</b> method to be called.
        /// </summary>
        WaitingToStart,

        /// <summary>
        /// Calling to recipient. This is used only by INVITE client transaction.
        /// </summary>
        Calling,

        /// <summary>
        /// This is transaction initial state. Used only in Non-INVITE transaction.
        /// </summary>
        Trying,

        /// <summary>
        /// This is INVITE server transaction initial state. Used only in INVITE server transaction.
        /// </summary>
        Proceeding,

        /// <summary>
        /// Transaction has got final response.
        /// </summary>
        Completed,

        /// <summary>
        /// Transation has got ACK from request maker. This is used only by INVITE server transaction.
        /// </summary>
        Confirmed,

        /// <summary>
        /// Transaction has terminated and waits disposing.
        /// </summary>
        Terminated,

        /// <summary>
        /// Transaction has disposed.
        /// </summary>
        Disposed,
    }
}