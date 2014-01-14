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
    /// <summary>
    /// Log entry type.
    /// </summary>
    public enum SocketLogEntryType
    {
        /// <summary>
        /// Data is readed from remote endpoint.
        /// </summary>
        ReadFromRemoteEP = 0,

        /// <summary>
        /// Data is sent to remote endpoint.
        /// </summary>
        SendToRemoteEP = 1,

        /// <summary>
        /// Comment log entry.
        /// </summary>
        FreeText = 2,
    }
}