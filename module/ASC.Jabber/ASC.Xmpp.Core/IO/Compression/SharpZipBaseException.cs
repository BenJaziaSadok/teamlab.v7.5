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

using System;

namespace ASC.Xmpp.Core.IO.Compression
{

    #region usings

    #endregion

    /// <summary>
    ///   SharpZipBaseException is the base exception class for the SharpZipLibrary. All library exceptions are derived from this.
    /// </summary>
    public class SharpZipBaseException : ApplicationException
    {
        #region Constructor

        /// <summary>
        ///   Initializes a new instance of the SharpZipLibraryException class.
        /// </summary>
        public SharpZipBaseException()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the SharpZipLibraryException class with a specified error message.
        /// </summary>
        /// <param name="msg"> </param>
        public SharpZipBaseException(string msg) : base(msg)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the SharpZipLibraryException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message"> Error message string </param>
        /// <param name="innerException"> The inner exception </param>
        public SharpZipBaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion
    }
}