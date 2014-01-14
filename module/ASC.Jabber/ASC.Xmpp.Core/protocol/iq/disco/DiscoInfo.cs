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

/*
Example 1. Querying for further information

<iq type='get'
from='romeo@montague.net/orchard'
to='plays.shakespeare.lit'
id='info1'>
<query xmlns='http://jabber.org/protocol/disco#info'/>
</iq>


Example 2. Result-set for information request

<iq type='result'
    from='plays.shakespeare.lit'
    to='romeo@montague.net/orchard'
    id='info1'>
  <query xmlns='http://jabber.org/protocol/disco#info'>
    <identity
        category='conference'
        type='text'
        name='Play-Specific Chatrooms'/>
    <identity
        category='directory'
        type='chatroom'
        name='Play-Specific Chatrooms'/>
    <feature var='http://jabber.org/protocol/disco#info'/>
    <feature var='http://jabber.org/protocol/disco#items'/>
    <feature var='http://jabber.org/protocol/muc'/>
    <feature var='jabber:iq:register'/>
    <feature var='jabber:iq:search'/>
    <feature var='jabber:iq:time'/>
    <feature var='jabber:iq:version'/>
  </query>
</iq>
    

Example 3. Target entity does not exist

<iq type='error'
    from='plays.shakespeare.lit'
    to='romeo@montague.net/orchard'
    id='info1'>
  <query xmlns='http://jabber.org/protocol/disco#info'/>
  <error code='404' type='cancel'>
    <item-not-found xmlns='urn:ietf:xml:params:ns:xmpp-stanzas'/>
  </error>
</iq>
    
 */

namespace ASC.Xmpp.Core.protocol.iq.disco
{
    /// <summary>
    ///   Discovering Information About a Jabber Entity
    /// </summary>
    public class DiscoInfo : Element
    {
        public DiscoInfo()
        {
            TagName = "query";
            Namespace = Uri.DISCO_INFO;
        }

        /// <summary>
        ///   Optional node Attrib
        /// </summary>
        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }

        public DiscoIdentity AddIdentity()
        {
            var id = new DiscoIdentity();
            AddChild(id);
            return id;
        }

        public void AddIdentity(DiscoIdentity id)
        {
            AddChild(id);
        }

        public DiscoFeature AddFeature()
        {
            var f = new DiscoFeature();
            AddChild(f);
            return f;
        }

        public void AddFeature(DiscoFeature f)
        {
            AddChild(f);
        }

        public DiscoIdentity[] GetIdentities()
        {
            ElementList nl = SelectElements(typeof (DiscoIdentity));
            var items = new DiscoIdentity[nl.Count];
            int i = 0;
            foreach (Element e in nl)
            {
                items[i] = (DiscoIdentity) e;
                i++;
            }
            return items;
        }

        /// <summary>
        ///   Gets all Features
        /// </summary>
        /// <returns> </returns>
        public DiscoFeature[] GetFeatures()
        {
            ElementList nl = SelectElements(typeof (DiscoFeature));
            var items = new DiscoFeature[nl.Count];
            int i = 0;
            foreach (Element e in nl)
            {
                items[i] = (DiscoFeature) e;
                i++;
            }
            return items;
        }

        /// <summary>
        ///   Check if a feature is supported
        /// </summary>
        /// <param name="var"> </param>
        /// <returns> </returns>
        public bool HasFeature(string var)
        {
            DiscoFeature[] features = GetFeatures();
            foreach (DiscoFeature feat in features)
            {
                if (feat.Var == var)
                    return true;
            }
            return false;
        }
    }
}