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
using ASC.Core;
using ASC.Files.Core;
using ASC.Notify;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Utility;
using ASC.Files.Core.Security;

namespace ASC.Web.Files.Services.NotifyService
{
    public static class NotifyClient
    {
        public static INotifyClient Instance { get; private set; }

        static NotifyClient()
        {
            Instance = WorkContext.NotifyContext.NotifyService.RegisterClient(NotifySource.Instance);
        }

        public static void SendLinkToEmail(File file, String url, String message, List<String> addressRecipients)
        {
            if (file == null || String.IsNullOrEmpty(url))
                throw new ArgumentException();

            var recipients = addressRecipients
                .ConvertAll(address => (IRecipient) (new DirectRecipient(Guid.NewGuid().ToString(), String.Empty, new[] {address}, false)));

            Instance.SendNoticeToAsync(
                NotifyConstants.Event_LinkToEmail,
                null,
                recipients.ToArray(),
                false,
                new TagValue(NotifyConstants.Tag_DocumentTitle, file.Title),
                new TagValue(NotifyConstants.Tag_DocumentUrl, CommonLinkUtility.GetFullAbsolutePath(url)),
                new TagValue(NotifyConstants.Tag_AccessRights, GetAccessString(file.Access)),
                new TagValue(NotifyConstants.Tag_Message, message.HtmlEncode())
                );
        }

        public static void SendShareNotice(FileEntry fileEntry, List<Guid> recipientsId, string message)
        {
            if (fileEntry == null || recipientsId.Count == 0) return;

            using (var folderDao = Global.DaoFactory.GetFolderDao())
            {
                if (fileEntry is File
                    && folderDao.GetFolder(((File) fileEntry).FolderID) == null) return;

                String url;
                if (fileEntry is File)
                {
                    url = CommonLinkUtility.GetFileWebPreviewUrl(fileEntry.Title, fileEntry.ID);
                }
                else
                    url = PathProvider.GetFolderUrl(((Folder) fileEntry));

                var recipientsProvider = NotifySource.Instance.GetRecipientsProvider();

                foreach (var recipientId in recipientsId)
                {
                    var u = CoreContext.UserManager.GetUsers(recipientId);
                    var culture = string.IsNullOrEmpty(u.CultureName)
                                      ? CoreContext.TenantManager.GetCurrentTenant().GetCulture()
                                      : CultureInfo.GetCultureInfo(u.CultureName);

                    var aceString = GetAccessString(fileEntry.Access, culture);
                    var recipient = recipientsProvider.GetRecipient(recipientId.ToString());

                    Instance.SendNoticeAsync(
                        fileEntry is File ? NotifyConstants.Event_ShareDocument : NotifyConstants.Event_ShareFolder,
                        fileEntry.UniqID,
                        recipient,
                        true,
                        new TagValue(NotifyConstants.Tag_DocumentTitle, fileEntry.Title),
                        new TagValue(NotifyConstants.Tag_FolderID, fileEntry.ID),
                        new TagValue(NotifyConstants.Tag_DocumentUrl, CommonLinkUtility.GetFullAbsolutePath(url)),
                        new TagValue(NotifyConstants.Tag_AccessRights, aceString),
                        new TagValue(NotifyConstants.Tag_Message, message.HtmlEncode())
                        );
                }
            }
        }

        private static String GetAccessString(FileShare fileShare)
        {
            return GetAccessString(fileShare, CoreContext.TenantManager.GetCurrentTenant().GetCulture());
        }

        private static String GetAccessString(FileShare fileShare, CultureInfo cultureInfo)
        {
            switch (fileShare)
            {
                case FileShare.Read:
                    return Resources.FilesCommonResource.ResourceManager.GetString("AceStatusEnum_Read", cultureInfo);
                case FileShare.ReadWrite:
                    return Resources.FilesCommonResource.ResourceManager.GetString("AceStatusEnum_ReadWrite", cultureInfo);
                default:
                    return String.Empty;
            }
        }
    }
}