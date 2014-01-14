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

namespace ASC.Xmpp.Core.authorization
{

    #region usings

    #endregion

    /// <summary>
    ///   Summary description for Mechanism.
    /// </summary>
    public abstract class Mechanism
    {
        #region Members

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// </summary>
        public string Username { // lower case that until i implement our c# port of libIDN
            get; set; }

        //public XmppClientConnection XmppClientConnection { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <summary>
        /// </summary>
        /// <param name="con"> </param>
        public abstract void Init();

        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        public abstract void Parse(Node e);

        #endregion
    }
}