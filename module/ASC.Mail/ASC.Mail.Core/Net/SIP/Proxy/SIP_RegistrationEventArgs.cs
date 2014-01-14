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

    using System;

    #endregion

    /// <summary>
    /// This class provides data for <b>SIP_Registrar.AorRegistered</b>,<b>SIP_Registrar.AorUnregistered</b> and <b>SIP_Registrar.AorUpdated</b> event.
    /// </summary>
    public class SIP_RegistrationEventArgs : EventArgs
    {
        #region Members

        private readonly SIP_Registration m_pRegistration;

        #endregion

        #region Properties

        /// <summary>
        /// Gets SIP registration.
        /// </summary>
        public SIP_Registration Registration
        {
            get { return m_pRegistration; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="registration">SIP reggistration.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>registration</b> is null reference.</exception>
        public SIP_RegistrationEventArgs(SIP_Registration registration)
        {
            if (registration == null)
            {
                throw new ArgumentNullException("registration");
            }

            m_pRegistration = registration;
        }

        #endregion
    }
}