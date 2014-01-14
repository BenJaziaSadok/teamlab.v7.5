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

namespace ASC.Xmpp.Core.protocol.extensions.chatstates
{
    /// <summary>
    ///   Enumeration of supported Chatstates (JEP-0085)
    /// </summary>
    public enum Chatstate
    {
        /// <summary>
        ///   No Chatstate at all
        /// </summary>
        None,

        /// <summary>
        ///   Active Chatstate
        /// </summary>
        active,

        /// <summary>
        ///   Inactive Chatstate
        /// </summary>
        inactive,

        /// <summary>
        ///   Composing Chatstate
        /// </summary>
        composing,

        /// <summary>
        ///   Gone Chatstate
        /// </summary>
        gone,

        /// <summary>
        ///   Paused Chatstate
        /// </summary>
        paused
    }
}