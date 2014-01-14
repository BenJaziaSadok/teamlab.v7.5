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

#region using

using ASC.Xmpp.Core.protocol.x.data;

#endregion

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    public class Configure : PubSubAction
    {
        #region << Constructors >>

        public Configure()
        {
            TagName = "configure";
        }

        public Configure(string node) : this()
        {
            Node = node;
        }

        public Configure(Type type) : this()
        {
            Type = type;
        }

        public Configure(string node, Type type) : this(node)
        {
            Type = type;
        }

        #endregion

        public Access Access
        {
            get { return (Access) GetAttributeEnum("access", typeof (Access)); }
            set
            {
                if (value == Access.NONE)
                    RemoveAttribute("access");
                else
                    SetAttribute("access", value.ToString());
            }
        }

        /// <summary>
        ///   The x-Data Element
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
    }
}