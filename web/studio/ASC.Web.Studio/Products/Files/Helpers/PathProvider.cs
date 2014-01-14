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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Security.Cryptography;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Files.Classes
{
    public static class PathProvider
    {
        public static readonly String ProjectVirtualPath = "~/products/projects/tmdocs.aspx";

        public static readonly String TemplatePath = "/products/files/templates/";

        public static readonly String StartURL = CommonLinkUtility.FilesBaseVirtualPath;

        public static readonly String GetFileServicePath = CommonLinkUtility.ToAbsolute("~/products/files/services/wcfservice/service.svc/");

        public static string GetImagePath(string imgFileName)
        {
            return WebImageSupplier.GetAbsoluteWebPath(imgFileName, Configuration.ProductEntryPoint.ID).ToLower();
        }

        public static String GetFileStaticRelativePath(String fileName, bool forBundle)
        {
            if (forBundle)
            {
                var ext = FileUtility.GetFileExtension(fileName);
                switch (ext)
                {
                    case ".js": //Attention: Only for ResourceBundleControl
                        return VirtualPathUtility.ToAbsolute("~/products/files/js/" + fileName);
                    case ".ascx":
                        return CommonLinkUtility.ToAbsolute("~/products/files/controls/" + fileName).ToLowerInvariant();
                    case ".css": //Attention: Only for ResourceBundleControl
                        return VirtualPathUtility.ToAbsolute("~/products/files/app_themes/default/" + fileName);
                }

                return fileName;
            }

            return GetFileStaticRelativePath(fileName);
        }

        public static String GetFileStaticRelativePath(String fileName)
        {
            var ext = FileUtility.GetFileExtension(fileName);
            switch (ext)
            {
                case ".js":
                    return WebPath.GetPath("/products/files/js/" + fileName).ToLowerInvariant();
                case ".ascx":
                    return CommonLinkUtility.ToAbsolute("~/products/files/controls/" + fileName).ToLowerInvariant();
                case ".css":
                    return WebPath.GetPath("/products/files/app_themes/default/" + fileName).ToLowerInvariant();
            }
            return fileName;
        }

        public static String GetFolderUrl(Folder folder)
        {
            if (folder == null) throw new ArgumentNullException(FilesCommonResource.ErrorMassage_FolderNotFound);

            using (var folderDao = Global.DaoFactory.GetFolderDao())
            {
                switch (folder.RootFolderType)
                {
                    case FolderType.BUNCH:
                        var path = folderDao.GetBunchObjectID(folder.RootFolderId);

                        var projectID = path.Split('/').Last();

                        if (String.IsNullOrEmpty(projectID)) return String.Empty;

                        return String.Format("{0}?{1}={2}#{3}", CommonLinkUtility.ToAbsolute(ProjectVirtualPath),
                                             UrlConstant.ProjectId, projectID, folder.ID);
                    default:
                        return CommonLinkUtility.FilesBaseAbsolutePath + "#" + HttpUtility.UrlPathEncode(folder.ID.ToString());
                }
            }
        }

        public static String GetFolderUrl(object folderId)
        {
            using (var folderDao = Global.DaoFactory.GetFolderDao())
            {
                var folder = folderDao.GetFolder(folderId);

                return GetFolderUrl(folder);
            }
        }

        public static string GetFileStreamUrl(File file)
        {
            string fileUri = null;
            if (!DocumentServiceHelper.HaveExternalIP())
                fileUri = DocumentServiceHelper.GetExternalUri(file);

            if (!string.IsNullOrEmpty(fileUri))
                return fileUri;

            using (var fileDao = Global.DaoFactory.GetFileDao())
            {
                if (fileDao.IsSupportedPreSignedUri(file) && string.IsNullOrEmpty(WebConfigurationManager.AppSettings["files.internal-uri"]))
                {
                    int validateTimespan;
                    int.TryParse(WebConfigurationManager.AppSettings["files.stream-url-minute"], out validateTimespan);
                    if (validateTimespan <= 0) validateTimespan = 5;
                    return fileDao.GetPreSignedUri(file, TimeSpan.FromMinutes(validateTimespan)).ToString();
                }
            }

            //NOTE: Always build path to handler!
            var uriBuilder = new UriBuilder(CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.FileHandlerPath));
            if (uriBuilder.Uri.IsLoopback)
            {
                uriBuilder.Host = Dns.GetHostName();
            }
            var query = uriBuilder.Query;
            query += CommonLinkUtility.Action + "=stream&";
            query += CommonLinkUtility.FileId + "=" + HttpUtility.UrlEncode(file.ID.ToString()) + "&";
            query += CommonLinkUtility.Version + "=" + file.Version + "&";
            query += CommonLinkUtility.AuthKey + "=" + EmailValidationKeyProvider.GetEmailKey(file.ID + file.Version.ToString(CultureInfo.InvariantCulture));

            return uriBuilder.Uri + "?" + query;
        }
    }
}