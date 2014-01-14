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

namespace ASC.Xmpp.Core.protocol.extensions.pubsub.owner
{
    /*
        <iq type='result'
            from='pubsub.shakespeare.lit'
            to='hamlet@denmark.lit/elsinore'
            id='subman1'>
          <pubsub xmlns='http://jabber.org/protocol/pubsub#owner'>
            <subscribers node='blogs/princely_musings'>
              <subscriber jid='hamlet@denmark.lit' subscription='subscribed'/>
              <subscriber jid='polonius@denmark.lit' subscription='unconfigured'/>
            </subscribers>
          </pubsub>
        </iq>
        
        <xs:element name='subscribers'>
            <xs:complexType>
              <xs:sequence>
                <xs:element ref='subscriber' minOccurs='0' maxOccurs='unbounded'/>
              </xs:sequence>
              <xs:attribute name='node' type='xs:string' use='required'/>
            </xs:complexType>
        </xs:element>
    */

    public class Subscribers : Element
    {
        #region << Constructors >>

        public Subscribers()
        {
            TagName = "subscribers";
            Namespace = Uri.PUBSUB_OWNER;
        }

        public Subscribers(string node) : this()
        {
            Node = node;
        }

        #endregion

        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }

        /// <summary>
        ///   Add a Subscriber
        /// </summary>
        /// <returns> </returns>
        public Subscriber AddSubscriber()
        {
            var subscriber = new Subscriber();
            AddChild(subscriber);
            return subscriber;
        }

        /// <summary>
        ///   Add a Subscriber
        /// </summary>
        /// <param name="subscriber"> the Subscriber to add </param>
        /// <returns> </returns>
        public Subscriber AddSubscriber(Subscriber subscriber)
        {
            AddChild(subscriber);
            return subscriber;
        }

        public void AddSubscribers(Subscriber[] subscribers)
        {
            foreach (Subscriber subscriber in subscribers)
            {
                AddSubscriber(subscriber);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public Subscriber[] GetSubscribers()
        {
            ElementList nl = SelectElements(typeof (Subscriber));
            var subscribers = new Subscriber[nl.Count];
            int i = 0;
            foreach (Element e in nl)
            {
                subscribers[i] = (Subscriber) e;
                i++;
            }
            return subscribers;
        }
    }
}