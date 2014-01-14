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

namespace ASC.Xmpp.Core.protocol.iq.privacy
{
    /// <summary>
    ///   Summary description for PrivateIq.
    /// </summary>
    public class PrivacyIq : IQ
    {
        private readonly Privacy m_Privacy = new Privacy();

        public PrivacyIq()
        {
            base.Query = m_Privacy;
            GenerateId();
        }

        public PrivacyIq(IqType type)
            : this()
        {
            Type = type;
        }

        public PrivacyIq(IqType type, Jid to)
            : this(type)
        {
            To = to;
        }

        public PrivacyIq(IqType type, Jid to, Jid from)
            : this(type, to)
        {
            From = from;
        }

        public new Privacy Query
        {
            get { return m_Privacy; }
        }
    }
}