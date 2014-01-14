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
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.component
{
    public enum RouteType
    {
        NONE = -1,
        error,
        auth,
        session
    }

    /// <summary>
    /// </summary>
    public class Route : Stanza
    {
        public Route()
        {
            TagName = "route";
            Namespace = Uri.ACCEPT;
        }

        public Route(Element route) : this()
        {
            RouteElement = route;
        }

        public Route(Element route, Jid from, Jid to) : this()
        {
            RouteElement = route;
            From = from;
            To = to;
        }

        public Route(Element route, Jid from, Jid to, RouteType type) : this()
        {
            RouteElement = route;
            From = from;
            To = to;
            Type = type;
        }

        /// <summary>
        ///   Gets or Sets the logtype
        /// </summary>
        public RouteType Type
        {
            get { return (RouteType) GetAttributeEnum("type", typeof (RouteType)); }
            set
            {
                if (value == RouteType.NONE)
                    RemoveAttribute("type");
                else
                    SetAttribute("type", value.ToString());
            }
        }

        /// <summary>
        ///   sets or gets the element to route
        /// </summary>
        public Element RouteElement
        {
            get { return FirstChild; }
            set
            {
                if (HasChildElements)
                    RemoveAllChildNodes();

                if (value != null)
                    AddChild(value);
            }
        }
    }
}