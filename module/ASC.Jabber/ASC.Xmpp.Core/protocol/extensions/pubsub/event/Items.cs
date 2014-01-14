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

namespace ASC.Xmpp.Core.protocol.extensions.pubsub.@event
{
    /*
        <message from='pubsub.shakespeare.lit' to='francisco@denmark.lit' id='foo'>
          <event xmlns='http://jabber.org/protocol/pubsub#event'>
            <items node='blogs/princely_musings'>
              <item id='ae890ac52d0df67ed7cfdf51b644e901'>
                <entry xmlns='http://www.w3.org/2005/Atom'>
                  <title>Soliloquy</title>
                  <summary>
                        To be, or not to be: that is the question:
                        Whether 'tis nobler in the mind to suffer
                        The slings and arrows of outrageous fortune,
                        Or to take arms against a sea of troubles,
                        And by opposing end them?
                  </summary>
                  <link rel='alternate' type='text/html' 
                        href='http://denmark.lit/2003/12/13/atom03'/>
                  <id>tag:denmark.lit,2003:entry-32397</id>
                  <published>2003-12-13T18:30:02Z</published>
                  <updated>2003-12-13T18:30:02Z</updated>
                </entry>
              </item>
            </items>
          </event>
        </message>
     
        <xs:element name='items'>
            <xs:complexType>
              <xs:sequence>
                <xs:element ref='item' minOccurs='0' maxOccurs='unbounded'/>
              </xs:sequence>
              <xs:attribute name='node' type='xs:string' use='required'/>
            </xs:complexType>
        </xs:element>
    */

    public class Items : Element
    {
        #region << Constructors >>

        public Items()
        {
            TagName = "items";
            Namespace = Uri.PUBSUB_EVENT;
        }

        public Items(string node) : this()
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
        ///   Add a payload Item
        /// </summary>
        /// <returns> returns the added Item </returns>
        public Item AddItem()
        {
            var item = new Item();
            AddChild(item);
            return item;
        }

        /// <summary>
        /// </summary>
        /// <param name="item"> </param>
        /// <returns> returns the added item </returns>
        public Item AddItem(Item item)
        {
            AddChild(item);
            return item;
        }

        /// <summary>
        ///   This will return all payload items. Multiple items are possible, but doe the most implementaions one item should be enough
        /// </summary>
        /// <returns> returns an Array of Items </returns>
        public Item[] GetItems()
        {
            ElementList nl = SelectElements(typeof (Item));
            var items = new Item[nl.Count];
            int i = 0;
            foreach (Element e in nl)
            {
                items[i] = (Item) e;
                i++;
            }
            return items;
        }
    }
}