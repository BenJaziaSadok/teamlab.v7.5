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
using System.Web.UI;
using ASC.Web.Studio.Core;
using ASC.Web.UserControls.Wiki.Handlers;
using ASC.Data.Storage;
using System.IO;
using ASC.Core;
using ASC.Web.UserControls.Wiki.Resources;

namespace ASC.Web.UserControls.Wiki.UC
{
    public class FileUploadResult
    {
        public FileUploadResult()
        {
            ErrorText = string.Empty;
            WebPath = string.Empty;
            LocalPath = string.Empty;
        }

        public string WebPath { get; set; }
        public string LocalPath { get; set; }
        public string ErrorText { get; set; }
    }

    public partial class FileUploader : Page
    {
        public static long MaxUploadSize
        {
            get { return SetupInfo.MaxUploadSize; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
            var result = new FileUploadResult();
            if (Request.Files.Count > 0 && !string.IsNullOrEmpty(Request["hfUserID"]))
            {
                try
                {
                    //string uploadedUserName;
                    var content = new byte[Request.Files[0].ContentLength];

                    if (content.Length > MaxUploadSize && MaxUploadSize > 0)
                    {
                        result.ErrorText = WikiUCResource.wikiErrorFileSizeLimitText;
                    }
                    else
                    {
                        Request.Files[0].InputStream.Read(content, 0, Request.Files[0].ContentLength);
                        string localPath;
                        result.WebPath = TempFileContentSave(content, out localPath);
                        result.LocalPath = localPath;
                    }


                    Response.StatusCode = 200;
                    Response.Write(AjaxPro.JavaScriptSerializer.Serialize(result));
                }
                catch (Exception)
                {
                }
            }
            Response.End();
        }

        private static string TempFileContentSave(byte[] fileContent, out string filaLocation)
        {
            var tenantId = CoreContext.TenantManager.GetCurrentTenant().TenantId.ToString();
            var storage = StorageFactory.GetStorage(tenantId, WikiSection.Section.DataStorage.ModuleName);
            string result;

            using (var ms = new MemoryStream(fileContent))
            {
                result = storage.SaveTemp(WikiSection.Section.DataStorage.TempDomain, out filaLocation, ms).ToString();
            }

            return result;
        }
    }
}