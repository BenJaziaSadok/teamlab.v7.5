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

#region Import

using System;
using System.Web;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.CRM
{
    public  static class PathProvider
    {

        public static readonly String BaseVirtualPath = "~/products/crm/";
        public static readonly String BaseAbsolutePath = CommonLinkUtility.ToAbsolute(BaseVirtualPath).ToLower();

        public static String StartURL()
        {
            return "~/products/crm/";
        }

        public static string BaseSiteUrl
        {
            get
            {
                HttpContext context = HttpContext.Current;
                string baseUrl = context.Request.GetUrlRewriter().Scheme + "://" + context.Request.GetUrlRewriter().Authority + context.Request.ApplicationPath.TrimEnd('/') + '/';
                return baseUrl;
            }
        }

        public static string GetVirtualPath(string physicalPath)
        {
            string rootpath = HttpContext.Current.Server.MapPath("~/");
            physicalPath = physicalPath.Replace(rootpath, "");
            physicalPath = physicalPath.Replace("\\", "/");

            return "~/" + physicalPath;
        }

        public static String GetFileStaticRelativePath(String fileName)
        {
            if (fileName.EndsWith(".js"))
            {
                //Attention: Only for ResourceBundleControl
                return VirtualPathUtility.ToAbsolute("~/products/crm/js/" + fileName);
            }
            if (fileName.EndsWith(".ascx"))
            {
                return VirtualPathUtility.ToAbsolute("~/products/crm/controls/" + fileName);
            }
            if (fileName.EndsWith(".css"))
            {
                //Attention: Only for ResourceBundleControl
                return VirtualPathUtility.ToAbsolute("~/products/crm/app_themes/default/css/" + fileName);
            }
            if (fileName.EndsWith(".png") || fileName.EndsWith(".gif") || fileName.EndsWith(".jpg"))
            {
                return WebPath.GetPath("/products/crm/app_themes/default/images/" + fileName);
            }

            return fileName;
        }

    }
}