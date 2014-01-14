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
using ASC.Common.Utils;
using ASC.Files.Core;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Utility;
using File = ASC.Files.Core.File;
using FileShare = ASC.Files.Core.Security.FileShare;

namespace ASC.Web.Files.Utils
{
    public static class FileShareLink
    {
        public static string GetLink(File file)
        {
            var url = file.ViewUrl;

            if (FileUtility.CanWebView(file.Title)
                && TenantExtra.GetTenantQuota().DocsEdition)
                url = CommonLinkUtility.GetFileWebEditorUrl(file.ID);

            var linkParams = Signature.Create(file.ID.ToString(), Global.GetDocDbKey());
            url += "&" + CommonLinkUtility.DocShareKey + "=" + HttpUtility.UrlEncode(linkParams);

            return CommonLinkUtility.GetFullAbsolutePath(url);
        }

        public static string Parse(string key)
        {
            return Signature.Read<string>(key ?? String.Empty, Global.GetDocDbKey());
        }

        public static bool Check(string key, bool checkRead, IFileDao fileDao, out File file)
        {
            var share = Check(key, fileDao, out file);
            return (!checkRead && share == FileShare.ReadWrite) || (checkRead && share <= FileShare.Read);
        }

        public static FileShare Check(string key, IFileDao fileDao, out File file)
        {
            file = null;
            if (string.IsNullOrEmpty(key)) return FileShare.Restrict;
            var fileId = Parse(key);
            file = fileDao.GetFile(fileId);
            if (file == null) return FileShare.Restrict;

            var filesSecurity = Global.GetFilesSecurity();
            if (filesSecurity.CanEdit(file, FileConstant.ShareLinkId)) return FileShare.ReadWrite;
            if (filesSecurity.CanRead(file, FileConstant.ShareLinkId)) return FileShare.Read;
            return FileShare.Restrict;
        }
    }
}