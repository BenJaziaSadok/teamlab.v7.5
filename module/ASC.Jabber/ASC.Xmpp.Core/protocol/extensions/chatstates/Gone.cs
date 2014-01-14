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
    ///   User has effectively ended their participation in the chat session. User has not interacted with the chat interface, system, or device for a relatively long period of time (e.g., 2 minutes), or has terminated the chat interface (e.g., by closing the chat window).
    /// </summary>
    public class Gone : Element
    {
        /// <summary>
        /// </summary>
        public Gone()
        {
            TagName = Chatstate.gone.ToString();
            ;
            Namespace = Uri.CHATSTATES;
        }
    }
}