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

namespace ASC.Mail.Net.SIP.Stack
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// This class represent SUBSCRIBE dialog. Defined in RFC 3265.
    /// </summary>
    public class SIP_Dialog_Subscribe
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal SIP_Dialog_Subscribe() {}

        #endregion

        #region Methods

        /// <summary>
        /// Sends notify request to remote end point.
        /// </summary>
        /// <param name="notify">SIP NOTIFY request.</param>
        public void Notify(SIP_Request notify)
        {
            if (notify == null)
            {
                throw new ArgumentNullException("notify");
            }

            // TODO:
        }

        #endregion
    }
}