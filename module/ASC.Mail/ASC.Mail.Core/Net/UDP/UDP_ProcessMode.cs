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

namespace ASC.Mail.Net.UDP
{
    /// <summary>
    /// This enum specified UDP server packets process mode.
    /// </summary>
    public enum UDP_ProcessMode
    {
        /// <summary>
        /// UDP packets processed one by one in their receive order.
        /// </summary>
        Sequential = 1,

        /// <summary>
        /// UDP packets proecesses parallel.
        /// </summary>
        Parallel = 2,
    }
}