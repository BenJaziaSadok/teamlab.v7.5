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
    ///   Summary description for Message.
    /// </summary>
    public class Message : client.Message
    {
        #region << Constructors >>

        public Message()
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(Jid to)
            : base(to)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(Jid to, string body)
            : base(to, body)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(Jid to, Jid from)
            : base(to, from)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(string to, string body)
            : base(to, body)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(Jid to, string body, string subject)
            : base(to, body, subject)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(string to, string body, string subject)
            : base(to, body, subject)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(string to, string body, string subject, string thread)
            : base(to, body, subject, thread)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(Jid to, string body, string subject, string thread)
            : base(to, body, subject, thread)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(string to, MessageType type, string body)
            : base(to, type, body)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(Jid to, MessageType type, string body)
            : base(to, type, body)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(string to, MessageType type, string body, string subject)
            : base(to, type, body, subject)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(Jid to, MessageType type, string body, string subject)
            : base(to, type, body, subject)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(string to, MessageType type, string body, string subject, string thread)
            : base(to, type, body, subject, thread)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(Jid to, MessageType type, string body, string subject, string thread)
            : base(to, type, body, subject, thread)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(Jid to, Jid from, string body)
            : base(to, from, body)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(Jid to, Jid from, string body, string subject)
            : base(to, from, body, subject)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(Jid to, Jid from, string body, string subject, string thread)
            : base(to, from, body, subject, thread)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(Jid to, Jid from, MessageType type, string body)
            : base(to, from, type, body)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(Jid to, Jid from, MessageType type, string body, string subject)
            : base(to, from, type, body, subject)
        {
            Namespace = Uri.ACCEPT;
        }

        public Message(Jid to, Jid from, MessageType type, string body, string subject, string thread)
            : base(to, from, type, body, subject, thread)
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