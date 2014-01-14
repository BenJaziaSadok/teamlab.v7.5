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
    /// Specifies message itmes.
    /// </summary>
    public enum IMAP_MessageItems_enum
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,

        /// <summary>
        /// Message main header.
        /// </summary>
        Header = 2,

        /// <summary>
        /// IMAP ENVELOPE structure.
        /// </summary>
        Envelope = 4,

        /// <summary>
        /// IMAP BODYSTRUCTURE structure.
        /// </summary>
        BodyStructure = 8,

        /// <summary>
        /// Full message.
        /// </summary>
        Message = 16,
    }
}