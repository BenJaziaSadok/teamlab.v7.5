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
using System.Security;
using ASC.Core;
using ASC.Files.Core;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using File = ASC.Files.Core.File;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Files.Utils
{
    public static class FileUploader
    {
        public static File Exec(string folderId, string title, long contentLength, Stream data)
        {
            return Exec(folderId, title, contentLength, data, !FilesSettings.UpdateIfExist);
        }

        public static File Exec(string folderId, string title, long contentLength, Stream data, bool createNewIfExist)
        {
            if (contentLength <= 0)
                throw new Exception(FilesCommonResource.ErrorMassage_EmptyFile);

            var file = VerifyFileUpload(folderId, title, contentLength, !createNewIfExist);

            using (var dao = Global.DaoFactory.GetFileDao())
            {
                try
                {
                    file = dao.SaveFile(file, data);
                }
                catch
                {
                    if (file.Version == 1)
                        dao.DeleteFile(file.ID);

                    throw;
                }
            }

            FileMarker.MarkAsNew(file);

            if (FileConverter.EnableAsUploaded && FileConverter.MustConvert(file))
                FileConverter.ExecAsync(file, true);

            return file;
        }

        public static File VerifyFileUpload(string folderId, string fileName, bool updateIfExists)
        {
            fileName = Global.ReplaceInvalidCharsAndTruncate(fileName);

            if (Global.EnableUploadFilter && !FileUtility.ExtsUploadable.Contains(FileUtility.GetFileExtension(fileName)))
                throw new Exception(FilesCommonResource.ErrorMassage_NotSupportedFormat);

            using (var fileDao = Global.DaoFactory.GetFileDao())
            {
                var file = fileDao.GetFile(folderId, fileName);

                if (updateIfExists && CanEdit(file))
                {
                    file.Title = fileName;
                    file.ConvertedType = null;
                    file.Version++;

                    return file;
                }

                using (var folderDao = Global.DaoFactory.GetFolderDao())
                {
                    var folder = folderDao.GetFolder(folderId);

                    if (folder == null)
                        throw new Exception(FilesCommonResource.ErrorMassage_FolderNotFound);

                    if (!Global.GetFilesSecurity().CanCreate(folder))
                        throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);
                }

                return new File { FolderID = folderId, Title = fileName };
            }
        }

        public static File VerifyFileUpload(string folderId, string fileName, long fileSize, bool updateIfExists)
        {
            if (fileSize <= 0)
                throw new Exception(FilesCommonResource.ErrorMassage_EmptyFile);

            long maxUploadSize;
            using (var folderDao = Global.DaoFactory.GetFolderDao())
            {
                maxUploadSize = folderDao.GetMaxUploadSize(folderId);
            }

            if (fileSize > maxUploadSize)
                throw FileSizeComment.GetFileSizeException(maxUploadSize);

            var file = VerifyFileUpload(folderId, fileName, updateIfExists);
            file.ContentLength = fileSize;

            return file;
        }

        private static long GetMaxFileSize(bool chunkedUpload)
        {
            return chunkedUpload ? SetupInfo.MaxChunkedUploadSize : SetupInfo.MaxUploadSize;
        }

        private static bool CanEdit(File file)
        {
            return file != null && Global.GetFilesSecurity().CanEdit(file) && (file.FileStatus & FileStatus.IsEditing) != FileStatus.IsEditing;
        }

        #region chunked upload

        public static File VerifyChunkedUpload(string folderId, string fileName, long fileSize, bool updateIfExists)
        {
            long maxUploadSize;
            using (var folderDao = Global.DaoFactory.GetFolderDao())
            {
               maxUploadSize = folderDao.GetMaxUploadSize(folderId, true);
            }

            if (fileSize > maxUploadSize)
                throw FileSizeComment.GetFileSizeException(maxUploadSize);

            File file = VerifyFileUpload(folderId, fileName, updateIfExists);
            file.ContentLength = fileSize;

            return file;
        }

        public static ChunkedUploadSession InitiateUpload(string folderId, string fileId, string fileName, long contentLength)
        {
            if (string.IsNullOrEmpty(folderId))
                folderId = null;

            if (string.IsNullOrEmpty(fileId))
                fileId = null;

            var file = new File {ID = fileId, FolderID = folderId, Title = fileName, ContentLength = contentLength};

            using (var dao = Global.DaoFactory.GetFileDao())
            {
                var uploadSession = dao.CreateUploadSession(file, contentLength);
                
                uploadSession.Expired = uploadSession.Created + ChunkedUploadSessionHolder.SlidingExpiration;
                uploadSession.Location = CommonLinkUtility.GetUploadChunkLocationUrl(uploadSession.Id, contentLength > 0);
                uploadSession.TenantId = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                uploadSession.UserId = SecurityContext.CurrentAccount.ID;

                ChunkedUploadSessionHolder.StoreSession(uploadSession);

                return uploadSession;
            }
        }

        public static ChunkedUploadSession UploadChunk(string uploadId, Stream stream, long chunkLength)
        {
            var uploadSession = ChunkedUploadSessionHolder.GetSession(uploadId);
            uploadSession.Expired = DateTime.UtcNow + ChunkedUploadSessionHolder.SlidingExpiration;

            if (chunkLength <= 0)
            {
                throw new Exception(FilesCommonResource.ErrorMassage_EmptyFile);
            }

            if (chunkLength > SetupInfo.MaxUploadSize)
            {
                throw FileSizeComment.GetFileSizeException(SetupInfo.MaxUploadSize);
            }

            if (uploadSession.BytesUploaded + chunkLength > GetMaxFileSize(uploadSession))
            {
                AbortUpload(uploadSession);
                throw FileSizeComment.GetFileSizeException(GetMaxFileSize(uploadSession));
            }
            
            using (var dao = Global.DaoFactory.GetFileDao())
            {
                dao.UploadChunk(uploadSession, stream, chunkLength);
            }

            if (uploadSession.BytesUploaded == uploadSession.BytesTotal)
            {
                FileMarker.MarkAsNew(uploadSession.File);
                ChunkedUploadSessionHolder.RemoveSession(uploadSession);
            }

            return uploadSession;
        }

        public static void AbortUpload(string uploadId)
        {
            AbortUpload(ChunkedUploadSessionHolder.GetSession(uploadId));
        }

        private static void AbortUpload(ChunkedUploadSession uploadSession)
        {
            using (var dao = Global.DaoFactory.GetFileDao())
            {
                dao.AbortUploadSession(uploadSession);
            }

            ChunkedUploadSessionHolder.RemoveSession(uploadSession);
        }

        private static long GetMaxFileSize(ChunkedUploadSession uploadSession)
        {
            return GetMaxFileSize(uploadSession.BytesTotal > 0);
        }

        #endregion
    }
}