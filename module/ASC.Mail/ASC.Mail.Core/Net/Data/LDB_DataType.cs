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

namespace ASC.Mail.Data.lsDB
{
    /// <summary>
    /// lsDB data types.
    /// </summary>
    public enum LDB_DataType
    {
        /// <summary>
        /// Unicode string.
        /// </summary>
        String = (int) 's',

        /// <summary>
        /// Long (64-bit integer).
        /// </summary>
        Long = (int) 'l',

        /// <summary>
        /// Integer (32-bit integer).
        /// </summary>
        Int = (int) 'i',

        /// <summary>
        /// Date time.
        /// </summary>
        DateTime = (int) 't',

        /// <summary>
        /// Boolean.
        /// </summary>
        Bool = (int) 'b',
    }
}