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

namespace ASC.Xmpp.Core.protocol.extensions.geoloc
{
    /// <summary>
    ///   a GeoLoc InfoQuery
    /// </summary>
    public class GeoLocIq : IQ
    {
        private readonly GeoLoc m_GeoLoc = new GeoLoc();

        public GeoLocIq()
        {
            base.Query = m_GeoLoc;
            GenerateId();
        }

        public GeoLocIq(IqType type) : this()
        {
            Type = type;
        }

        public GeoLocIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public GeoLocIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new GeoLoc Query
        {
            get { return m_GeoLoc; }
        }
    }
}