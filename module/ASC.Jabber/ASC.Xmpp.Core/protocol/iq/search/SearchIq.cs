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

//	Example 1. Requesting Search Fields
//
//	<iq type='get'
//		from='romeo@montague.net/home'
//		to='characters.shakespeare.lit'
//		id='search1'
//		xml:lang='en'>
//		<query xmlns='jabber:iq:search'/>
//	</iq>

namespace ASC.Xmpp.Core.protocol.iq.search
{
    /// <summary>
    ///   Summary description for SearchIq.
    /// </summary>
    public class SearchIq : IQ
    {
        private readonly Search m_Search = new Search();

        public SearchIq()
        {
            base.Query = m_Search;
            GenerateId();
        }

        public SearchIq(IqType type) : this()
        {
            Type = type;
        }

        public SearchIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public SearchIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Search Query
        {
            get { return m_Search; }
        }
    }
}