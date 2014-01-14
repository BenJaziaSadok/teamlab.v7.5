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

namespace ASC.Xmpp.Core.protocol.extensions.pubsub.@event
{
    public class Event : Element
    {
        public Event()
        {
            TagName = "event";
            Namespace = Uri.PUBSUB_EVENT;
        }


        public Delete Delete
        {
            get { return SelectSingleElement(typeof (Delete)) as Delete; }
            set
            {
                if (HasTag(typeof (Delete)))
                    RemoveTag(typeof (Delete));

                if (value != null)
                    AddChild(value);
            }
        }

        public Purge Purge
        {
            get { return SelectSingleElement(typeof (Purge)) as Purge; }
            set
            {
                if (HasTag(typeof (Purge)))
                    RemoveTag(typeof (Purge));

                if (value != null)
                    AddChild(value);
            }
        }

        public Items Items
        {
            get { return SelectSingleElement(typeof (Items)) as Items; }
            set
            {
                if (HasTag(typeof (Items)))
                    RemoveTag(typeof (Items));

                if (value != null)
                    AddChild(value);
            }
        }
    }
}