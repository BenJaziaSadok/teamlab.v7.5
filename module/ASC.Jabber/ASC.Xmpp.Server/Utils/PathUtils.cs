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
using System.IO;

namespace ASC.Xmpp.Server.Utils
{
	static class PathUtils
	{
        private const string DATA_DIRECTORY = "|DataDirectory|";
        private const string DATA_DIRECTORY_KEY = "DataDirectory";


        public static string GetAbsolutePath(string path)
		{
            var currDir = AppDomain.CurrentDomain.BaseDirectory;
            if (path.Trim(Path.DirectorySeparatorChar).StartsWith(DATA_DIRECTORY, StringComparison.CurrentCultureIgnoreCase))
            {
                path = path.Substring(DATA_DIRECTORY.Length).Trim(Path.DirectorySeparatorChar);
                var dataDir = (string)AppDomain.CurrentDomain.GetData(DATA_DIRECTORY_KEY) ?? currDir;
                path = Path.Combine(dataDir, path);
            }
            return Path.IsPathRooted(path) ? path : Path.Combine(currDir, path);
        }
	}
}
