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

namespace ASC.Mail.Net.Mime
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// Rfc 2183 Content-Disposition.
    /// </summary>
    [Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
    public enum ContentDisposition_enum
    {
        /// <summary>
        /// Content is attachment.
        /// </summary>
        Attachment = 0,

        /// <summary>
        /// Content is embbed resource.
        /// </summary>
        Inline = 1,

        /// <summary>
        /// Content-Disposition header field isn't available or isn't written to mime message.
        /// </summary>
        NotSpecified = 30,

        /// <summary>
        /// Content is unknown.
        /// </summary>
        Unknown = 40
    }
}