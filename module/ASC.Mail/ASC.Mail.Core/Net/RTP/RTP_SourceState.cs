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

namespace ASC.Mail.Net.RTP
{
    /// <summary>
    /// This enum specifies <b>RTP_SyncSource</b> state.
    /// </summary>
    public enum RTP_SourceState
    {
        /// <summary>
        /// Source is passive, sending only RTCP packets.
        /// </summary>
        Passive = 1,

        /// <summary>
        /// Source is active, sending RTP packets.
        /// </summary>
        Active = 2,

        /// <summary>
        /// Source has disposed.
        /// </summary>
        Disposed = 3,
    }
}