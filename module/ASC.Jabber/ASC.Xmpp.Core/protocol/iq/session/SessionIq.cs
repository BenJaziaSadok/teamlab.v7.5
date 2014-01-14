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

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.iq.session
{
    /// <summary>
    ///   Starting the session, this is done after resource binding
    /// </summary>
    public class SessionIq : IQ
    {
        /*
		SEND:	<iq xmlns="jabber:client" id="agsXMPP_2" type="set" to="jabber.ru">
					<session xmlns="urn:ietf:params:xml:ns:xmpp-session" />
				</iq>
		RECV:	<iq xmlns="jabber:client" from="jabber.ru" type="result" id="agsXMPP_2">
					<session xmlns="urn:ietf:params:xml:ns:xmpp-session" />
				</iq> 
		 */
        private readonly Session m_Session = new Session();

        public SessionIq()
        {
            GenerateId();
            AddChild(m_Session);
        }

        public SessionIq(IqType type) : this()
        {
            Type = type;
        }

        public SessionIq(IqType type, Jid to) : this()
        {
            Type = type;
            To = to;
        }
    }
}