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
    /// <summary>
    /// This class specifies SIP UA registration state.
    /// </summary>
    public enum SIP_UA_RegistrationState
    {
        /// <summary>
        /// Registration is currently registering.
        /// </summary>
        Registering,

        /// <summary>
        /// Registration is active.
        /// </summary>
        Registered,

        /// <summary>
        /// Registration is not registered to registrar server.
        /// </summary>
        Unregistered,

        /// <summary>
        /// Registering has failed.
        /// </summary>
        Error,

        /// <summary>
        /// Registration has disposed.
        /// </summary>
        Disposed
    }
}