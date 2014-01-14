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
using System.Runtime.Remoting.Messaging;
using System.Web;
using ASC.Files.Core;
using ASC.Web.Studio.Controls.FileUploader;
using ASC.Web.Studio.Controls.FileUploader.HttpModule;
using ASC.Web.Studio.Core;

namespace ASC.Web.CRM.Classes
{
    public class FileUploaderHandler : FileUploadHandler
    {
        public override FileUploadResult ProcessUpload(HttpContext context)
        {
            var fileUploadResult = new FileUploadResult();

            if (!ProgressFileUploader.HasFilesToUpload(context)) return fileUploadResult;

            var file = new ProgressFileUploader.FileToUpload(context);

            if (String.IsNullOrEmpty(file.FileName) || file.ContentLength == 0)
                throw new InvalidOperationException("Invalid file.");

            if (0 < SetupInfo.MaxUploadSize && SetupInfo.MaxUploadSize < file.ContentLength)
                throw FileSizeComment.FileSizeException;

            if (CallContext.GetData("CURRENT_ACCOUNT") == null)
                CallContext.SetData("CURRENT_ACCOUNT", new Guid(context.Request["UserID"]));


            var fileName = file.FileName.LastIndexOf('\\') != -1
                               ? file.FileName.Substring(file.FileName.LastIndexOf('\\') + 1)
                               : file.FileName;

            var document = new File
                {
                    Title = fileName,
                    FolderID = Global.DaoFactory.GetFileDao().GetRoot(),
                    ContentLength = file.ContentLength
                };

            document = Global.DaoFactory.GetFileDao().SaveFile(document, file.InputStream);

            fileUploadResult.Data = document.ID;
            fileUploadResult.FileName = document.Title;
            fileUploadResult.FileURL = document.FileDownloadUrl;


            fileUploadResult.Success = true;


            return fileUploadResult;
        }
    }
}