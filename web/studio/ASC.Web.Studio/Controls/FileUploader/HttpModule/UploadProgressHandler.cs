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
using AjaxPro;
using ASC.Common.Web;

namespace ASC.Web.Studio.Controls.FileUploader.HttpModule
{
    public abstract class FileUploadHandler
    {
        public class FileUploadResult
        {
            public bool Success { get; set; }
            public string FileName { get; set; }
            public string FileURL { get; set; }
            public object Data { get; set; }
            public string Message { get; set; }
        }

        public virtual string GetFileName(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;

            var ind = path.LastIndexOf('\\');
            return ind != -1 ? path.Substring(ind + 1) : path;
        }

        public abstract FileUploadResult ProcessUpload(HttpContext context);
    }

    public class UploadProgressHandler : AbstractHttpAsyncHandler
    {
        public override void OnProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request["submit"]))
            {
                FileUploadHandler.FileUploadResult result;
                try
                {
                    var uploadHandler = (FileUploadHandler)Activator.CreateInstance(Type.GetType(context.Request["submit"], true));
                    result = uploadHandler.ProcessUpload(context);
                }
                catch (Exception ex)
                {
                    result = new FileUploadHandler.FileUploadResult
                        {
                            Success = false,
                            Message = ex.Message.HtmlEncode(),
                        };
                }

                //NOTE: Don't set content type. ie cant parse it
                context.Response.StatusCode = 200;
                context.Response.Write(JavaScriptSerializer.Serialize(result));
            }
            else
            {
                context.Response.ContentType = "application/json";
                var id = context.Request.QueryString[UploadProgressStatistic.UploadIdField];
                var us = UploadProgressStatistic.GetStatistic(id);

                if (!string.IsNullOrEmpty(context.Request["limit"]))
                {
                    var limit = long.Parse(context.Request["limit"]);
                    if (us.TotalBytes > limit)
                    {
                        us.ReturnCode = 1;
                        us.IsFinished = true;
                    }
                }

                context.Response.Write(us.ToJson());
            }
        }
    }
}