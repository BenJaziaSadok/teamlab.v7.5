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

namespace ASC.Xmpp.Core.protocol.x
{

    #region usings

    #endregion

    // <x xmlns="jabber:x:avatar"><hash>bbf231f2b7fa1772c2ec5cffa620d3aedb4bd793</hash></x>

    /// <summary>
    ///   JEP-0008 avatars
    /// </summary>
    public class Avatar : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Avatar()
        {
            TagName = "x";
            Namespace = Uri.X_AVATAR;
        }

        /// <summary>
        /// </summary>
        /// <param name="hash"> </param>
        public Avatar(string hash) : this()
        {
            Hash = hash;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public string Hash
        {
            get { return GetTag("hash"); }

            set { SetTag("hash", value); }
        }

        #endregion
    }
}