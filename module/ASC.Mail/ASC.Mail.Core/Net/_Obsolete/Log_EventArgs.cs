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
    /// Provides data for the SessionLog event.
    /// </summary>
    public class Log_EventArgs
    {
        #region Members

        private readonly bool m_FirstLogPart = true;
        private readonly bool m_LastLogPart;
        private readonly SocketLogger m_pLoggger;

        #endregion

        #region Properties

        /// <summary>
        /// Gets log text.
        /// </summary>
        public string LogText
        {
            get { return SocketLogger.LogEntriesToString(m_pLoggger, m_FirstLogPart, m_LastLogPart); }
        }

        /// <summary>
        /// Gets logger.
        /// </summary>
        public SocketLogger Logger
        {
            get { return m_pLoggger; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger">Socket logger.</param>
        /// <param name="firstLogPart">Specifies if first log part of multipart log.</param>
        /// <param name="lastLogPart">Specifies if last log part (logging ended).</param>
        public Log_EventArgs(SocketLogger logger, bool firstLogPart, bool lastLogPart)
        {
            m_pLoggger = logger;
            m_FirstLogPart = firstLogPart;
            m_LastLogPart = lastLogPart;
        }

        #endregion
    }
}