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

namespace ASC.Web.Host.Config
{
    class RequestConfiguration
    {
        public static string[] DefaultFileNames = new[]
        {
            "default.aspx", "default.htm", "default.html", "index.htm", "index.html"
        };

        public static string[] RestrictedDirs = new[]
        {
            "/bin", "/_private_folder", "/app_browsers", "/app_code", "/app_data", "/app_localresources", "/app_globalresources", "/app_webreferences"
        };
    }
}