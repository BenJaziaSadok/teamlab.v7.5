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
    ///   Enumeration for the Presence Type structure. This enum is used to describe what type of Subscription Type the current subscription is. When sending a presence or receiving a subscription this type is used to easily identify the type of subscription it is.
    /// </summary>
    public enum PresenceType
    {
        /// <summary>
        ///   Used when one wants to send presence to someone/server/transport that you�re available.
        /// </summary>
        available = -1,

        /// <summary>
        ///   Used to send a subscription request to someone.
        /// </summary>
        subscribe,

        /// <summary>
        ///   Used to accept a subscription request.
        /// </summary>
        subscribed,

        /// <summary>
        ///   Used to unsubscribe someone from your presence.
        /// </summary>
        unsubscribe,

        /// <summary>
        ///   Used to deny a subscription request.
        /// </summary>
        unsubscribed,

        /// <summary>
        ///   Used when one wants to send presence to someone/server/transport that you�re unavailable.
        /// </summary>
        unavailable,

        /// <summary>
        ///   Used when you want to see your roster, but don't want anyone on you roster to see you
        /// </summary>
        invisible,

        /// <summary>
        ///   presence error
        /// </summary>
        error,

        /// <summary>
        ///   used in server to server protocol to request presences
        /// </summary>
        probe
    }
}