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
    /// This class holds MIME content disposition types. Defined in RFC 2183.
    /// </summary>
    public class MIME_DispositionTypes
    {
        #region Members

        /// <summary>
        /// Bodyparts can be designated `attachment' to indicate that they are separate from the main body of the mail message, 
        /// and that their display should not be automatic, but contingent upon some further action of the user.
        /// </summary>
        public static readonly string Attachment = "attachment";

        /// <summary>
        /// A bodypart should be marked `inline' if it is intended to be displayed automatically upon display of the message. 
        /// Inline bodyparts should be presented in the order in which they occur, subject to the normal semantics of multipart messages.
        /// </summary>
        public static readonly string Inline = "inline";

        #endregion
    }
}