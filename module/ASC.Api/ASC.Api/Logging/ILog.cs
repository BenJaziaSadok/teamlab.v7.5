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

namespace ASC.Api.Logging
{
    public interface ILog
    {
        void Fatal(Exception error, string format, params object[] args);
        void Error(Exception error, string format, params object[] args);
        void Warn(string format, params object[] args);
        void Info(string format, params object[] args);
        void Debug(string format, params object[] args);

        bool IsDebugEnabled { get; }
    }
}