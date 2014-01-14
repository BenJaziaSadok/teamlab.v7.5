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

using ASC.Xmpp.Core.protocol.Base;
using ASC.Xmpp.Core.utils;

namespace ASC.Xmpp.Core.protocol.component
{
    //<handshake>aaee83c26aeeafcbabeabfcbcd50df997e0a2a1e</handshake>

    /// <summary>
    ///   Handshake Element
    /// </summary>
    public class Handshake : Stanza
    {
        public Handshake()
        {
            TagName = "handshake";
            Namespace = Uri.ACCEPT;
        }

        public Handshake(string password, string streamid) : this()
        {
            SetAuth(password, streamid);
        }

        /// <summary>
        ///   Digest (Hash) for authentication
        /// </summary>
        public string Digest
        {
            get { return Value; }
            set { Value = value; }
        }

        public void SetAuth(string password, string streamId)
        {
            Value = Hash.Sha1Hash(streamId + password);
        }
    }
}