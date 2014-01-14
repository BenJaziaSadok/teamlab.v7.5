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
    /// This class provides data for error events and methods.
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        #region Members

        private readonly Exception m_pException;

        #endregion

        #region Properties

        /// <summary>
        /// Gets exception.
        /// </summary>
        public Exception Exception
        {
            get { return m_pException; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>exception</b> is null reference value.</exception>
        public ExceptionEventArgs(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            m_pException = exception;
        }

        #endregion
    }
}