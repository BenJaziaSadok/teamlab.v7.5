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

namespace ASC.Xmpp.Core.protocol.extensions.ping
{
    /// <summary>
    /// </summary>
    public class PingIq : IQ
    {
        private readonly Ping m_Ping = new Ping();

        #region << Constructors >>

        public PingIq()
        {
            base.Query = m_Ping;
            GenerateId();
        }

        public PingIq(Jid to) : this()
        {
            To = to;
        }

        public PingIq(Jid to, Jid from) : this()
        {
            To = to;
            From = from;
        }

        #endregion

        public new Ping Query
        {
            get { return m_Ping; }
        }
    }
}