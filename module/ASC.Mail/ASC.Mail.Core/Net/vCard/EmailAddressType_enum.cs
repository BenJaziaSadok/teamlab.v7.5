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

namespace ASC.Mail.Net.Mime.vCard
{
    /// <summary>
    /// vCal email address type. Note this values may be flagged !
    /// </summary>
    public enum EmailAddressType_enum
    {
        /// <summary>
        /// Email address type not specified.
        /// </summary>
        NotSpecified = 0,

        /// <summary>
        /// Preferred email address.
        /// </summary>
        Preferred = 1,

        /// <summary>
        /// Internet addressing type.
        /// </summary>
        Internet = 2,

        /// <summary>
        /// X.400 addressing type.
        /// </summary>
        X400 = 4,
    }
}