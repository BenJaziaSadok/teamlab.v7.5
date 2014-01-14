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

namespace ASC.Xmpp.Core.protocol.iq.last
{
    /// <summary>
    ///   Summary description for LastIq.
    /// </summary>
    public class LastIq : IQ
    {
        private readonly Last m_Last = new Last();

        public LastIq()
        {
            base.Query = m_Last;
            GenerateId();
        }

        public LastIq(IqType type) : this()
        {
            Type = type;
        }

        public LastIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public LastIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Last Query
        {
            get { return m_Last; }
        }
    }
}