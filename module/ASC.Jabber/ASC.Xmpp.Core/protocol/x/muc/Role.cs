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

using System;

namespace ASC.Xmpp.Core.protocol.x.muc
{
    /// <summary>
    ///   There are four defined roles that an occupant may have
    /// </summary>
    [Flags]
    public enum Role
    {
        /// <summary>
        ///   the absence of a role
        /// </summary>
        none,

        /// <summary>
        /// </summary>
        moderator,

        /// <summary>
        /// </summary>
        participant,

        /// <summary>
        /// </summary>
        visitor
    }
}