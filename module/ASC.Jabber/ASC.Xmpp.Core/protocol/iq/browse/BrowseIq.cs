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

namespace ASC.Xmpp.Core.protocol.iq.browse
{
    /// <summary>
    ///   Summary description for BrowseIq.
    /// </summary>
    public class BrowseIq : IQ
    {
        private readonly Browse m_Browse = new Browse();

        public BrowseIq()
        {
            base.Query = m_Browse;
            GenerateId();
        }

        public BrowseIq(IqType type) : this()
        {
            Type = type;
        }

        public BrowseIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public BrowseIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Browse Query
        {
            get { return m_Browse; }
        }
    }
}