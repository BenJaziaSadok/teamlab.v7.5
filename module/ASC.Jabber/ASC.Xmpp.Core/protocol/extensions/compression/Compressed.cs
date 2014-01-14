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

namespace ASC.Xmpp.Core.protocol.extensions.compression
{
    /*
     * Example 5. Receiving Entity Acknowledges Stream Compression
     * <compressed xmlns='http://jabber.org/protocol/compress'/> 
     */

    public class Compressed : Element
    {
        public Compressed()
        {
            TagName = "compressed";
            Namespace = Uri.COMPRESS;
        }
    }
}