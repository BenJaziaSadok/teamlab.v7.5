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
    /*
        <affiliation node='node1' jid='francisco@denmark.lit' affiliation='owner'/>
    */

    public class Affiliation : Element
    {
        #region << Constructors >>

        public Affiliation()
        {
            TagName = "affiliation";
            Namespace = Uri.PUBSUB;
        }

        public Affiliation(Jid jid, AffiliationType affiliation)
        {
            Jid = jid;
            AffiliationType = affiliation;
        }

        public Affiliation(string node, Jid jid, AffiliationType affiliation) : this(jid, affiliation)
        {
            Node = node;
        }

        #endregion

        public Jid Jid
        {
            get
            {
                if (HasAttribute("jid"))
                    return new Jid(GetAttribute("jid"));
                else
                    return null;
            }
            set
            {
                if (value != null)
                    SetAttribute("jid", value.ToString());
            }
        }

        public string Node
        {
            get { return GetAttribute("node"); }
            set { SetAttribute("node", value); }
        }

        public AffiliationType AffiliationType
        {
            get { return (AffiliationType) GetAttributeEnum("affiliation", typeof (AffiliationType)); }
            set { SetAttribute("affiliation", value.ToString()); }
        }
    }
}