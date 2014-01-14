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

namespace ASC.Mail.Net.IO
{
    /// <summary>
    /// Specifies action what is done if requested action exceeds maximum allowed size.
    /// </summary>
    public enum SizeExceededAction
    {
        /// <summary>
        /// Throws exception at once when maximum size exceeded.
        /// </summary>
        ThrowException = 1,

        /// <summary>
        /// Junks all data what exceeds maximum allowed size and after requested operation completes,
        /// throws exception.
        /// </summary>
        JunkAndThrowException = 2,
    }
}