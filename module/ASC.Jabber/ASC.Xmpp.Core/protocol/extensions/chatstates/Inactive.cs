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

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.chatstates
{
    /// <summary>
    ///   User has not been actively participating in the chat session. User has not interacted with the chat interface for an intermediate period of time (e.g., 30 seconds).
    /// </summary>
    public class Inactive : Element
    {
        /// <summary>
        /// </summary>
        public Inactive()
        {
            TagName = Chatstate.inactive.ToString();
            ;
            Namespace = Uri.CHATSTATES;
        }
    }
}