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
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using ASC.Core;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Core.Client.HttpHandlers;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Files.Masters.ClientScripts
{
    public class FilesConstantsResources : ClientScript
    {
        private static readonly Guid CaheId = Guid.NewGuid();

        protected override string BaseNamespace
        {
            get { return "ASC.Files.Constants"; }
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            yield return RegisterObject("URL_OAUTH_GOOGLE", Import.Google.OAuth.Location.ToLower());
            yield return RegisterObject("URL_OAUTH_BOXNET", Import.Boxnet.BoxLogin.Location.ToLower());
            yield return RegisterObject("URL_OAUTH_DROPBOX", Import.DropBox.Dropbox.Location.ToLower());
            yield return RegisterObject("URL_OAUTH_SKYDRIVE", Import.SkyDrive.SkyDriveOAuth.Location.ToLower());

            yield return RegisterObject("URL_SHARE_GOOGLE_PLUS", "https://plus.google.com/share?url={0}");
            yield return RegisterObject("URL_SHARE_TWITTER", "https://twitter.com/intent/tweet?text={0}");
            yield return RegisterObject("URL_SHARE_FACEBOOK", "http://www.facebook.com/sharer.php?s=100&p[url]={0}&p[title]={1}&p[images][0]={2}&p[summary]={3}");

            yield return RegisterObject("URL_WCFSERVICE", PathProvider.GetFileServicePath);
            yield return RegisterObject("URL_TEMPLATES_HANDLER", CommonLinkUtility.ToAbsolute("~/template.ashx") + "?id=" + PathProvider.TemplatePath + "&name=collection");

            yield return RegisterObject("USER_ID", SecurityContext.CurrentAccount.ID);
            yield return RegisterObject("USER_ADMIN", Global.IsAdministrator);
            yield return RegisterObject("YOUR_DOCS", CoreContext.Configuration.YourDocs);
            yield return RegisterObject("YOUR_DOCS_DEMO", CoreContext.Configuration.YourDocsDemo);
            yield return RegisterObject("MAX_NAME_LENGTH", Global.MaxTitle);
            yield return RegisterObject("CHUNK_UPLOAD_SIZE", SetupInfo.ChunkUploadSize);
            yield return RegisterObject("UPLOAD_FILTER", Global.EnableUploadFilter);
            yield return RegisterObject("REQUEST_CONVERT_DELAY", FileConverter.TimerConvertPeriod);
            yield return RegisterObject("ENABLE_UPLOAD_CONVERT", FileConverter.EnableAsUploaded);

            yield return RegisterObject("FOLDER_ID_MY_FILES", Global.FolderMy);
            yield return RegisterObject("FOLDER_ID_SHARE", Global.FolderShare);
            yield return RegisterObject("FOLDER_ID_COMMON_FILES", Global.FolderCommon);
            yield return RegisterObject("FOLDER_ID_PROJECT", Global.FolderProjects);
            yield return RegisterObject("FOLDER_ID_TRASH", Global.FolderTrash);

            yield return RegisterObject("ShareLinkId", FileConstant.ShareLinkId);

            yield return RegisterObject("AceStatusEnum", new object());
            yield return RegisterObject("AceStatusEnum.None", FileShare.None);
            yield return RegisterObject("AceStatusEnum.ReadWrite", FileShare.ReadWrite);
            yield return RegisterObject("AceStatusEnum.Read", FileShare.Read);
            yield return RegisterObject("AceStatusEnum.Restrict", FileShare.Restrict);

            yield return RegisterObject("FilterType", new object());
            yield return RegisterObject("FilterType.FilesOnly", FilterType.FilesOnly);
            yield return RegisterObject("FilterType.FoldersOnly", FilterType.FoldersOnly);
            yield return RegisterObject("FilterType.DocumentsOnly", FilterType.DocumentsOnly);
            yield return RegisterObject("FilterType.PresentationsOnly", FilterType.PresentationsOnly);
            yield return RegisterObject("FilterType.SpreadsheetsOnly", FilterType.SpreadsheetsOnly);
            yield return RegisterObject("FilterType.ImagesOnly", FilterType.ImagesOnly);
        }

        protected override string GetCacheHash()
        {
            return
                CaheId //cache forever until restart (cfg)
                + CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).LastModified.ToString(CultureInfo.InvariantCulture) //change users data (is admin)
                + CoreContext.TenantManager.GetCurrentTenant().LastModified.ToString(CultureInfo.InvariantCulture) //change tenant data (domain)
                + SecurityContext.IsAuthenticated
                ;
        }
    }
}