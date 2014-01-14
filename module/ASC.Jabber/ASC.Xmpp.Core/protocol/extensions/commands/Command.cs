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

using ASC.Xmpp.Core.protocol.x.data;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.commands
{
    public class Command : Element
    {
        #region << Constructors >>

        public Command()
        {
            TagName = "command";
            Namespace = Uri.COMMANDS;
        }

        public Command(string node) : this()
        {
            Node = node;
        }

        public Command(Action action) : this()
        {
            Action = action;
        }

        public Command(Status status) : this()
        {
            Status = status;
        }

        public Command(string node, string sessionId) : this(node)
        {
            SessionId = sessionId;
        }

        public Command(string node, string sessionId, Action action) : this(node, sessionId)
        {
            Action = action;
        }

        public Command(string node, string sessionId, Status status) : this(node, sessionId)
        {
            Status = status;
        }

        public Command(string node, string sessionId, Action action, Status status) : this(node, sessionId, action)
        {
            Status = status;
        }

        #endregion

        public Action Action
        {
            get { return (Action) GetAttributeEnum("action", typeof (Action)); }
            set
            {
                if (value == Action.NONE)
                    RemoveAttribute("action");
                else
                    SetAttribute("action", value.ToString());
            }
        }

        public Status Status
        {
            get { return (Status) GetAttributeEnum("status", typeof (Status)); }
            set
            {
                if (value == Status.NONE)
                    RemoveAttribute("status");
                else
                    SetAttribute("status", value.ToString());
            }
        }


        // <xs:attribute name='node' type='xs:string' use='required'/>

        /// <summary>
        ///   Node is Required
        /// </summary>
        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }

        // <xs:attribute name='sessionid' type='xs:string' use='optional'/>
        public string SessionId
        {
            get { return GetAttribute("sessionid"); }
            set { SetAttribute("sessionid", value); }
        }

        /// <summary>
        ///   The X-Data Element
        /// </summary>
        public Data Data
        {
            get { return SelectSingleElement(typeof (Data)) as Data; }
            set
            {
                if (HasTag(typeof (Data)))
                    RemoveTag(typeof (Data));

                if (value != null)
                    AddChild(value);
            }
        }

        public Note Note
        {
            get { return SelectSingleElement(typeof (Note)) as Note; }
            set
            {
                if (HasTag(typeof (Note)))
                    RemoveTag(typeof (Note));

                if (value != null)
                    AddChild(value);
            }
        }

        public Actions Actions
        {
            get { return SelectSingleElement(typeof (Actions)) as Actions; }
            set
            {
                if (HasTag(typeof (Actions)))
                    RemoveTag(typeof (Actions));

                if (value != null)
                    AddChild(value);
            }
        }
    }
}