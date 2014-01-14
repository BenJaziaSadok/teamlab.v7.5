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
    public class Create : PubSubAction
    {
        #region << Constructors >>

        public Create()
        {
            TagName = "create";
        }

        public Create(string node) : this()
        {
            Node = node;
        }

        public Create(Type type) : this()
        {
            Type = type;
        }

        public Create(string node, Type type) : this(node)
        {
            Type = type;
        }

        #endregion

        /*
        <iq type="set"
            from="pgm@jabber.org"
            to="pubsub.jabber.org"
            id="create1">
            <pubsub xmlns="http://jabber.org/protocol/pubsub">
                <create node="generic/pgm-mp3-player"/>
                <configure/>
            </pubsub>
        </iq>
         
        ...
            <pubsub xmlns="http://jabber.org/protocol/pubsub">
                <create node="test"
	                    type="collection"/>
            </pubsub>
        ...
        
        */
    }
}