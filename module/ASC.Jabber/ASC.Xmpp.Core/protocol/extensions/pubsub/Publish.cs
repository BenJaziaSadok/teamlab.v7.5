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

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    /*
     Example 9. Entity publishes an item with an ItemID of the Payload Type UserTune

        <iq type="set"
            from="pgm@jabber.org"
            to="pubsub.jabber.org"
            id="publish1">
          <pubsub xmlns="http://jabber.org/protocol/pubsub">
            <publish node="generic/pgm-mp3-player">
              <item id="current">
                <tune xmlns="http://jabber.org/protocol/tune">
                  <artist>Ralph Vaughan Williams</artist>
                  <title>Concerto in F for Bass Tuba</title>
                  <source>Golden Brass: The Collector's Edition</source>
                </tune>
              </item>
            </publish>
          </pubsub>
        </iq>
     
    */

    public class Publish : Element
    {
        #region << Constructors >>

        public Publish()
        {
            TagName = "publish";
            Namespace = Uri.PUBSUB;
        }

        /// <summary>
        ///   Its recommended to use this constructor because a node is required
        /// </summary>
        /// <param name="node"> Node to publish </param>
        public Publish(string node) : this()
        {
            Node = node;
        }

        public Publish(string node, Item item) : this(node)
        {
            AddItem(item);
        }

        #endregion

        /// <summary>
        ///   The node to publish to. This Property is required
        /// </summary>
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