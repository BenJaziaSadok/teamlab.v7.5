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
using System.Collections;
using ASC.Xmpp.Core.protocol;

namespace ASC.Xmpp.Core.utils.Collections
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class FullJidComparer : IComparer
    {
        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="x"> </param>
        /// <param name="y"> </param>
        /// <returns> </returns>
        public int Compare(object x, object y)
        {
            if (x is Jid && y is Jid)
            {
                var jidX = (Jid) x;
                var jidY = (Jid) y;

                if (jidX.ToString() == jidY.ToString())
                {
                    return 0;
                }
                else
                {
                    return String.Compare(jidX.ToString(), jidY.ToString());
                }
            }

            throw new ArgumentException("the objects to compare must be Jids");
        }

        #endregion
    }
}