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

#region Using directives

using ASC.Xmpp.Core.protocol.client;

#endregion

namespace ASC.Xmpp.Core.protocol.component
{
    /// <summary>
    ///   Summary description for Iq.
    /// </summary>
    public class IQ : client.IQ
    {
        #region << Constructors >>

        public IQ()
        {
            Namespace = Uri.ACCEPT;
        }

        public IQ(IqType type) : base(type)
        {
            Namespace = Uri.ACCEPT;
        }

        public IQ(Jid from, Jid to) : base(from, to)
        {
            Namespace = Uri.ACCEPT;
        }

        public IQ(IqType type, Jid from, Jid to) : base(type, from, to)
        {
            Namespace = Uri.ACCEPT;
        }

        #endregion

        /// <summary>
        ///   Error Child Element
        /// </summary>
        public new Error Error
        {
            get { return SelectSingleElement(typeof (Error)) as Error; }
            set
            {
                if (HasTag(typeof (Error)))
                    RemoveTag(typeof (Error));

                if (value != null)
                    AddChild(value);
            }
        }
    }
}