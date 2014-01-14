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
        <note type='info'>Service 'httpd' has been configured.</note>
        
        <xs:element name='note'>
            <xs:complexType>
              <xs:simpleContent>
                <xs:extension base='xs:string'>
                  <xs:attribute name='type' use='required'>
                    <xs:simpleType>
                      <xs:restriction base='xs:NCName'>
                        <xs:enumeration value='error'/>
                        <xs:enumeration value='info'/>
                        <xs:enumeration value='warn'/>
                      </xs:restriction>
                    </xs:simpleType>
                  </xs:attribute>
                </xs:extension>
              </xs:simpleContent>
            </xs:complexType>
        </xs:element>
    */

    public class Note : Element
    {
        /// <summary>
        ///   Default constructor
        /// </summary>
        public Note()
        {
            TagName = "note";
            Namespace = Uri.COMMANDS;
        }

        /// <summary>
        /// </summary>
        /// <param name="type"> </param>
        public Note(NoteType type) : this()
        {
            Type = type;
        }

        /// <summary>
        /// </summary>
        /// <param name="text"> </param>
        /// <param name="type"> </param>
        public Note(string text, NoteType type) : this(type)
        {
            Value = text;
        }

        public NoteType Type
        {
            get { return (NoteType) GetAttributeEnum("type", typeof (NoteType)); }
            set { SetAttribute("type", value.ToString()); }
        }
    }
}