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

// Request Agents:
// <iq id='someid' to='myjabber.net' type='get'>
//		<query xmlns='jabber:iq:agents'/>
// </iq>

namespace ASC.Xmpp.Core.protocol.iq.agent
{
    /// <summary>
    ///   Summary description for AgentsIq.
    /// </summary>
    public class AgentsIq : IQ
    {
        private readonly Agents m_Agents = new Agents();

        public AgentsIq()
        {
            base.Query = m_Agents;
            GenerateId();
        }

        public AgentsIq(IqType type) : this()
        {
            Type = type;
        }

        public AgentsIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public AgentsIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Agents Query
        {
            get { return m_Agents; }
        }
    }
}