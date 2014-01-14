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

namespace ASC.Mail.Net.FTP
{
    /// <summary>
    /// Specifies FTP data connection transfer mode.
    /// </summary>
    public enum FTP_TransferMode
    {
        /// <summary>
        /// Active transfer mode - FTP server opens data connection FTP client.
        /// </summary>
        Active,

        /// <summary>
        /// Passive transfer mode - FTP client opens data connection FTP server.
        /// </summary>
        Passive
    }
}