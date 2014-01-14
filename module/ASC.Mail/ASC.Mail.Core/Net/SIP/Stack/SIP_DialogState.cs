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
    /// Specifies dialog state.
    /// </summary>
    public enum SIP_DialogState
    {
        /// <summary>
        /// Dialog isn't established yet.
        /// </summary>
        Early,

        /// <summary>
        /// Dialog has got 2xx response.
        /// </summary>
        Confirmed,

        /// <summary>
        /// Dialog is terminating.
        /// </summary>
        Terminating,

        /// <summary>
        /// Dialog is terminated.
        /// </summary>
        Terminated,

        /// <summary>
        /// Dialog is disposed.
        /// </summary>
        Disposed,
    }
}