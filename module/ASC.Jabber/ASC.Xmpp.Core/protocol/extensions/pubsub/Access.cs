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

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    /*
        <xs:attribute name='access' use='optional' default='open'>
            <xs:simpleType>
              <xs:restriction base='xs:NCName'>
                <xs:enumeration value='authorize'/>
                <xs:enumeration value='open'/>
                <xs:enumeration value='presence'/>
                <xs:enumeration value='roster'/>
                <xs:enumeration value='whitelist'/>
              </xs:restriction>
            </xs:simpleType>
        </xs:attribute>
    */

    public enum Access
    {
        NONE = -1,
        open,
        authorize,
        presence,
        roster,
        whitelist
    }
}