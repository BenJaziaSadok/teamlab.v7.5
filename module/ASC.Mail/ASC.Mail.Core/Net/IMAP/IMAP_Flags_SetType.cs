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

namespace ASC.Mail.Net.IMAP.Server
{
    /// <summary>
    /// IMAP flags store type.
    /// </summary>
    public enum IMAP_Flags_SetType
    {
        /// <summary>
        /// Flags are added to existing ones.
        /// </summary>
        Add = 1,
        /// <summary>
        /// Flags are removed from existing ones.
        /// </summary>
        Remove = 3,
        /// <summary>
        /// Flags are replaced.
        /// </summary>
        Replace = 4,
    }
}