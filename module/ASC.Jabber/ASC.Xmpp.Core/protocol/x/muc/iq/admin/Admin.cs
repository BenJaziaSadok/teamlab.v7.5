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

namespace ASC.Xmpp.Core.protocol.x.muc.iq.admin
{

    #region usings

    #endregion

    /*
        <query xmlns='http://jabber.org/protocol/muc#admin'>
            <item nick='pistol' role='none'>
              <reason>Avaunt, you cullion!</reason>
            </item>
        </query>
    */

    /// <summary>
    /// </summary>
    public class Admin : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Admin()
        {
            TagName = "query";
            Namespace = Uri.MUC_ADMIN;
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="item"> </param>
        public void AddItem(Item item)
        {
            AddChild(item);
        }

        /// <summary>
        /// </summary>
        /// <param name="items"> </param>
        public void AddItems(Item[] items)
        {
            foreach (Item itm in items)
            {
                AddItem(itm);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public Item[] GetItems()
        {
            ElementList nl = SelectElements(typeof (Item));
            var items = new Item[nl.Count];
            int i = 0;
            foreach (Item itm in nl)
            {
                items[i] = itm;
                i++;
            }

            return items;
        }

        #endregion
    }
}