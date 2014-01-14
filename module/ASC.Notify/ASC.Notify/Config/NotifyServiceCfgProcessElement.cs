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

using System.Configuration;
using System;

namespace ASC.Notify.Config
{
    class NotifyServiceCfgProcessElement : ConfigurationElement
    {
        [ConfigurationProperty("maxThreads")]
        public int MaxThreads
        {
            get { return (int)base["maxThreads"]; }
        }

        [ConfigurationProperty("bufferSize", DefaultValue = 10)]
        public int BufferSize
        {
            get { return (int)base["bufferSize"]; }
        }

        [ConfigurationProperty("maxAttempts", DefaultValue = 10)]
        public int MaxAttempts
        {
            get { return (int)base["maxAttempts"]; }
        }

        [ConfigurationProperty("attemptsInterval", DefaultValue = "0:5:0")]
        public TimeSpan AttemptsInterval
        {
            get { return (TimeSpan)base["attemptsInterval"]; }
        }
    }
}
