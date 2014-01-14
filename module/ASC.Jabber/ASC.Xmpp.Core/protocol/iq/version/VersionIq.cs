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

namespace ASC.Xmpp.Core.protocol.iq.version
{
    /// <summary>
    ///   Summary description for VersionIq.
    /// </summary>
    public class VersionIq : IQ
    {
        private readonly Version m_Version = new Version();

        public VersionIq()
        {
            base.Query = m_Version;
            GenerateId();
        }

        public VersionIq(IqType type) : this()
        {
            Type = type;
        }

        public VersionIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public VersionIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Version Query
        {
            get { return m_Version; }
        }
    }
}