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
using System.Reflection;
using System.Text;
using System.Web;
using ASC.Core;

namespace ASC.Web.Studio.Core
{
    public  class DebugInfo
    {
        private static readonly DateTime compileDateTime;


        public static bool ShowDebugInfo
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public static string DebugString
        {
            get
            {
                if (HttpContext.Current == null) return "Unknown (HttpContext is null)";

                var pathToRoot = HttpContext.Current.Server.MapPath("~/");

                var pathToFile = String.Concat(pathToRoot, "change.log");

                if (!System.IO.File.Exists(pathToFile))
                    return "File 'change.log' is not exists";

                var fileContent = File.ReadAllText(pathToFile, UnicodeEncoding.Default);

                fileContent = fileContent.Replace("{BuildDate}", compileDateTime.ToString("yyyy-MM-dd hh:mm"));
                fileContent = fileContent.Replace("{User}", SecurityContext.CurrentAccount.ToString());
                fileContent = fileContent.Replace("{UserAgent}", HttpContext.Current.Request.UserAgent);
                fileContent = fileContent.Replace("{Url}", HttpContext.Current.Request.Url.ToString());
                fileContent = fileContent.Replace("{RewritenUrl}", HttpContext.Current.Request.GetUrlRewriter().ToString());

                return fileContent;
            }
        }


        static DebugInfo()
        {
            try
            {
                const int PE_HEADER_OFFSET = 60;
                const int LINKER_TIMESTAMP_OFFSET = 8;
                var b = new byte[2048];
                using (var s = new FileStream(Assembly.GetCallingAssembly().Location, FileMode.Open, FileAccess.Read))
                {
                    s.Read(b, 0, 2048);
                }
                var i = BitConverter.ToInt32(b, PE_HEADER_OFFSET);
                var secondsSince1970 = BitConverter.ToInt32(b, i + LINKER_TIMESTAMP_OFFSET);
                compileDateTime = new DateTime(1970, 1, 1).AddSeconds(secondsSince1970);
            }
            catch { }
        }
    }
}
