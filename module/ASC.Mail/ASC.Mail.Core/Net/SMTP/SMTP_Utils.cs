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

namespace ASC.Mail.Net.SMTP
{
    /// <summary>
    /// This class provedes SMTP related utility methods.
    /// </summary>
    public class SMTP_Utils
    {
        #region Methods

        /// <summary>
        /// Gets if specified smtp address has valid syntax.
        /// </summary>
        /// <param name="address">SMTP address, eg. ivar@lumisoft.ee.</param>
        /// <returns>Returns ture if address is valid, otherwise false.</returns>
        public static bool IsValidAddress(string address)
        {
            // TODO:

            return true;
        }

        #endregion
    }
}