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
using ASC.Data.Storage;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.HttpHandlers
{
    public class FCKEditorFileUploadHandler : AbstractHttpAsyncHandler
    {
        public override void OnProcessRequest(HttpContext context)
        {
            try
            {
                var storeDomain = context.Request["esid"];
                var itemID = context.Request["iid"] ?? "";

                var file = context.Request.Files["NewFile"];

                if (file.ContentLength > SetupInfo.MaxImageUploadSize)
                {
                    SendFileUploadResponse(context, 1, true, string.Empty, string.Empty, FileSizeComment.FileImageSizeExceptionString);
                    return;
                }

                var filename = file.FileName.Replace("%", string.Empty);
                var ind = file.FileName.LastIndexOf("\\");
                if (ind >= 0)
                {
                    filename = file.FileName.Substring(ind + 1);
                }
                var folderID = CommonControlsConfigurer.FCKAddTempUploads(storeDomain, filename, itemID);

                var store = StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "fckuploaders");
                var saveUri = store.Save(storeDomain, folderID + "/" + filename, file.InputStream).ToString();

                SendFileUploadResponse(context, 0, true, saveUri, filename, string.Empty);
            }
            catch (Exception e)
            {
                SendFileUploadResponse(context, 1, true, string.Empty, string.Empty, e.Message.HtmlEncode());
            }
        }


        private void SendFileUploadResponse(HttpContext context, int errorNumber, bool isQuickUpload, string fileUrl, string fileName, string customMsg)
        {
            context.Response.Clear();

            context.Response.Write("<script type=\"text/javascript\">");
            // Minified version of the document.domain automatic fix script.
            // The original script can be found at _dev/domain_fix_template.js
            context.Response.Write(@"(function(){var d=document.domain; while (true){try{var A=window.top.opener.document.domain;break;}catch(e) {};d=d.replace(/.*?(?:\.|$)/g,'');if (d.length==0) break;try{document.domain=d;}catch (e){break;}}})();");

            if (!string.IsNullOrEmpty(fileUrl))
            {
                var parts = fileUrl.Split('/');
                parts[parts.Length - 1] = HttpUtility.UrlEncode(parts[parts.Length - 1]);
                fileUrl = string.Join("/", parts);
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                fileName = HttpUtility.UrlEncode(fileName);
            }

            if (isQuickUpload)
                context.Response.Write("window.parent.OnUploadCompleted(" + errorNumber + ",'" + fileUrl.Replace("'", "\\'") + "','" + fileName.Replace("'", "\\'") + "','" + customMsg.Replace("'", "\\'") + "') ;");
            else
                context.Response.Write("window.parent.frames['frmUpload'].OnUploadCompleted(" + errorNumber + ",'" + fileName.Replace("'", "\\'") + "') ;");

            context.Response.Write("</script>");
        }
    }
}