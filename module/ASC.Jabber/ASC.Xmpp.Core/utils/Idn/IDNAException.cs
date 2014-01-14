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

using System;

namespace ASC.Xmpp.Core.utils.Idn
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class IDNAException : Exception
    {
        #region Members

        /// <summary>
        /// </summary>
        public static string CONTAINS_ACE_PREFIX = "ACE prefix (xn--) not allowed.";

        /// <summary>
        /// </summary>
        public static string CONTAINS_HYPHEN = "Leading or trailing hyphen not allowed.";

        /// <summary>
        /// </summary>
        public static string CONTAINS_NON_LDH = "Contains non-LDH characters.";

        /// <summary>
        /// </summary>
        public static string TOO_LONG = "String too long.";

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        /// <param name="m"> </param>
        public IDNAException(string m) : base(m)
        {
        }

        // TODO
        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        public IDNAException(StringprepException e) : base(string.Empty, e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        public IDNAException(PunycodeException e) : base(string.Empty, e)
        {
        }

        #endregion
    }
}