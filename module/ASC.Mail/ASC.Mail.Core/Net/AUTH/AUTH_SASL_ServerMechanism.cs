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

namespace ASC.Mail.Net.AUTH
{
    #region usings

    using System;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// This base class for server SASL authentication mechanisms.
    /// </summary>
    public abstract class AUTH_SASL_ServerMechanism
    {
        #region Properties

        /// <summary>
        /// Gets if user has authenticated sucessfully.
        /// </summary>
        public abstract bool IsAuthenticated { get; }

        /// <summary>
        /// Gets if the authentication exchange has completed.
        /// </summary>
        public abstract bool IsCompleted { get; }

        /// <summary>
        /// Gets IANA-registered SASL authentication mechanism name.
        /// </summary>
        /// <remarks>The registered list is available from: http://www.iana.org/assignments/sasl-mechanisms .</remarks>
        public abstract string Name { get; }

        /// <summary>
        /// Gets if specified SASL mechanism is available only to SSL connection.
        /// </summary>
        public abstract bool RequireSSL { get; }

        /// <summary>
        /// Gets user login name.
        /// </summary>
        public abstract string UserName { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Continues authentication process.
        /// </summary>
        /// <param name="clientResponse">Client sent SASL response.</param>
        /// <returns>Retunrns challange response what must be sent to client or null if authentication has completed.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>clientRespone</b> is null reference.</exception>
        public abstract string Continue(string clientResponse);

        #endregion
    }
}