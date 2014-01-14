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

namespace ASC.Xmpp.Core.utils.Xml.Dom
{
    /// <summary>
    /// </summary>
    public class Text : Node
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Text()
        {
            NodeType = NodeType.Text;
        }

        /// <summary>
        /// </summary>
        /// <param name="text"> </param>
        public Text(string text) : this()
        {
            Value = text;
        }

        #endregion
    }
}