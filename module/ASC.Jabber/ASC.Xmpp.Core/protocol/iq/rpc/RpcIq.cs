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

namespace ASC.Xmpp.Core.protocol.iq.rpc
{
    /// <summary>
    ///   RpcIq.
    /// </summary>
    public class RpcIq : IQ
    {
        private readonly Rpc m_Rpc = new Rpc();

        public RpcIq()
        {
            base.Query = m_Rpc;
            GenerateId();
        }

        public RpcIq(IqType type) : this()
        {
            Type = type;
        }

        public RpcIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public RpcIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public new Rpc Query
        {
            get { return m_Rpc; }
        }
    }
}