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

namespace ASC.Xmpp.Core.protocol.iq.register
{
    /// <summary>
    ///   Used for registering new usernames on Jabber/XMPP Servers
    /// </summary>
    public class RegisterIq : IQ
    {
        private readonly Register m_Register = new Register();

        public RegisterIq()
        {
            base.Query = m_Register;
            GenerateId();
        }

        public RegisterIq(IqType type) : this()
        {
            Type = type;
        }

        public RegisterIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public RegisterIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Register Query
        {
            get { return m_Register; }
        }
    }
}