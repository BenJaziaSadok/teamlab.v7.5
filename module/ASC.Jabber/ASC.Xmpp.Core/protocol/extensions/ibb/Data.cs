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
         <data xmlns='http://jabber.org/protocol/ibb' sid='mySID' seq='0'>
            qANQR1DBwU4DX7jmYZnncmUQB/9KuKBddzQH+tZ1ZywKK0yHKnq57kWq+RFtQdCJ
            WpdWpR0uQsuJe7+vh3NWn59/gTc5MDlX8dS9p0ovStmNcyLhxVgmqS8ZKhsblVeu
            IpQ0JgavABqibJolc3BKrVtVV1igKiX/N7Pi8RtY1K18toaMDhdEfhBRzO/XB0+P
            AQhYlRjNacGcslkhXqNjK5Va4tuOAPy2n1Q8UUrHbUd0g+xJ9Bm0G0LZXyvCWyKH
            kuNEHFQiLuCY6Iv0myq6iX6tjuHehZlFSh80b5BVV9tNLwNR5Eqz1klxMhoghJOA
         </data>
      
         <xs:element name='data'>
             <xs:complexType>
              <xs:simpleContent>
                <xs:extension base='xs:string'>
                  <xs:attribute name='sid' type='xs:string' use='required'/>
                  <xs:attribute name='seq' type='xs:string' use='required'/>
                </xs:extension>
              </xs:simpleContent>
             </xs:complexType>
           </xs:element>
    */

    /// <summary>
    /// </summary>
    public class Data : Base
    {
        /// <summary>
        /// </summary>
        public Data()
        {
            TagName = "data";
        }

        /// <summary>
        /// </summary>
        /// <param name="sid"> </param>
        /// <param name="seq"> </param>
        public Data(string sid, int seq) : this()
        {
            Sid = sid;
            Sequence = seq;
        }

        /// <summary>
        ///   the sequence
        /// </summary>
        public int Sequence
        {
            get { return GetAttributeInt("seq"); }
            set { SetAttribute("seq", value); }
        }
    }
}