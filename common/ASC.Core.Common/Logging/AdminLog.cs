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

using log4net;

namespace ASC.Core.Common.Logging
{
    /// <summary>
    /// Allows to manage and write Info to administator log
    /// </summary>
    public static class AdminLog
    {
        private static readonly string loggerName = "AdminLog";
        private static ILog log;

        /// <summary>
        /// Write a message to administator log
        /// </summary>
        /// <param name="action">message to write</param>
        public static void PostAction(object message)
        {
            GetLogger().Info(message);
        }

        public static void PostAction(string format, params object[] args)
        {
            GetLogger().Info(string.Format(new AdminLogFormatter(), format, args));
        }

        private static ILog GetLogger()
        {
            return log ?? (log = LogManager.GetLogger(loggerName));
        }
    }
}
