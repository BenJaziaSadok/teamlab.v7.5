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

namespace ASC.Mail.Net.SIP.UA
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// This class provides data for <b>SIP_UA.IncomingCall</b> event.
    /// </summary>
    public class SIP_UA_Call_EventArgs : EventArgs
    {
        #region Members

        private readonly SIP_UA_Call m_pCall;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="call">SIP UA call.</param>
        /// <exception cref="ArgumentNullException">Is called when <b>call</b> is null reference.</exception>
        public SIP_UA_Call_EventArgs(SIP_UA_Call call)
        {
            if (call == null)
            {
                throw new ArgumentNullException("call");
            }

            m_pCall = call;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets call.
        /// </summary>
        public SIP_UA_Call Call
        {
            get { return m_pCall; }
        }

        #endregion
    }
}