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
using ASC.Xmpp.Core.protocol.iq.@private;

namespace ASC.Xmpp.Core.protocol.extensions.bookmarks
{
    /// <summary>
    /// </summary>
    public class StorageIq : PrivateIq
    {
        public StorageIq()
        {
            Query.AddChild(new Storage());
        }

        public StorageIq(IqType type) : this()
        {
            Type = type;
        }

        public StorageIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public StorageIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }
    }
}