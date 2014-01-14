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
     * 
     * Note: If the initiating entity did not understand any of the advertised compression methods, 
     * it SHOULD ignore the compression option and proceed as if no compression methods were advertised.
     * 
     * If the initiating entity requests a stream compression method that is not supported by the 
     * receiving entity, the receiving entity MUST return an <unsupported-method/> error:
     * 
     * Example 3. Receiving Entity Reports That Method is Unsupported
     * <failure xmlns='http://jabber.org/protocol/compress'>
     *  <unsupported-method/>
     * </failure>
     * 
     * If the receiving entity cannot establish compression using the requested method for any 
     * other reason, it MUST return a <setup-failed/> error:
     * 
     * Example 4. Receiving Entity Reports That Compression Setup Failed
     * <failure xmlns='http://jabber.org/protocol/compress'>
     *  <setup-failed/>
     * </failure>
     */

    public class Failure : Element
    {
        public Failure()
        {
            TagName = "failure";
            Namespace = Uri.COMPRESS;
        }
    }
}