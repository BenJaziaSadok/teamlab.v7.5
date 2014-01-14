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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Web;
using ASC.Data.Storage;

namespace ASC.Web.Core.Utility.Skins
{
    [Serializable]
    [DataContract]
    public class WebSkin
    {
        public static string BaseCSSFileAbsoluteWebPath
        {
            get { return WebPath.GetPath("/skins/default/common_style.css"); }
        }


        private static readonly HashSet<string> BaseCultureCss = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        public static bool HasCurrentCultureCssFile
        {
            get { return BaseCultureCss.Contains(CultureInfo.CurrentCulture.Name); }
        }

        static WebSkin()
        {
            if (HttpContext.Current == null) return;

            try
            {
                var dir = HttpContext.Current.Server.MapPath("~/skins/default/");
                if (!Directory.Exists(dir)) return;

                foreach (var f in Directory.GetFiles(dir, "common_style.*.css"))
                {
                    BaseCultureCss.Add(Path.GetFileName(f).Split('.')[1]);
                }
            }
            catch
            {
            }
        }
    }
}