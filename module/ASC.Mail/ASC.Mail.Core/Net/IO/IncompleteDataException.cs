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

namespace ASC.Mail.Net.IO
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// The exception that is thrown when incomplete data received.
    /// For example for ReadPeriodTerminated() method reaches end of stream before getting period terminator.
    /// </summary>
    public class IncompleteDataException : Exception
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IncompleteDataException() {}

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        public IncompleteDataException(string message) : base(message) {}

        #endregion
    }
}