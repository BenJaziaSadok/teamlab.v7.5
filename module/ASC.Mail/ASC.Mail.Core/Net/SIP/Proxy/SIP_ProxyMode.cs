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
    /// Specifies SIP proxy mode.
    /// <example>
    /// All flags may be combined, except Stateless,Statefull,B2BUA.
    /// For example: (Stateless | Statefull) not allowed, but (Registrar | Presence | Statefull) is allowed.
    /// </example>
    /// </summary>
    [Flags]
    public enum SIP_ProxyMode
    {
        /// <summary>
        /// Proxy implements SIP registrar.
        /// </summary>
        Registrar = 1,

        /// <summary>
        /// Proxy implements SIP presence server.
        /// </summary>
        Presence = 2,

        /// <summary>
        /// Proxy runs in stateless mode.
        /// </summary>
        Stateless = 4,

        /// <summary>
        /// Proxy runs in statefull mode.
        /// </summary>
        Statefull = 8,

        /// <summary>
        /// Proxy runs in B2BUA(back to back user agent) mode.
        /// </summary>
        B2BUA = 16,
    }
}