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
    /// This enum holds SSL modes.
    /// </summary>
    public enum SslMode
    {
        /// <summary>
        /// No SSL is used.
        /// </summary>
        None,

        /// <summary>
        /// Connection is SSL.
        /// </summary>
        SSL,

        /// <summary>
        /// Connection will be switched to SSL with start TLS.
        /// </summary>
        TLS
    }
}