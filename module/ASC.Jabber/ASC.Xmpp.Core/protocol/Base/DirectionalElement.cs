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

namespace ASC.Xmpp.Core.protocol.Base
{

    #region usings

    #endregion

    /// <summary>
    ///   Base XMPP Element This must ne used to build all other new packets
    /// </summary>
    public abstract class DirectionalElement : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public DirectionalElement()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="tag"> </param>
        public DirectionalElement(string tag) : base(tag)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="tag"> </param>
        /// <param name="ns"> </param>
        public DirectionalElement(string tag, string ns) : base(tag)
        {
            Namespace = ns;
        }

        /// <summary>
        /// </summary>
        /// <param name="tag"> </param>
        /// <param name="text"> </param>
        /// <param name="ns"> </param>
        public DirectionalElement(string tag, string text, string ns) : base(tag, text)
        {
            Namespace = ns;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public Jid From
        {
            get
            {
                if (HasAttribute("from"))
                {
                    return new Jid(GetAttribute("from"));
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value != null)
                {
                    SetAttribute("from", value.ToString());
                }
                else
                {
                    RemoveAttribute("from");
                }
            }
        }

        /// <summary>
        /// </summary>
        public Jid To
        {
            get
            {
                if (HasAttribute("to"))
                {
                    return new Jid(GetAttribute("to"));
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value != null)
                {
                    SetAttribute("to", value.ToString());
                }
                else
                {
                    RemoveAttribute("to");
                }
            }
        }

        public bool Switched { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///   Switches the from and to attributes when existing
        /// </summary>
        public void SwitchDirection()
        {
            Jid from = From;
            Jid to = To;

            // Remove from and to now
            RemoveAttribute("from");
            RemoveAttribute("to");

            Jid helper = null;

            helper = from;
            from = to;
            to = helper;

            From = from;
            To = to;

            Switched = !Switched;
        }

        #endregion
    }
}