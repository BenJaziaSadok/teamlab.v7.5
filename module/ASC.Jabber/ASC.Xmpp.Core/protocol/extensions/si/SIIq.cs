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

namespace ASC.Xmpp.Core.protocol.extensions.si
{
    /*
    <iq id="jcl_18" to="xxx" type="result" from="yyy">
        <si xmlns="http://jabber.org/protocol/si">
            <feature xmlns="http://jabber.org/protocol/feature-neg">
                <x xmlns="jabber:x:data" type="submit">
                    <field var="stream-method">
                        <value>http://jabber.org/protocol/bytestreams</value>
                    </field>
                </x>
            </feature>
        </si>
    </iq>
 
    */

    /// <summary>
    /// </summary>
    public class SIIq : IQ
    {
        private readonly SI m_SI = new SI();

        public SIIq()
        {
            GenerateId();
            AddChild(m_SI);
        }

        public SIIq(IqType type)
            : this()
        {
            Type = type;
        }

        public SIIq(IqType type, Jid to)
            : this(type)
        {
            To = to;
        }

        public SIIq(IqType type, Jid to, Jid from)
            : this(type, to)
        {
            From = from;
        }

        public SI SI
        {
            get { return m_SI; }
        }
    }
}