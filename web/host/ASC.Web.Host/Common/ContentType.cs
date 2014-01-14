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

namespace ASC.Web.Host.Common
{
    using System.IO;
    using Microsoft.Win32;

    class ContentType
    {
        internal static string GetContentType(string fileName)
        {
            string mime = "application/octetstream";
            string ext = Path.GetExtension(fileName).ToLower();
            RegistryKey rk = Registry.ClassesRoot.OpenSubKey(ext);
            if (rk != null && rk.GetValue("Content Type") != null)
            {
                mime = rk.GetValue("Content Type").ToString();
            }
            return (string.Format("Content-Type: {0}\r\n", mime));
        }
    }
}