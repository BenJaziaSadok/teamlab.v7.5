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
    /// This class provides data for BeginWriteCallback delegate.
    /// </summary>
    public class Write_EventArgs
    {
        #region Members

        private readonly Exception m_pException;

        #endregion

        #region Properties

        /// <summary>
        /// Gets exception happened during write or null if operation was successfull.
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
        /// <param name="exception">Exception happened during write or null if operation was successfull.</param>
        internal Write_EventArgs(Exception exception)
        {
            m_pException = exception;
        }

        #endregion
    }
}