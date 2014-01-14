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

namespace ASC.Xmpp.Core.protocol.extensions.commands
{
    /*
      <xs:attribute name='action' use='optional'>
        <xs:simpleType>
          <xs:restriction base='xs:NCName'>
            <xs:enumeration value='cancel'/>
            <xs:enumeration value='complete'/>
            <xs:enumeration value='execute'/>
            <xs:enumeration value='next'/>
            <xs:enumeration value='prev'/>
          </xs:restriction>
        </xs:simpleType>
      </xs:attribute>
    */

    public enum Action
    {
        NONE = -1,
        next = 1,
        prev = 2,
        complete = 4,
        execute = 8,
        cancel = 16
    }
}