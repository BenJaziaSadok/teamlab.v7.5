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
using System.Configuration;
using System.IO;
using System.Web.Configuration;

namespace ASC.Data.Backup
{
    internal static class ConfigurationHelper
    {
        public static Configuration OpenConfiguration(string path)
        {
            if (path.Contains("\\") && !Uri.IsWellFormedUriString(path, UriKind.Relative))
            {
                if (!path.EndsWith(".config", StringComparison.InvariantCultureIgnoreCase))
                    path = Path.Combine(path, "web.config");

                var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = path };
                return ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            }
            return WebConfigurationManager.OpenWebConfiguration(path);
        }
    }
}
