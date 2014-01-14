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

namespace ASC.Xmpp.Core.protocol.iq.auth
{
    /// <summary>
    ///   Summary description for AuthIq.
    /// </summary>
    public class AuthIq : IQ
    {
        private readonly Auth m_Auth = new Auth();

        public AuthIq()
        {
            base.Query = m_Auth;
            GenerateId();
        }

        public AuthIq(IqType type) : this()
        {
            Type = type;
        }

        public AuthIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public AuthIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Auth Query
        {
            get { return m_Auth; }
        }
    }
}