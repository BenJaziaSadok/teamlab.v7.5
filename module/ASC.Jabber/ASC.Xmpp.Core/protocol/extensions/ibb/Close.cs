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
         <close xmlns='http://jabber.org/protocol/ibb' sid='mySID'/>      
    */

    /// <summary>
    /// </summary>
    public class Close : Base
    {
        /// <summary>
        /// </summary>
        public Close()
        {
            TagName = "close";
        }

        /// <summary>
        /// </summary>
        /// <param name="sid"> </param>
        public Close(string sid) : this()
        {
            Sid = sid;
        }
    }
}