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

namespace ASC.Xmpp.Core.protocol.extensions.msgreceipts
{
    /// <summary>
    /// </summary>
    public class Received : Element
    {
        /*         
         * <received xmlns='http://www.xmpp.org/extensions/xep-0184.html#ns'/>
         */

        public Received()
        {
            TagName = "received";
            Namespace = Uri.MSG_RECEIPT;
        }
    }
}