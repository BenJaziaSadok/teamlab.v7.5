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

namespace ASC.Mail.Net.MIME
{
    /// <summary>
    /// This enum specifies MIME RFC 2047 'encoded-word' encoding method.
    /// </summary>
    public enum MIME_EncodedWordEncoding
    {
        /// <summary>
        /// The "B" encoding. Defined in RFC 2047 (section 4.1).
        /// </summary>
        Q,

        /// <summary>
        /// The "Q" encoding. Defined in RFC 2047 (section 4.2).
        /// </summary>
        B
    }
}