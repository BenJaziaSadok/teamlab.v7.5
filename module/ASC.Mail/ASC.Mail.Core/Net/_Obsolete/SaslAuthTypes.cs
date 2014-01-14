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

namespace ASC.Mail.Net.AUTH
{
    /// <summary>
    /// SASL authentications
    /// </summary>
    public enum SaslAuthTypes
    {
        /// <summary>
        /// Non authentication
        /// </summary>
        None = 0,

        /// <summary>
        /// Plain text authentication. For POP3 USER/PASS commands, for IMAP LOGIN command.
        /// </summary>
        Plain = 1,

        /// <summary>
        /// LOGIN.
        /// </summary>
        Login = 2,

        /// <summary>
        /// CRAM-MD5
        /// </summary>
        Cram_md5 = 4,

        /// <summary>
        /// DIGEST-MD5.
        /// </summary>
        Digest_md5 = 8,

        /// <summary>
        /// All authentications.
        /// </summary>
        All = 0xF,
    }
}