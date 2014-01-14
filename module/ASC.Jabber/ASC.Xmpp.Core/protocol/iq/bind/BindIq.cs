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

namespace ASC.Xmpp.Core.protocol.iq.bind
{
    /// <summary>
    ///   Summary description for BindIq.
    /// </summary>
    public class BindIq : IQ
    {
        private readonly Bind m_Bind = new Bind();

        public BindIq()
        {
            GenerateId();
            AddChild(m_Bind);
        }

        public BindIq(IqType type) : this()
        {
            Type = type;
        }

        public BindIq(IqType type, Jid to) : this()
        {
            Type = type;
            To = to;
        }

        public BindIq(IqType type, Jid to, string resource) : this(type, to)
        {
            m_Bind.Resource = resource;
        }

        public new Bind Query
        {
            get { return m_Bind; }
        }
    }
}