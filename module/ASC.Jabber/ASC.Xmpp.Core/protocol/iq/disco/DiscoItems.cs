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
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.disco
{
    /*
	Example 10. Requesting all items

	<iq type='get'
	from='romeo@montague.net/orchard'
	to='shakespeare.lit'
	id='items1'>
	<query xmlns='http://jabber.org/protocol/disco#items'/>
	</iq>
	
	
	Example 11. Result-set for all items

	<iq type='result'
		from='shakespeare.lit'
		to='romeo@montague.net/orchard'
		id='items1'>
	<query xmlns='http://jabber.org/protocol/disco#items'>
		<item jid='people.shakespeare.lit'
			name='Directory of Characters'/>
		<item jid='plays.shakespeare.lit'
			name='Play-Specific Chatrooms'/>
		<item jid='mim.shakespeare.lit'
			name='Gateway to Marlowe IM'/>
		<item jid='words.shakespeare.lit'
			name='Shakespearean Lexicon'/>
		<item jid='globe.shakespeare.lit'
			name='Calendar of Performances'/>
		<item jid='headlines.shakespeare.lit'
			name='Latest Shakespearean News'/>
		<item jid='catalog.shakespeare.lit'
			name='Buy Shakespeare Stuff!'/>
		<item jid='en2fr.shakespeare.lit'
			name='French Translation Service'/>
	</query>
	</iq>
	
	
	Example 12. Empty result set

	<iq type='result'
		from='shakespeare.lit'
		to='romeo@montague.net/orchard'
		id='items1'>
	<query xmlns='http://jabber.org/protocol/disco#items'/>
	</iq>
      
    */

    /// <summary>
    ///   Discovering the Items Associated with a Jabber Entity
    /// </summary>
    public class DiscoItems : IQ
    {
        public DiscoItems()
        {
            TagName = "query";
            Namespace = Uri.DISCO_ITEMS;
        }

        /// <summary>
        ///   The node to discover (Optional)
        /// </summary>
        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }

        public DiscoItem AddDiscoItem()
        {
            var item = new DiscoItem();
            AddChild(item);
            return item;
        }

        public void AddDiscoItem(DiscoItem item)
        {
            AddChild(item);
        }

        public DiscoItem[] GetDiscoItems()
        {
            ElementList nl = SelectElements(typeof (DiscoItem));
            var items = new DiscoItem[nl.Count];
            int i = 0;
            foreach (Element e in nl)
            {
                items[i] = (DiscoItem) e;
                i++;
            }
            return items;
        }
    }
}