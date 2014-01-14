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

namespace ASC.Xmpp.Core.protocol.x.muc
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class Actor : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Actor()
        {
            TagName = "actor";
            Namespace = Uri.MUC_USER;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public Jid Jid
        {
            get { return GetAttributeJid("jid"); }

            set { SetAttribute("jid", value); }
        }

        #endregion
    }
}