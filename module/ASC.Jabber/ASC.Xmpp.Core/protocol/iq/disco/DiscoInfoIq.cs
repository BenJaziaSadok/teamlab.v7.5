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

namespace ASC.Xmpp.Core.protocol.iq.disco
{
    /// <summary>
    ///   Discovering Information About a Jabber Entity
    /// </summary>
    public class DiscoInfoIq : IQ
    {
        private readonly DiscoInfo m_DiscoInfo = new DiscoInfo();

        public DiscoInfoIq()
        {
            base.Query = m_DiscoInfo;
            GenerateId();
        }

        public DiscoInfoIq(IqType type) : this()
        {
            Type = type;
        }

        public new DiscoInfo Query
        {
            get { return m_DiscoInfo; }
        }
    }
}