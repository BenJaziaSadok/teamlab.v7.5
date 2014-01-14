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

namespace ASC.Xmpp.Core.protocol.x.data
{

    #region usings

    #endregion

    /// <summary>
    ///   Summary description for Value.
    /// </summary>
    public class Value : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Value()
        {
            TagName = "value";
            Namespace = Uri.X_DATA;
        }

        /// <summary>
        /// </summary>
        /// <param name="val"> </param>
        public Value(string val) : this()
        {
            Value = val;
        }

        /// <summary>
        /// </summary>
        /// <param name="val"> </param>
        public Value(bool val) : this()
        {
            Value = val ? "1" : "0";
        }

        #endregion
    }
}