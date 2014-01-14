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
using System.Web;
using ASC.Data.Storage;

namespace ASC.Web.Core.Utility.Skins
{
    public static class WebImageSupplier
    {
        public static string GetAbsoluteWebPath(string imgFileName)
        {
            return GetAbsoluteWebPath(imgFileName, Guid.Empty);
        }

        public static string GetAbsoluteWebPath(string imgFileName, Guid moduleID)
        {
            return GetImageAbsoluteWebPath(imgFileName, moduleID);
        }

        public static string GetImageFolderAbsoluteWebPath()
        {
            if (HttpContext.Current == null) return string.Empty;

            var currentThemePath = GetPartImageFolderRel(Guid.Empty);
            return WebPath.GetPath(currentThemePath.ToLower());
        }

        private static string GetImageAbsoluteWebPath(string fileName, Guid partID)
        {
            if (HttpContext.Current == null || string.IsNullOrEmpty(fileName)) return string.Empty;

            var filepath = GetPartImageFolderRel(partID) + "/" + fileName;
            return WebPath.GetPath(filepath.ToLower());
        }

        private static string GetPartImageFolderRel(Guid partID)
        {
            var folderName = "/skins/default/images";
            string itemFolder = null;
            if (!Guid.Empty.Equals(partID))
            {
                var product = WebItemManager.Instance[partID];
                if (product != null && product.Context != null)
                {
                    itemFolder = GetAppThemeVirtualPath(product) + "/default/images";
                }

                folderName = itemFolder ?? folderName;
            }
            return folderName.TrimStart('~').ToLowerInvariant();
        }

        private static string GetAppThemeVirtualPath(IWebItem webitem)
        {
            if (webitem == null || string.IsNullOrEmpty(webitem.StartURL))
            {
                return string.Empty;
            }

            var dir = webitem.StartURL.Contains(".") ?
                          webitem.StartURL.Substring(0, webitem.StartURL.LastIndexOf("/")) :
                          webitem.StartURL.TrimEnd('/');
            return dir + "/app_themes";
        }
    }
}