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

    /*
     
        <x xmlns='http://jabber.org/protocol/muc'>
            <password>secret</password>
        </x>
     
     */

    /// <summary>
    ///   Summary description for MucUser.
    /// </summary>
    public class Muc : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Muc()
        {
            TagName = "x";
            Namespace = Uri.MUC;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   The History object
        /// </summary>
        public History History
        {
            get { return SelectSingleElement(typeof (History)) as History; }

            set
            {
                if (HasTag(typeof (History)))
                {
                    RemoveTag(typeof (History));
                }

                if (value != null)
                {
                    AddChild(value);
                }
            }
        }

        /// <summary>
        /// </summary>
        public string Password
        {
            get { return GetTag("password"); }

            set { SetTag("password", value); }
        }

        #endregion
    }
}