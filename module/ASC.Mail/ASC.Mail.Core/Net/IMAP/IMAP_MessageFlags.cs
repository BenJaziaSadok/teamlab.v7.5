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

namespace ASC.Mail.Net.IMAP
{
    using System;

    /// <summary>
    /// IMAP message flags.
    /// </summary>
    [Flags]
    public enum IMAP_MessageFlags
    {
        /// <summary>
        /// No flags defined.
        /// </summary>
        None = 0,

        /// <summary>
        /// Message has been read.
        /// </summary>
        Seen = 2,

        /// <summary>
        /// Message has been answered.
        /// </summary>
        Answered = 4,

        /// <summary>
        /// Message is "flagged" for urgent/special attention.
        /// </summary>
        Flagged = 8,

        /// <summary>
        /// Message is "deleted" for removal by later EXPUNGE.
        /// </summary>
        Deleted = 16,

        /// <summary>
        /// Message has not completed composition.
        /// </summary>
        Draft = 32,

        /// <summary>
        /// Message is "recently" arrived in this mailbox.
        /// </summary>
        Recent = 64,
    }
}