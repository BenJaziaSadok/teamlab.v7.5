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
    using System.Diagnostics;

    #endregion

    /// <summary>
    /// Provides data for the SysError event for servers.
    /// </summary>
    public class Error_EventArgs
    {
        #region Members

        private readonly Exception m_pException;
        private readonly StackTrace m_pStackTrace;
        private string m_Text = "";

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="stackTrace"></param>
        public Error_EventArgs(Exception x, StackTrace stackTrace)
        {
            m_pException = x;
            m_pStackTrace = stackTrace;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occured error's exception.
        /// </summary>
        public Exception Exception
        {
            get { return m_pException; }
        }

        /// <summary>
        /// Occured error's stacktrace.
        /// </summary>
        public StackTrace StackTrace
        {
            get { return m_pStackTrace; }
        }

        /// <summary>
        /// Gets comment text.
        /// </summary>
        public string Text
        {
            get { return m_Text; }
        }

        #endregion
    }
}