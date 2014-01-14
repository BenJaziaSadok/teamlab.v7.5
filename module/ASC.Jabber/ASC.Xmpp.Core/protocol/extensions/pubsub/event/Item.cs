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

namespace ASC.Xmpp.Core.protocol.extensions.pubsub.@event
{
    /*
        <message from='pubsub.shakespeare.lit' to='francisco@denmark.lit' id='foo'>
          <event xmlns='http://jabber.org/protocol/pubsub#event'>
            <items node='blogs/princely_musings'>
              <item id='ae890ac52d0df67ed7cfdf51b644e901'>
                <entry xmlns='http://www.w3.org/2005/Atom'>
                  <title>Soliloquy</title>
                  <summary>
                        To be, or not to be: that is the question:
                        Whether 'tis nobler in the mind to suffer
                        The slings and arrows of outrageous fortune,
                        Or to take arms against a sea of troubles,
                        And by opposing end them?
                  </summary>
                  <link rel='alternate' type='text/html' 
                        href='http://denmark.lit/2003/12/13/atom03'/>
                  <id>tag:denmark.lit,2003:entry-32397</id>
                  <published>2003-12-13T18:30:02Z</published>
                  <updated>2003-12-13T18:30:02Z</updated>
                </entry>
              </item>
            </items>
          </event>
        </message>
     
        <xs:element name='item'>
            <xs:complexType>
              <xs:choice minOccurs='0'>
                <xs:element name='retract' type='empty'/>
                <xs:any namespace='##other'/>
              </xs:choice>
              <xs:attribute name='id' type='xs:string' use='optional'/>
            </xs:complexType>
        </xs:element>
    */

    // This class is the same as the Item class in the main pubsub namespace,
    // so inherit it and overwrite some properties and functions

    public class Item : pubsub.Item
    {
        #region << Constructors >>

        public Item()
        {
            Namespace = Uri.PUBSUB_EVENT;
        }

        public Item(string id) : this()
        {
            Id = id;
        }

        #endregion

        private const string RETRACT = "retract";

        public bool Retract
        {
            get { return HasTag(RETRACT); }
            set
            {
                if (value)
                    SetTag(RETRACT);
                else
                    RemoveTag(RETRACT);
            }
        }
    }
}