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

//<stream:stream xmlns:stream='http://etherx.jabber.org/streams/'
//xmlns='jabber:client'
//from='somedomain'
//version='1.0'>
//<stream:features>
//...
//<register xmlns='http://jabber.org/features/iq-register'/>
//...

namespace ASC.Xmpp.Core.protocol.stream.feature
{
    /// <summary>
    /// </summary>
    public class Register : Element
    {
        public Register()
        {
            TagName = "register";
            Namespace = Uri.FEATURE_IQ_REGISTER;
        }
    }
}