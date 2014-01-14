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

namespace ASC.Xmpp.Core.protocol.x.data
{
    /// <summary>
    ///   Used in XData seach reports. includes the headers of the search results
    /// </summary>
    public class Reported : FieldContainer
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Reported()
        {
            TagName = "reported";
            Namespace = Uri.X_DATA;
        }

        #endregion
    }
}