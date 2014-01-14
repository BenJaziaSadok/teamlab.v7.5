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
      <xs:element name='item'>
        <xs:complexType>
          <xs:sequence minOccurs='0'>
            <xs:any namespace='##other'/>
          </xs:sequence>
          <xs:attribute name='id' type='xs:string' use='optional'/>
        </xs:complexType>
      </xs:element>
    */

    public class Item : Element
    {
        public Item()
        {
            TagName = "item";
            Namespace = Uri.PUBSUB;
        }

        public Item(string id) : this()
        {
            Id = id;
        }

        /// <summary>
        ///   The optional id
        /// </summary>
        public string Id
        {
            get { return GetAttribute("id"); }
            set { SetAttribute("id", value); }
        }
    }
}