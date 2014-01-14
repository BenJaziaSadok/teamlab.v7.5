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

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.extensions.pubsub.owner
{
    public class PubSubIq : IQ
    {
        /*
            Example 133. Owner deletes a node

            <iq type='set'
                from='hamlet@denmark.lit/elsinore'
                to='pubsub.shakespeare.lit'
                id='delete1'>
              <pubsub xmlns='http://jabber.org/protocol/pubsub#owner'>
                <delete node='blogs/princely_musings'/>
              </pubsub>
            </iq>
                
        */
        private readonly PubSub m_PubSub = new PubSub();

        #region << Constructors >>

        public PubSubIq()
        {
            GenerateId();
            AddChild(m_PubSub);
        }

        public PubSubIq(IqType type) : this()
        {
            Type = type;
        }

        public PubSubIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public PubSubIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        #endregion

        public PubSub PubSub
        {
            get { return m_PubSub; }
        }
    }
}