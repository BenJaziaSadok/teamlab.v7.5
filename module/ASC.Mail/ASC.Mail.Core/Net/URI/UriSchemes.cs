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
    /// This class represents well known URI schemes.
    /// </summary>
    public class UriSchemes
    {
        #region Constants

        /// <summary>
        /// HTTP Extensions for Distributed Authoring (WebDAV).
        /// </summary>
        public const string dav = "dav";

        /// <summary>
        /// Addressing files on local or network file systems.
        /// </summary>
        public const string file = "file";

        /// <summary>
        /// FTP resources.
        /// </summary>
        public const string ftp = "ftp";

        /// <summary>
        /// HTTP resources.
        /// </summary>
        public const string http = "http";

        /// <summary>
        /// HTTP connections secured using SSL/TLS.
        /// </summary>
        public const string https = "https";

        /// <summary>
        /// SMTP e-mail addresses and default content.
        /// </summary>
        public const string mailto = "mailto";

        /// <summary>
        /// Session Initiation Protocol (SIP).
        /// </summary>
        public const string sip = "sip";

        /// <summary>
        /// Session Initiation Protocol (SIP) using TLS.
        /// </summary>
        public const string sips = "sips";

        /// <summary>
        /// Telephone numbers.
        /// </summary>
        public const string tel = "tel";

        #endregion
    }
}