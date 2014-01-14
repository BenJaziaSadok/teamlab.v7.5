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

namespace ASC.Xmpp.Core.protocol.extensions.commands
{
    /*
      <xs:element name='actions'>
        <xs:complexType>
          <xs:sequence>
            <xs:element name='prev' type='empty' minOccurs='0'/>
            <xs:element name='next' type='empty' minOccurs='0'/>
            <xs:element name='complete' type='empty' minOccurs='0'/>
          </xs:sequence>
          <xs:attribute name='execute' use='optional'>
            <xs:simpleType>
              <xs:restriction base='xs:NCName'>
                <xs:enumeration value='complete'/>
                <xs:enumeration value='next'/>
                <xs:enumeration value='prev'/>
              </xs:restriction>
            </xs:simpleType>
          </xs:attribute>
        </xs:complexType>
      </xs:element>
     
      <actions execute='complete'>
        <prev/>
        <complete/>
      </actions>
    */

    public class Actions : Element
    {
        public Actions()
        {
            TagName = "actions";
            Namespace = Uri.COMMANDS;
        }

        /// <summary>
        ///   Optional Execute Action, only complete, next and previous is allowed
        /// </summary>
        public Action Execute
        {
            get { return (Action) GetAttributeEnum("execute", typeof (Action)); }
            set
            {
                if (value == Action.NONE)
                    RemoveAttribute("execute");
                else
                    SetAttribute("execute", value.ToString());
            }
        }


        /// <summary>
        /// </summary>
        public bool Complete
        {
            get { return HasTag("complete"); }
            set
            {
                if (value)
                    SetTag("complete");
                else
                    RemoveTag("complete");
            }
        }

        public bool Next
        {
            get { return HasTag("next"); }
            set
            {
                if (value)
                    SetTag("next");
                else
                    RemoveTag("next");
            }
        }

        public bool Previous
        {
            get { return HasTag("prev"); }
            set
            {
                if (value)
                    SetTag("prev");
                else
                    RemoveTag("prev");
            }
        }

        /// <summary>
        ///   Actions, only complete, prev and next are allowed here and can be combined
        /// </summary>
        public Action Action
        {
            get
            {
                Action res = 0;

                if (Complete)
                    res |= Action.complete;
                if (Previous)
                    res |= Action.prev;
                if (Next)
                    res |= Action.next;

                if (res == 0)
                    return Action.NONE;
                else
                    return res;
            }
            set
            {
                if (value == Action.NONE)
                {
                    Complete = false;
                    Previous = false;
                    Next = false;
                }
                else
                {
                    Complete = ((value & Action.complete) == Action.complete);
                    Previous = ((value & Action.prev) == Action.prev);
                    Next = ((value & Action.next) == Action.next);
                }
            }
        }
    }
}