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

namespace ASC.Xmpp.Core.IO.Compression
{
    /// <summary>
    ///   This class stores the pending output of the Deflater. author of the original java version : Jochen Hoenicke
    /// </summary>
    public class DeflaterPending : PendingBuffer
    {
        #region Constructor

        /// <summary>
        ///   Construct instance with default buffer size
        /// </summary>
        public DeflaterPending() : base(DeflaterConstants.PENDING_BUF_SIZE)
        {
        }

        #endregion
    }
}