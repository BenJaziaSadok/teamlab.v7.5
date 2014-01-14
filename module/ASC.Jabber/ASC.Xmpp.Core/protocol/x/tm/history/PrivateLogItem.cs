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

using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.protocol.x.tm.history
{
    public class PrivateLogItem : Element
    {
        public PrivateLogItem()
        {
            TagName = "item";
            Namespace = Uri.X_TM_IQ_PRIVATELOG;
        }

        public Jid Jid
        {
            get { return GetAttributeJid("jid"); }
            set { SetAttribute("jid", value); }
        }

        public bool Log
        {
            get { return GetAttributeBool("log"); }
            set { SetAttribute("log", value); }
        }
    }
}