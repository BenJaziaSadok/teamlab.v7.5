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

namespace ASC.Xmpp.Core.protocol.iq.privacy
{
    public class List : Element
    {
        public List()
        {
            TagName = "list";
            Namespace = Uri.IQ_PRIVACY;
        }

        public List(string name) : this()
        {
            Name = name;
        }

        public string Name
        {
            get { return GetAttribute("name"); }
            set { SetAttribute("name", value); }
        }

        /// <summary>
        ///   Gets all Rules (Items) when available
        /// </summary>
        /// <returns> </returns>
        public Item[] GetItems()
        {
            ElementList el = SelectElements(typeof (Item));
            int i = 0;
            var result = new Item[el.Count];
            foreach (Item itm in el)
            {
                result[i] = itm;
                i++;
            }
            return result;
        }

        /// <summary>
        ///   Adds a rule (item) to the list
        /// </summary>
        /// <param name="itm"> </param>
        public void AddItem(Item item)
        {
            AddChild(item);
        }

        public void AddItems(Item[] items)
        {
            foreach (Item item in items)
            {
                AddChild(item);
            }
        }

        /// <summary>
        ///   Remove all items/rules of this list
        /// </summary>
        public void RemoveAllItems()
        {
            RemoveTags(typeof (Item));
        }
    }
}