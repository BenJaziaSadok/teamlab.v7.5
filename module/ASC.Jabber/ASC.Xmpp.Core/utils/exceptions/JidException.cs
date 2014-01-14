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

#region file header

#endregion

#region file header

#endregion

using System;

namespace ASC.Xmpp.Core.utils.exceptions
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class JidException : Exception
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public JidException()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="msg"> </param>
        public JidException(string msg) : base(msg)
        {
        }

        #endregion
    }
}