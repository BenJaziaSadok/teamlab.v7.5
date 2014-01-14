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

namespace ASC.Mail.Net.SIP.UA
{
    /// <summary>
    /// This enum specifies SIP UA call states.
    /// </summary>
    public enum SIP_UA_CallState
    {
        /// <summary>
        /// Outgoing call waits to be started.
        /// </summary>
        WaitingForStart,

        /// <summary>
        /// Outgoing calling is in progress.
        /// </summary>
        Calling,

        /// <summary>
        /// Outgoing call remote end party is ringing.
        /// </summary>
        Ringing,

        /// <summary>
        /// Outgoing call remote end pary queued a call.
        /// </summary>
        Queued,

        /// <summary>
        /// Incoming call waits to be accepted.
        /// </summary>
        WaitingToAccept,

        /// <summary>
        /// Call is active.
        /// </summary>
        Active,

        /// <summary>
        /// Call is terminating.
        /// </summary>
        Terminating,

        /// <summary>
        /// Call is terminated.
        /// </summary>
        Terminated,

        /// <summary>
        /// Call has disposed.
        /// </summary>
        Disposed
    }
}