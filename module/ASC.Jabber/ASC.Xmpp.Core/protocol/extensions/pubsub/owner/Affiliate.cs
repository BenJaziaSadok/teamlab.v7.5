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

namespace ASC.Xmpp.Core.protocol.extensions.pubsub.owner
{
    /*
        <iq type='result'
            from='pubsub.shakespeare.lit'
            to='hamlet@denmark.lit/elsinore'
            id='ent1'>
          <pubsub xmlns='http://jabber.org/protocol/pubsub#owner'>
            <affiliates node='blogs/princely_musings'>
              <affiliate jid='hamlet@denmark.lit' affiliation='owner'/>
              <affiliate jid='polonius@denmark.lit' affiliation='outcast'/>
            </affiliates>
          </pubsub>
        </iq>
        
        <xs:element name='affiliate'>
            <xs:complexType>
              <xs:simpleContent>
                <xs:extension base='empty'>
                  <xs:attribute name='affiliation' use='required'>
                    <xs:simpleType>
                      <xs:restriction base='xs:NCName'>
                        <xs:enumeration value='none'/>
                        <xs:enumeration value='outcast'/>
                        <xs:enumeration value='owner'/>
                        <xs:enumeration value='publisher'/>
                      </xs:restriction>
                    </xs:simpleType>
                  </xs:attribute>
                  <xs:attribute name='jid' type='xs:string' use='required'/>
                </xs:extension>
              </xs:simpleContent>
            </xs:complexType>
        </xs:element>
     
    */

    public class Affiliate : Element
    {
        #region << Constructors >>

        public Affiliate()
        {
            TagName = "affiliate";
            Namespace = Uri.PUBSUB_OWNER;
        }

        public Affiliate(Jid jid, AffiliationType affiliation) : this()
        {
            Jid = jid;
            Affiliation = affiliation;
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

        public AffiliationType Affiliation
        {
            get { return (AffiliationType) GetAttributeEnum("affiliation", typeof (AffiliationType)); }
            set { SetAttribute("affiliation", value.ToString()); }
        }
    }
}