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

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    /*
        
        Example 38. Entity unsubscribes from a node

        <iq type='set'
            from='francisco@denmark.lit/barracks'
            to='pubsub.shakespeare.lit'
            id='unsub1'>
          <pubsub xmlns='http://jabber.org/protocol/pubsub'>
             <unsubscribe
                 node='blogs/princely_musings'
                 jid='francisco@denmark.lit'/>
          </pubsub>
        </iq>
    
    */

    // looks exactly the same as subscribe, but has an additional Attribute subid

    public class Unsubscribe : Subscribe
    {
        #region << Constructors >>

        public Unsubscribe()
        {
            TagName = "unsubscribe";
        }

        public Unsubscribe(string node, Jid jid) : this()
        {
            Node = node;
            Jid = jid;
        }

        public Unsubscribe(string node, Jid jid, string subid)
            : this(node, jid)
        {
            SubId = subid;
        }

        #endregion

        public string SubId
        {
            get { return GetAttribute("subid"); }
            set { SetAttribute("subid", value); }
        }
    }
}