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

namespace ASC.Mail.Net.Dns.Client
{
    /// <summary>
    /// Dns server reply codes.
    /// </summary>
    public enum RCODE
    {
        /// <summary>
        /// No error condition.
        /// </summary>
        NO_ERROR = 0,

        /// <summary>
        /// Format error - The name server was unable to interpret the query.
        /// </summary>
        FORMAT_ERRROR = 1,

        /// <summary>
        /// Server failure - The name server was unable to process this query due to a problem with the name server.
        /// </summary>
        SERVER_FAILURE = 2,

        /// <summary>
        /// Name Error - Meaningful only for responses from an authoritative name server, this code signifies that the
        /// domain name referenced in the query does not exist.
        /// </summary>
        NAME_ERROR = 3,

        /// <summary>
        /// Not Implemented - The name server does not support the requested kind of query.
        /// </summary>
        NOT_IMPLEMENTED = 4,

        /// <summary>
        /// Refused - The name server refuses to perform the specified operation for policy reasons.
        /// </summary>
        REFUSED = 5,
    }
}