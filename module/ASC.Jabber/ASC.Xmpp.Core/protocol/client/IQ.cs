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
using ASC.Xmpp.Core.protocol.iq.bind;
using ASC.Xmpp.Core.protocol.iq.blocklist;
using ASC.Xmpp.Core.protocol.iq.jingle;
using ASC.Xmpp.Core.protocol.iq.session;
using ASC.Xmpp.Core.protocol.iq.vcard;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.client
{

    #region usings

    #endregion

    // a i know that i shouldnt use keywords for Enums. But its much easier this way
    // because of enum.ToString() and enum.Parse() Members
    /// <summary>
    /// </summary>
    public enum IqType
    {
        /// <summary>
        /// </summary>
        get,

        /// <summary>
        /// </summary>
        set,

        /// <summary>
        /// </summary>
        result,

        /// <summary>
        /// </summary>
        error
    }

    /// <summary>
    ///   Iq Stanza.
    /// </summary>
    public class IQ : Stanza
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public IQ()
        {
            TagName = "iq";
            Namespace = Uri.CLIENT;
        }

        /// <summary>
        /// </summary>
        /// <param name="type"> </param>
        public IQ(IqType type) : this()
        {
            Type = type;
        }

        /// <summary>
        /// </summary>
        /// <param name="from"> </param>
        /// <param name="to"> </param>
        public IQ(Jid from, Jid to) : this()
        {
            From = from;
            To = to;
        }

        /// <summary>
        /// </summary>
        /// <param name="type"> </param>
        /// <param name="from"> </param>
        /// <param name="to"> </param>
        public IQ(IqType type, Jid from, Jid to) : this()
        {
            Type = type;
            From = from;
            To = to;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Get or Set the Bind ELement if it is a BingIq
        /// </summary>
        public virtual Bind Bind
        {
            get { return SelectSingleElement(typeof (Bind)) as Bind; }

            set
            {
                RemoveTag(typeof (Bind));
                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        public virtual Blocklist Blocklist
        {
            get { return SelectSingleElement(typeof (Blocklist)) as Blocklist; }

            set
            {
                RemoveTag(typeof (Blocklist));
                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        public virtual Jingle Jingle
        {
            get { return SelectSingleElement(typeof (Jingle)) as Jingle; }

            set
            {
                RemoveTag(typeof (Jingle));
                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        /// <summary>
        ///   Error Child Element
        /// </summary>
        public Error Error
        {
            get { return SelectSingleElement(typeof (Error)) as Error; }

            set
            {
                if (HasTag(typeof (Error)))
                {
                    RemoveTag(typeof (Error));
                }

                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        /// <summary>
        ///   The query Element. Value can also be null which removes the Query tag when existing
        /// </summary>
        public Element Query
        {
            get { return SelectSingleElement("query"); }

            set
            {
                if (value != null)
                {
                    ReplaceChild(value);
                }
                else
                {
                    RemoveTag("query");
                }
            }
        }

        /// <summary>
        ///   Get or Set the Session Element if it is a SessionIq
        /// </summary>
        public virtual Session Session
        {
            get { return SelectSingleElement(typeof (Session)) as Session; }

            set
            {
                RemoveTag(typeof (Session));
                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        /// <summary>
        /// </summary>
        public IqType Type
        {
            get { return (IqType) GetAttributeEnum("type", typeof (IqType)); }

            set { SetAttribute("type", value.ToString()); }
        }

        /// <summary>
        ///   Get or Set the VCard if it is a Vcard IQ
        /// </summary>
        public virtual Vcard Vcard
        {
            get { return SelectSingleElement("vCard") as Vcard; }

            set
            {
                if (value != null)
                {
                    ReplaceChild(value);
                }
                else
                {
                    RemoveTag("vCard");
                }
            }
        }

        #endregion
    }
}