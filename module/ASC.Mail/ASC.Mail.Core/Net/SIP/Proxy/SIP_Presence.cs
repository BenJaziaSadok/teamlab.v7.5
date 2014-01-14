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

namespace ASC.Mail.Net.SIP.Proxy
{
    #region usings

    using Stack;

    #endregion

    /// <summary>
    /// This class implements SIP presence server.
    /// </summary>
    public class SIP_Presence
    {
        #region Internal methods

        /// <summary>
        /// Handles SUBSCRIBE method.
        /// </summary>
        /// <param name="e">Request event arguments.</param>
        internal void Subscribe(SIP_RequestReceivedEventArgs e) {}

        /// <summary>
        /// Handles NOTIFY method.
        /// </summary>
        /// <param name="e">Request event arguments.</param>
        internal void Notify(SIP_RequestReceivedEventArgs e) {}

        #endregion
    }
}