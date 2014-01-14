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
    public class PunycodeException : Exception
    {
        #region Members

        /// <summary>
        /// </summary>
        public static string BAD_INPUT = "Bad input.";

        /// <summary>
        /// </summary>
        public static string OVERFLOW = "Overflow.";

        #endregion

        #region Constructor

        /// <summary>
        ///   Creates a new PunycodeException.
        /// </summary>
        /// <param name="message"> message </param>
        public PunycodeException(string message) : base(message)
        {
        }

        #endregion
    }
}