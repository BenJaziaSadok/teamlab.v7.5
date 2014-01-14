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

namespace ASC.Xmpp.Core.protocol.x.muc
{

    #region usings

    #endregion

    /*
    <x xmlns='http://jabber.org/protocol/muc#user'>
        <status code='100'/>
    </x>    
    */

    /// <summary>
    ///   Summary description for MucUser.
    /// </summary>
    public class Status : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Status()
        {
            TagName = "status";
            Namespace = Uri.MUC_USER;
        }

        /// <summary>
        /// </summary>
        /// <param name="code"> </param>
        public Status(StatusCode code) : this()
        {
            Code = code;
        }

        /// <summary>
        /// </summary>
        /// <param name="code"> </param>
        public Status(int code) : this()
        {
            SetAttribute("code", code);
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public StatusCode Code
        {
            get { return (StatusCode) GetAttributeEnum("code", typeof (StatusCode)); }

            set { SetAttribute("code", value.ToString()); }
        }

        #endregion
    }
}