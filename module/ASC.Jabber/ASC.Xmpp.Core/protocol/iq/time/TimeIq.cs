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

namespace ASC.Xmpp.Core.protocol.iq.time
{
    /// <summary>
    ///   Summary description for TimeIq.
    /// </summary>
    public class TimeIq : IQ
    {
        private readonly Time m_Time = new Time();

        public TimeIq()
        {
            base.Query = m_Time;
            GenerateId();
        }

        public TimeIq(IqType type) : this()
        {
            Type = type;
        }

        public TimeIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public TimeIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Time Query
        {
            get { return m_Time; }
        }
    }
}