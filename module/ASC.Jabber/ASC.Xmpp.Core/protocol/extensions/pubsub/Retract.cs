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
    // Publish and retract looks exactly the same, so inherit from publish here
    public class Retract : Publish
    {
        /*
            A service SHOULD allow a publisher to delete an item once it has been published to a node that 
            supports persistent items.
            To delete an item, the publisher sends a retract request as shown in the following examples. 
            The <retract/> element MUST possess a 'node' attribute and SHOULD contain one <item/> element
            (but MAY contain more than one <item/> element for Batch Processing of item retractions); 
            the <item/> element MUST be empty and MUST possess an 'id' attribute.
            
            <iq type="set"
                from="pgm@jabber.org"
                to="pubsub.jabber.org"
                id="deleteitem1">
              <pubsub xmlns="http://jabber.org/protocol/pubsub">
                <retract node="generic/pgm-mp3-player">
                  <item id="current"/>
                </retract>
              </pubsub>
            </iq>
        */

        public Retract()
        {
            TagName = "retract";
        }

        public Retract(string node) : this()
        {
            Node = node;
        }

        public Retract(string node, string id) : this(node)
        {
            AddItem(new Item(id));
        }
    }
}