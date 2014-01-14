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
    ///   User is actively participating in the chat session. User accepts an initial content message, sends a content message, gives focus to the chat interface, or is otherwise paying attention to the conversation.
    /// </summary>
    public class Active : Element
    {
        /// <summary>
        /// </summary>
        public Active()
        {
            TagName = Chatstate.active.ToString();
            Namespace = Uri.CHATSTATES;
        }
    }
}