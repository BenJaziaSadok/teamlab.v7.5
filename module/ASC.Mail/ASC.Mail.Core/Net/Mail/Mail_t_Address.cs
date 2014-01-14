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

namespace ASC.Mail.Net.Mail
{
    #region usings

    using MIME;

    #endregion

    /// <summary>
    /// This class represents RFC 5322 3.4 Address class. 
    /// This class is base class for <see cref="Mail_t_Mailbox">mailbox address</see> and <see cref="Mail_t_Group">group address</see>.
    /// </summary>
    public abstract class Mail_t_Address
    {
        #region Methods

        /// <summary>
        /// Returns address as string value.
        /// </summary>
        /// <param name="wordEncoder">8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <returns>Returns address as string value.</returns>
        public abstract string ToString(MIME_Encoding_EncodedWord wordEncoder);

        #endregion
    }
}