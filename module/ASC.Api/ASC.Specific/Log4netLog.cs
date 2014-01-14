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
using ASC.Api.Logging;

namespace ASC.Specific
{
    public class Log4NetLog:ILog
    {
        private readonly log4net.ILog _loger;

        public Log4NetLog()
        {
            log4net.Config.XmlConfigurator.Configure();
            _loger = log4net.LogManager.GetLogger("ASC.Api");
        }

        public void Fatal(Exception error, string format, params object[] args)
        {
            _loger.Fatal(Format(format, args),error);
        }

        private static string Format(string format, object[] args)
        {
            if (args != null && args.Length > 0)
            {
                return string.Format(format, args);
            }
            return format;
        }

        public void Error(Exception error, string format, params object[] args)
        {
            _loger.Error(Format(format, args), error);
        }

        public void Warn(string format, params object[] args)
        {
            _loger.Warn(Format(format, args));
        }

        public void Info(string format, params object[] args)
        {
            _loger.Info(Format(format, args));
        }

        public void Debug(string format, params object[] args)
        {
            _loger.Debug(Format(format, args));
        }

        public bool IsDebugEnabled
        {
            get { return _loger.IsDebugEnabled; }
        }
    }
}