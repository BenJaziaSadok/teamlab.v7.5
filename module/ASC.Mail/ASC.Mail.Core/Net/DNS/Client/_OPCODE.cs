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

namespace ASC.Mail.Net.Dns.Client
{
    /// <summary>
    /// 
    /// </summary>
    internal enum OPCODE
    {
        /// <summary>
        /// A standard query.
        /// </summary>
        QUERY = 0,

        /// <summary>
        /// An inverse query.
        /// </summary>
        IQUERY = 1,

        /// <summary>
        /// A server status request.
        /// </summary>
        STATUS = 2,
    }
}