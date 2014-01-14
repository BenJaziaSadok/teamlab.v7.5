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
using ASC.Common.Web;
using ASC.Web.Core.Utility;

namespace ASC.Web.Studio.HttpHandlers
{
    public class AjaxFileUploadHandler : AbstractHttpAsyncHandler
    {
        public override void OnProcessRequest(HttpContext context)
        {
            var result = new FileUploadResult()
            {
                Success = false,
                Message = "type not found"
            };
            if (!String.IsNullOrEmpty(context.Request["type"]))
            {
                try
                {
                    var uploadHandler = (IFileUploadHandler)Activator.CreateInstance(Type.GetType(context.Request["type"], true));
                    result = uploadHandler.ProcessUpload(context);
                }
                catch { }
            }

            context.Response.StatusCode = 200;
            context.Response.Write(AjaxPro.JavaScriptSerializer.Serialize(result));
        }
    }
}
