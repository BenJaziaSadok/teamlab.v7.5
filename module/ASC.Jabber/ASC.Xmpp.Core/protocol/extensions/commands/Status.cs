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
     * <xs:attribute name='status' use='optional'>
        <xs:simpleType>
          <xs:restriction base='xs:NCName'>
            <xs:enumeration value='canceled'/>
            <xs:enumeration value='completed'/>
            <xs:enumeration value='executing'/>
          </xs:restriction>
        </xs:simpleType>
      </xs:attribute>
    */

    public enum Status
    {
        NONE = -1,
        canceled,
        completed,
        executing
    }
}