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

namespace ASC.Xmpp.Core.protocol.iq.oob
{
    /// <summary>
    ///   Summary description for OobIq.
    /// </summary>
    public class OobIq : IQ
    {
        private readonly Oob m_Oob = new Oob();

        public OobIq()
        {
            base.Query = m_Oob;
            GenerateId();
        }

        public OobIq(IqType type) : this()
        {
            Type = type;
        }

        public OobIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public OobIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Oob Query
        {
            get { return m_Oob; }
        }
    }
}