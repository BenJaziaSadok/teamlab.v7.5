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

namespace ASC.Xmpp.Core.protocol.iq.avatar
{
    /// <summary>
    ///   Summary description for AvatarIq.
    /// </summary>
    public class AvatarIq : IQ
    {
        private readonly Avatar m_Avatar = new Avatar();

        public AvatarIq()
        {
            base.Query = m_Avatar;
            GenerateId();
        }

        public AvatarIq(IqType type) : this()
        {
            Type = type;
        }

        public AvatarIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public AvatarIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Avatar Query
        {
            get { return m_Avatar; }
        }
    }
}