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

using System.Web;
using System.Web.UI;
using ASC.Data.Storage;
using ASC.Web.Core.Client.HttpHandlers;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Core.Client.PageExtensions
{
    public static class ControlExtensions
    {
        public static string LocalPath(string relativePath)
        {
            IProduct product = null;
            if (HttpContext.Current != null)
            {
                product = HttpContext.Current.Items["currentProduct"] as IProduct;
            }
            if (product == null)
            {
                //If not saved - get
                IModule module;
                CommonLinkUtility.GetLocationByRequest(out product, out module);
            }
            if (product != null)
            {
                relativePath = product.StartURL.TrimEnd('/') + "/" + relativePath.TrimStart('/');
            }
            return WebPath.GetPath(relativePath.TrimStart('~'));
        }

        public static string LocalPath(this Control control, string relativePath)
        {
            return LocalPath(relativePath);
        }
    }
}