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

// Request Roster:
// <iq id='someid' to='myjabber.net' type='get'>
//		<query xmlns='jabber:iq:roster'/>
// </iq>

namespace ASC.Xmpp.Core.protocol.iq.roster
{
    /// <summary>
    ///   Build a new roster query, jabber:iq:roster
    /// </summary>
    public class RosterIq : IQ
    {
        private readonly Roster m_Roster = new Roster();

        public RosterIq()
        {
            base.Query = m_Roster;
            GenerateId();
        }

        public RosterIq(IqType type) : this()
        {
            Type = type;
        }

        public new Roster Query
        {
            get { return m_Roster; }
        }
    }
}