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

namespace ASC.Xmpp.Core.protocol.extensions.ibb
{
    /*
        <open sid='mySID' 
            block-size='4096'
            xmlns='http://jabber.org/protocol/ibb'/>
     
       <xs:element name='open'>
         <xs:complexType>
          <xs:simpleContent>
            <xs:extension base='empty'>
              <xs:attribute name='sid' type='xs:string' use='required'/>
              <xs:attribute name='block-size' type='xs:string' use='required'/>
            </xs:extension>
          </xs:simpleContent>
         </xs:complexType>
       </xs:element>
    */

    public class Open : Base
    {
        /// <summary>
        /// </summary>
        public Open()
        {
            TagName = "open";
        }

        /// <summary>
        /// </summary>
        /// <param name="sid"> </param>
        /// <param name="blocksize"> </param>
        public Open(string sid, long blocksize) : this()
        {
            Sid = sid;
            BlockSize = blocksize;
        }

        /// <summary>
        ///   Block size
        /// </summary>
        public long BlockSize
        {
            get { return GetAttributeLong("block-size"); }
            set { SetAttribute("block-size", value); }
        }
    }
}