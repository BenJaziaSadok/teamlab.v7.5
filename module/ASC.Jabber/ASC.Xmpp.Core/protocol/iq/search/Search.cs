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

using ASC.Xmpp.Core.protocol.x.data;
using ASC.Xmpp.Core.utils.Xml.Dom;

//	Example 1. Requesting Search Fields
//
//	<iq type='get'
//		from='romeo@montague.net/home'
//		to='characters.shakespeare.lit'
//		id='search1'
//		xml:lang='en'>
//		<query xmlns='jabber:iq:search'/>
//	</iq>
//

//	The service MUST then return the possible search fields to the user, and MAY include instructions:
//
//	Example 2. Receiving Search Fields
//
//	<iq type='result'
//		from='characters.shakespeare.lit'
//		to='romeo@montague.net/home'
//		id='search1'
//		xml:lang='en'>
//		<query xmlns='jabber:iq:search'>
//			<instructions>
//			Fill in one or more fields to search
//			for any matching Jabber users.
//			</instructions>
//			<first/>
//			<last/>
//			<nick/>
//			<email/>
//		</query>
//	</iq>    

namespace ASC.Xmpp.Core.protocol.iq.search
{
    /// <summary>
    ///   http://www.jabber.org/jeps/jep-0055.html
    /// </summary>
    public class Search : Element
    {
        public Search()
        {
            TagName = "query";
            Namespace = Uri.IQ_SEARCH;
        }

        public string Instructions
        {
            get { return GetTag("instructions"); }
            set { SetTag("instructions", value); }
        }

        public string Firstname
        {
            get { return GetTag("first"); }
            set { SetTag("first", value); }
        }

        public string Lastname
        {
            get { return GetTag("last"); }
            set { SetTag("last", value); }
        }

        public string Nickname
        {
            get { return GetTag("nick"); }
            set { SetTag("nick", value); }
        }

        public string Email
        {
            get { return GetTag("email"); }
            set { SetTag("email", value); }
        }

        /// <summary>
        ///   The X-Data Element
        /// </summary>
        public Data Data
        {
            get { return SelectSingleElement(typeof (Data)) as Data; }
            set
            {
                if (HasTag(typeof (Data)))
                    RemoveTag(typeof (Data));

                if (value != null)
                    AddChild(value);
            }
        }

        /// <summary>
        ///   Retrieve the result items of a search
        /// </summary>
        //public ElementList GetItems
        //{
        //    get
        //    {
        //        return this.SelectElements("item");
        //    }			
        //}
        public SearchItem[] GetItems()
        {
            ElementList nl = SelectElements(typeof (SearchItem));
            var items = new SearchItem[nl.Count];
            int i = 0;
            foreach (Element e in nl)
            {
                items[i] = (SearchItem) e;
                i++;
            }
            return items;
        }
    }
}