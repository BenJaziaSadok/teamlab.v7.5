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
    ///   User is composing a message. User is interacting with a message input interface specific to this chat session (e.g., by typing in the input area of a chat window).
    /// </summary>
    public class Composing : Element
    {
        public Composing()
        {
            TagName = Chatstate.composing.ToString();
            ;
            Namespace = Uri.CHATSTATES;
        }
    }
}