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
    /// <summary>
    /// This is base class for asynchronous operation.
    /// </summary>
    public abstract class AsyncOP
    {
        #region Properties

        /// <summary>
        /// Gets if asynchronous operation has completed.
        /// </summary>
        public abstract bool IsCompleted { get; }

        /// <summary>
        /// Gets if operation completed synchronously.
        /// </summary>
        public abstract bool IsCompletedSynchronously { get; }

        /// <summary>
        /// Gets if this object is disposed.
        /// </summary>
        public abstract bool IsDisposed { get; }

        #endregion
    }
}