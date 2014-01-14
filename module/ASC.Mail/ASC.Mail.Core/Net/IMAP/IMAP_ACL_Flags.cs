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
    /// <summary>
    /// IMAP ACL(access control list) rights.
    /// </summary>
    public enum IMAP_ACL_Flags
    {
        /// <summary>
        /// No permissions at all.
        /// </summary>
        None = 0,
        /// <summary>
        /// Lookup (mailbox is visible to LIST/LSUB commands).
        /// </summary>
        l = 1,
        /// <summary>
        /// Read (SELECT the mailbox, perform CHECK, FETCH, PARTIAL,SEARCH, COPY from mailbox).
        /// </summary>
        r = 2,
        /// <summary>
        /// Keep seen/unseen information across sessions (STORE SEEN flag).
        /// </summary>
        s = 4,
        /// <summary>
        /// Write (STORE flags other than SEEN and DELETED).
        /// </summary>
        w = 8,
        /// <summary>
        /// Insert (perform APPEND, COPY into mailbox).
        /// </summary>
        i = 16,
        /// <summary>
        /// Post (send mail to submission address for mailbox,not enforced by IMAP4 itself).
        /// </summary>
        p = 32,
        /// <summary>
        /// Create (CREATE new sub-mailboxes in any implementation-defined hierarchy).
        /// </summary>
        c = 64,
        /// <summary>
        /// Delete (STORE DELETED flag, perform EXPUNGE).
        /// </summary>
        d = 128,
        /// <summary>
        /// Administer (perform SETACL).
        /// </summary>
        a = 256,
        /// <summary>
        /// All permissions
        /// </summary>
        All = 0xFFFF,
    }
}