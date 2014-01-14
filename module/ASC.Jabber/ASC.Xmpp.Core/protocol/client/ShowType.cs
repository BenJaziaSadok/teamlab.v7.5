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

namespace ASC.Xmpp.Core.protocol.client
{
    //	# away -- The entity or resource is temporarily away.
    //	# chat -- The entity or resource is actively interested in chatting.
    //	# dnd -- The entity or resource is busy (dnd = "Do Not Disturb").
    //	# xa -- The entity or resource is away for an extended period (xa = "eXtended Away").

    /// <summary>
    ///   Enumeration that represents the online state.
    /// </summary>
    public enum ShowType
    {
        /// <summary>
        /// </summary>
        NONE = -1,

        /// <summary>
        ///   The entity or resource is temporarily away.
        /// </summary>
        away,

        /// <summary>
        ///   The entity or resource is actively interested in chatting.
        /// </summary>
        chat,

        /// <summary>
        ///   The entity or resource is busy (dnd = "Do Not Disturb").
        /// </summary>
        dnd,

        /// <summary>
        ///   The entity or resource is away for an extended period (xa = "eXtended Away").
        /// </summary>
        xa,
    }
}