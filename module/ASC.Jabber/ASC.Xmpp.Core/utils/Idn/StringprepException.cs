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

using System;

namespace ASC.Xmpp.Core.utils.Idn
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class StringprepException : Exception
    {
        #region Members

        /// <summary>
        /// </summary>
        public static string BIDI_BOTHRAL = "Contains both R and AL code points.";

        /// <summary>
        /// </summary>
        public static string BIDI_LTRAL = "Leading and trailing code points not both R or AL.";

        /// <summary>
        /// </summary>
        public static string CONTAINS_PROHIBITED = "Contains prohibited code points.";

        /// <summary>
        /// </summary>
        public static string CONTAINS_UNASSIGNED = "Contains unassigned code points.";

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        /// <param name="message"> </param>
        public StringprepException(string message) : base(message)
        {
        }

        #endregion
    }
}