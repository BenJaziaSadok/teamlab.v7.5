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

namespace ASC.Xmpp.Core.protocol.iq.vcard
{
    //<iq id="id_62" to="gnauck@myjabber.net" type="get"><vCard xmlns="vcard-temp"/></iq>

    /// <summary>
    ///   Summary description for VcardIq.
    /// </summary>
    public class VcardIq : IQ
    {
        private readonly Vcard m_Vcard = new Vcard();

        #region << Constructors >>

        public VcardIq()
        {
            GenerateId();
            AddChild(m_Vcard);
        }

        public VcardIq(IqType type) : this()
        {
            Type = type;
        }

        public VcardIq(IqType type, Vcard vcard) : this(type)
        {
            Vcard = vcard;
        }

        public VcardIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        public VcardIq(IqType type, Jid to, Vcard vcard) : this(type, to)
        {
            Vcard = vcard;
        }

        public VcardIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        public VcardIq(IqType type, Jid to, Jid from, Vcard vcard) : this(type, to, from)
        {
            Vcard = vcard;
        }

        #endregion

        public override Vcard Vcard
        {
            get { return m_Vcard; }
            set { ReplaceChild(value); }
        }
    }
}