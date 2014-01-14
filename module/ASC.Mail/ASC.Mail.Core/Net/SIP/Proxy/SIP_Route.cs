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

    using System.Net;

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public class SIP_Route
    {
        #region Properties

        /// <summary>
        /// Gets regex match pattern.
        /// </summary>
        public string MatchPattern
        {
            get { return ""; }
        }

        /// <summary>
        /// Gets matched URI.
        /// </summary>
        public string Uri
        {
            get { return ""; }
        }

        /// <summary>
        /// Gets SIP targets for <b>Uri</b>.
        /// </summary>
        public string[] Targets
        {
            get { return null; }
        }

        /// <summary>
        /// Gets targets processing mode.
        /// </summary>
        public SIP_ForkingMode ProcessMode
        {
            get { return SIP_ForkingMode.Parallel; }
        }

        /// <summary>
        /// Gets if user needs to authenticate to use this route.
        /// </summary>
        public bool RequireAuthentication
        {
            get { return true; }
        }

        /// <summary>
        /// Gets targets credentials.
        /// </summary>
        public NetworkCredential[] Credentials
        {
            get { return null; }
        }

        #endregion
    }
}