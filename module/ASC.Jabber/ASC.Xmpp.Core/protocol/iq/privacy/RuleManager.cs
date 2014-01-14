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

using ASC.Xmpp.Core.protocol.iq.roster;

namespace ASC.Xmpp.Core.protocol.iq.privacy
{
    /// <summary>
    ///   Helper class for creating rules for communication blocking
    /// </summary>
    public class RuleManager
    {
        /// <summary>
        ///   Block stanzas by Jid
        /// </summary>
        /// <param name="JidToBlock"> </param>
        /// <param name="Order"> </param>
        /// <param name="stanza"> stanzas you want to block </param>
        /// <returns> </returns>
        public Item BlockByJid(Jid JidToBlock, int Order, Stanza stanza)
        {
            return new Item(Action.deny, Order, Type.jid, JidToBlock.ToString(), stanza);
        }


        /// <summary>
        ///   Block stanzas for a given roster group
        /// </summary>
        /// <param name="group"> </param>
        /// <param name="Order"> </param>
        /// <param name="stanza"> stanzas you want to block </param>
        /// <returns> </returns>
        public Item BlockByGroup(string group, int Order, Stanza stanza)
        {
            return new Item(Action.deny, Order, Type.group, group, stanza);
        }

        /// <summary>
        ///   Block stanzas by subscription type
        /// </summary>
        /// <param name="subType"> </param>
        /// <param name="Order"> </param>
        /// <param name="stanza"> stanzas you want to block </param>
        /// <returns> </returns>
        public Item BlockBySubscription(SubscriptionType subType, int Order, Stanza stanza)
        {
            return new Item(Action.deny, Order, Type.subscription, subType.ToString(), stanza);
        }

        /// <summary>
        ///   Block globally (all users) the given stanzas
        /// </summary>
        /// <param name="Order"> </param>
        /// <param name="stanza"> stanzas you want to block </param>
        /// <returns> </returns>
        public Item BlockGlobal(int Order, Stanza stanza)
        {
            return new Item(Action.deny, Order, stanza);
        }

        /// <summary>
        ///   Allow stanzas by Jid
        /// </summary>
        /// <param name="JidToBlock"> </param>
        /// <param name="Order"> </param>
        /// <param name="stanza"> stanzas you want to block </param>
        /// <returns> </returns>
        public Item AllowByJid(Jid JidToBlock, int Order, Stanza stanza)
        {
            return new Item(Action.allow, Order, Type.jid, JidToBlock.ToString(), stanza);
        }

        /// <summary>
        ///   Allow stanzas for a given roster group
        /// </summary>
        /// <param name="group"> </param>
        /// <param name="Order"> </param>
        /// <param name="stanza"> stanzas you want to block </param>
        /// <returns> </returns>
        public Item AllowByGroup(string group, int Order, Stanza stanza)
        {
            return new Item(Action.allow, Order, Type.group, group, stanza);
        }

        /// <summary>
        ///   Allow stanzas by subscription type
        /// </summary>
        /// <param name="subType"> </param>
        /// <param name="Order"> </param>
        /// <param name="stanza"> stanzas you want to block </param>
        /// <returns> </returns>
        public Item AllowBySubscription(SubscriptionType subType, int Order, Stanza stanza)
        {
            return new Item(Action.allow, Order, Type.subscription, subType.ToString(), stanza);
        }
    }
}