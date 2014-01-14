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

    using System;
    using System.Text;
    using MIME;

    #endregion

    /// <summary>
    /// Represents "Disposition-Notification-Options:" header. Defined in RFC 2298 2.2.
    /// </summary>
    public class Mail_h_DispositionNotificationOptions : MIME_h
    {
        /*
            Disposition-Notification-Options    = "Disposition-Notification-Options" ":" disposition-notification-parameters
            disposition-notification-parameters = parameter *(";" parameter)

            parameter  = attribute "=" importance "," 1#value
            importance = "required" / "optional"
        */

        #region Properties

        /// <summary>
        /// Gets if this header field is modified since it has loaded.
        /// </summary>
        /// <remarks>All new added header fields has <b>IsModified = true</b>.</remarks>
        /// <exception cref="ObjectDisposedException">Is riased when this class is disposed and this property is accessed.</exception>
        public override bool IsModified
        {
            get { return true; } //m_pAddresses.IsModified; }
        }

        /// <summary>
        /// Gets header field name. For example "Sender".
        /// </summary>
        public override string Name
        {
            get { return "Disposition-Notification-Options"; }
        }

        /// <summary>
        /// Gets or sets mailbox address.
        /// </summary>
        public string Address
        {
            get { return "TODO:"; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns header field as string.
        /// </summary>
        /// <param name="wordEncoder">8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <param name="parmetersCharset">Charset to use to encode 8-bit characters. Value null means parameters not encoded.</param>
        /// <returns>Returns header field as string.</returns>
        public override string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset)
        {
            return "TODO:";
        }

        #endregion
    }
}