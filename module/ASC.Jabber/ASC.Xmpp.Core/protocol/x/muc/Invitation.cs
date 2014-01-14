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

namespace ASC.Xmpp.Core.protocol.x.muc
{

    #region usings

    #endregion

    /// <summary>
    ///   A base class vor Decline and Invite We need From, To and SwitchDirection here. This is why we inherit from XmppPacket Base
    /// </summary>
    public abstract class Invitation : Stanza
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Invitation()
        {
            Namespace = Uri.MUC_USER;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   A reason why you want to invite this contact
        /// </summary>
        public string Reason
        {
            get { return GetTag("reason"); }

            set { SetTag("reason", value); }
        }

        #endregion
    }
}