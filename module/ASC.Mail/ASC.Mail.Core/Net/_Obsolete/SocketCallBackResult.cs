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

namespace ASC.Mail.Net
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public delegate void SocketCallBack(SocketCallBackResult result, long count, Exception x, object tag);

    /// <summary>
    /// Asynchronous command execute result.
    /// </summary>
    public enum SocketCallBackResult
    {
        /// <summary>
        /// Operation was successfull.
        /// </summary>
        Ok,

        /// <summary>
        /// Exceeded maximum allowed size.
        /// </summary>
        LengthExceeded,

        /// <summary>
        /// Connected client closed connection.
        /// </summary>
        SocketClosed,

        /// <summary>
        /// Exception happened.
        /// </summary>
        Exception
    }
}