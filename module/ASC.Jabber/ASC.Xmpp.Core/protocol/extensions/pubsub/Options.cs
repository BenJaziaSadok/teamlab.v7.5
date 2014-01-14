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

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    /*
        <xs:element name='options'>
            <xs:complexType>
              <xs:sequence minOccurs='0'>
                <xs:any namespace='jabber:x:data'/>
              </xs:sequence>
              <xs:attribute name='jid' type='xs:string' use='required'/>
              <xs:attribute name='node' type='xs:string' use='optional'/>
              <xs:attribute name='subid' type='xs:string' use='optional'/>
            </xs:complexType>
        </xs:element>
     
        <iq type='get'
            from='francisco@denmark.lit/barracks'
            to='pubsub.shakespeare.lit'
            id='options1'>
          <pubsub xmlns='http://jabber.org/protocol/pubsub'>
            <options node='blogs/princely_musings' jid='francisco@denmark.lit'/>
          </pubsub>
        </iq>
    */

    public class Options : Element
    {
        #region << Constructors >>

        public Options()
        {
            TagName = "options";
            Namespace = Uri.PUBSUB;
        }

        public Options(Jid jid) : this()
        {
            Jid = jid;
        }

        public Options(Jid jid, string node) : this(jid)
        {
            Node = node;
        }

        public Options(Jid jid, string node, string subId) : this(jid, node)
        {
            SubId = subId;
        }

        #endregion

        public Jid Jid
        {
            get
            {
                if (HasAttribute("jid"))
                    return new Jid(GetAttribute("jid"));
                else
                    return null;
            }
            set
            {
                if (value != null)
                    SetAttribute("jid", value.ToString());
            }
        }

        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }

        public string SubId
        {
            get { return GetAttribute("subid"); }
            set { SetAttribute("subid", value); }
        }

        /// <summary>
        ///   The X-Data Element/Form
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
    }
}