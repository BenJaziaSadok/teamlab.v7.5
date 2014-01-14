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

namespace ASC.Mail.Net.SIP.Proxy
{
    /// <summary>
    /// This enum specifies SIP proxy server 'forking' mode.
    /// </summary>
    public enum SIP_ForkingMode
    {
        /// <summary>
        /// No forking. The contact with highest q value is used.
        /// </summary>
        None,

        /// <summary>
        /// All contacts are processed parallel at same time.
        /// </summary>
        Parallel,

        /// <summary>
        /// In a sequential search, a proxy server attempts each contact address in sequence, 
        /// proceeding to the next one only after the previous has generated a final response. 
        /// Contacts are processed from highest q value to lower.
        /// </summary>
        Sequential,
    }
}