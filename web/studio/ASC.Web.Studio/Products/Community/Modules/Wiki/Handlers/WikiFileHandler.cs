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
using System.Net;
using System.Web;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Core;

namespace ASC.Web.UserControls.Wiki.Handlers
{
    public class WikiFileHandler : IHttpHandler
    {
        public static string ImageExtentions = ".png.jpg.bmp.gif";


        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (!SecurityContext.IsAuthenticated)
            {
                if (!SecurityContext.AuthenticateMe(CookiesManager.GetCookies(CookiesType.AuthKey)))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Response.End();
                    return;
                }
            }            

            context.Response.Clear();
            if (string.IsNullOrEmpty(context.Request["file"]))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.End();
                return;
            }

            var file = new WikiEngine().GetFile(context.Request["file"]);
            if (file == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.End();
                return;
            }
            if (string.IsNullOrEmpty(file.FileLocation))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.End();
                return;
            }

            var storage = StorageFactory.GetStorage(CoreContext.TenantManager.GetCurrentTenant().TenantId.ToString(), WikiSection.Section.DataStorage.ModuleName);
            context.Response.Redirect(storage.GetUri(WikiSection.Section.DataStorage.DefaultDomain, file.FileLocation).OriginalString);
        }
    }
}
