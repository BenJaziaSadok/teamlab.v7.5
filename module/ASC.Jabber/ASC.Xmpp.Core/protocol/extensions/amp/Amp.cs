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

using ASC.Xmpp.Core.protocol.Base;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.amp
{
    /*
        <xs:element name='amp'>
            <xs:complexType>
              <xs:sequence>
                <xs:element ref='rule' minOccurs='1' maxOccurs='unbounded'/>
              </xs:sequence>
              <xs:attribute name='from' usage='optional' type='xs:string'/>
              <xs:attribute name='per-hop' use='optional' type='xs:bool' default='false'/>
              <xs:attribute name='status' usage='optional' type='xs:NCName'/>
              <xs:attribute name='to' usage='optional' type='xs:string'/>
            </xs:complexType>
        </xs:element>
    */

    public class Amp : DirectionalElement
    {
        public Amp()
        {
            TagName = "amp";
            Namespace = Uri.AMP;
        }

        /// <summary>
        ///   The 'status' attribute specifies the reason for the amp element. When specifying semantics to be applied (client to server), this attribute MUST NOT be present. When replying to a sending entity regarding a met condition, this attribute MUST be present and SHOULD be the value of the 'action' attribute for the triggered rule. (Note: Individual action definitions MAY provide their own requirements.)
        /// </summary>
        public Action Status
        {
            get { return (Action) GetAttributeEnum("status", typeof (Action)); }
            set
            {
                if (value == Action.Unknown)
                    RemoveAttribute("status");
                else
                    SetAttribute("status", value.ToString());
            }
        }

        /// <summary>
        ///   The 'per-hop' attribute flags the contained ruleset for processing at each server in the route between the original sender and original intended recipient. This attribute MAY be present, and MUST be either "true" or "false". If not present, the default is "false".
        /// </summary>
        public bool PerHop
        {
            get { return GetAttributeBool("per-hop"); }
            set { SetAttribute("per-hop", value); }
        }

        public void AddRule(Rule rule)
        {
            AddChild(rule);
        }

        public Rule AddRule()
        {
            var rule = new Rule();
            AddChild(rule);

            return rule;
        }

        /// <summary>
        ///   Gets a list of all form fields
        /// </summary>
        /// <returns> </returns>
        public Rule[] GetRules()
        {
            ElementList nl = SelectElements(typeof (Rule));
            var items = new Rule[nl.Count];
            int i = 0;
            foreach (Element e in nl)
            {
                items[i] = (Rule) e;
                i++;
            }
            return items;
        }
    }
}