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

using System;
using ASC.Xmpp.Core.utils;
using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.protocol.x.tm.history
{
    public class History : Element
    {
        public History()
        {
            TagName = "query";
            Namespace = Uri.X_TM_IQ_HISTORY;
        }

        public DateTime From
        {
            get { return Time.Date(GetAttribute("from")); }
            set { SetAttribute("from", Time.Date(value)); }
        }

        public DateTime To
        {
            get
            {
                DateTime to = Time.Date(GetAttribute("to"));
                return to != DateTime.MinValue ? to : DateTime.MaxValue;
            }
            set { SetAttribute("to", Time.Date(value)); }
        }

        public int Count
        {
            get { return GetAttributeInt("count"); }
            set { SetAttribute("count", value); }
        }
    }
}