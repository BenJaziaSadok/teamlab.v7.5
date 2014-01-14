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

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.disco
{
    public enum DiscoAction
    {
        NONE = -1,
        remove,
        update
    }

    ///<summary>
    ///</summary>
    public class DiscoItem : Element
    {
        public DiscoItem()
        {
            TagName = "item";
            Namespace = Uri.DISCO_ITEMS;
        }

        public Jid Jid
        {
            get { return new Jid(GetAttribute("jid")); }
            set { SetAttribute("jid", value.ToString()); }
        }

        public string Name
        {
            get { return GetAttribute("name"); }
            set { SetAttribute("name", value); }
        }

        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }

        public DiscoAction Action
        {
            get { return (DiscoAction) GetAttributeEnum("action", typeof (DiscoAction)); }
            set
            {
                if (value == DiscoAction.NONE)
                    RemoveAttribute("action");
                else
                    SetAttribute("action", value.ToString());
            }
        }
    }
}