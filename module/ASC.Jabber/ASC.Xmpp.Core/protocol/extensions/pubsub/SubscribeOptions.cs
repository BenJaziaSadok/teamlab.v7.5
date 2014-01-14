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

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    /*
        <xs:element name='subscribe-options'>
            <xs:complexType>
              <xs:sequence>
                <xs:element name='required' type='empty' minOccurs='0'/>
              </xs:sequence>
            </xs:complexType>
        </xs:element>
     
        
        Example 36. Service replies with success and indicates that subscription configuration is required

        <iq type='result'
            from='pubsub.shakespeare.lit'
            to='francisco@denmark.lit/barracks'
            id='sub1'>
          <pubsub xmlns='http://jabber.org/protocol/pubsub'>
            <subscription 
                node='blogs/princely_musings'
                jid='francisco@denmark.lit'
                subscription='unconfigured'>
              <subscribe-options>
                <required/>
              </subscribe-options>
            </subscription>
          </pubsub>
        </iq>           
    
    */

    public class SubscribeOptions : Element
    {
        #region << Constructors >>

        public SubscribeOptions()
        {
            TagName = "subscribe-options";
            Namespace = Uri.PUBSUB;
        }

        public SubscribeOptions(bool required)
        {
            Required = required;
        }

        #endregion

        public bool Required
        {
            get { return HasTag("required"); }
            set
            {
                if (value)
                    SetTag("required");
                else
                    RemoveTag("required");
            }
        }
    }
}