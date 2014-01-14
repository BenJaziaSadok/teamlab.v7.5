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
    ///   User had been composing but now has stopped. User was composing but has not interacted with the message input interface for a short period of time (e.g., 5 seconds).
    /// </summary>
    public class Paused : Element
    {
        /// <summary>
        /// </summary>
        public Paused()
        {
            TagName = Chatstate.paused.ToString();
            ;
            Namespace = Uri.CHATSTATES;
        }
    }
}