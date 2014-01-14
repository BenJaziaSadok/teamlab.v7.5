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
    /// <summary>
    /// </summary>
    /// <param name="sender"> </param>
    /// <param name="msg"> </param>
    public delegate void MessageHandler(object sender, Message msg);

    /// <summary>
    /// </summary>
    /// <param name="sender"> </param>
    /// <param name="pres"> </param>
    public delegate void PresenceHandler(object sender, Presence pres);

    /// <summary>
    /// </summary>
    /// <param name="sender"> </param>
    /// <param name="iq"> </param>
    public delegate void IqHandler(object sender, IQ iq);
}