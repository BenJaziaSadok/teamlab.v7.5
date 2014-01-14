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

namespace ASC.Mail.Net.Log
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// This class provides data for <b>Logger.WriteLog</b> event.
    /// </summary>
    public class WriteLogEventArgs : EventArgs
    {
        #region Members

        private readonly LogEntry m_pLogEntry;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logEntry">New log entry.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>logEntry</b> is null.</exception>
        public WriteLogEventArgs(LogEntry logEntry)
        {
            if (logEntry == null)
            {
                throw new ArgumentNullException("logEntry");
            }

            m_pLogEntry = logEntry;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets new log entry.
        /// </summary>
        public LogEntry LogEntry
        {
            get { return m_pLogEntry; }
        }

        #endregion
    }
}