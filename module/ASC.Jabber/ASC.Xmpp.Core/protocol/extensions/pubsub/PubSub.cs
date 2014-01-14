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

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    public class PubSub : Element
    {
        public PubSub()
        {
            TagName = "pubsub";
            Namespace = Uri.PUBSUB;
        }

        /// <summary>
        ///   the Create Element of the Pubsub Element
        /// </summary>
        public Create Create
        {
            get { return SelectSingleElement(typeof (Create)) as Create; }
            set
            {
                if (HasTag(typeof (Create)))
                    RemoveTag(typeof (Create));

                if (value != null)
                    AddChild(value);
            }
        }

        public Publish Publish
        {
            get { return SelectSingleElement(typeof (Publish)) as Publish; }
            set
            {
                if (HasTag(typeof (Publish)))
                    RemoveTag(typeof (Publish));

                if (value != null)
                    AddChild(value);
            }
        }

        public Retract Retract
        {
            get { return SelectSingleElement(typeof (Retract)) as Retract; }
            set
            {
                if (HasTag(typeof (Retract)))
                    RemoveTag(typeof (Retract));

                if (value != null)
                    AddChild(value);
            }
        }

        public Subscribe Subscribe
        {
            get { return SelectSingleElement(typeof (Subscribe)) as Subscribe; }
            set
            {
                if (HasTag(typeof (Subscribe)))
                    RemoveTag(typeof (Subscribe));

                if (value != null)
                    AddChild(value);
            }
        }

        public Unsubscribe Unsubscribe
        {
            get { return SelectSingleElement(typeof (Unsubscribe)) as Unsubscribe; }
            set
            {
                if (HasTag(typeof (Unsubscribe)))
                    RemoveTag(typeof (Unsubscribe));

                if (value != null)
                    AddChild(value);
            }
        }

        public Subscriptions Subscriptions
        {
            get { return SelectSingleElement(typeof (Subscriptions)) as Subscriptions; }
            set
            {
                if (HasTag(typeof (Subscriptions)))
                    RemoveTag(typeof (Subscriptions));

                if (value != null)
                    AddChild(value);
            }
        }

        public Affiliations Affiliations
        {
            get { return SelectSingleElement(typeof (Affiliations)) as Affiliations; }
            set
            {
                if (HasTag(typeof (Affiliations)))
                    RemoveTag(typeof (Affiliations));

                if (value != null)
                    AddChild(value);
            }
        }

        public Options Options
        {
            get { return SelectSingleElement(typeof (Options)) as Options; }
            set
            {
                if (HasTag(typeof (Options)))
                    RemoveTag(typeof (Options));

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

        /// <summary>
        ///   The Configure Element of the PunSub Element
        /// </summary>
        public Configure Configure
        {
            get { return SelectSingleElement(typeof (Configure)) as Configure; }
            set
            {
                if (HasTag(typeof (Configure)))
                    RemoveTag(typeof (Configure));

                if (value != null)
                    AddChild(value);
            }
        }
    }
}