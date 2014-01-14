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
using System.Linq;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Utils;

namespace ASC.Web.Files.Services.WCFService.FileOperations
{
    internal class FileDeleteOperation : FileOperation
    {
        private object _trashId;
        private bool ignoreException;

        protected override FileOperationType OperationType
        {
            get { return FileOperationType.Delete; }
        }

        public FileDeleteOperation(Tenant tenant, List<object> folders, List<object> files)
            : base(tenant, folders, files)
        {
            ignoreException = false;
        }

        public FileDeleteOperation(Tenant tenant, List<object> folders, List<object> files, bool ignoreException)
            : base(tenant, folders, files)
        {
            this.ignoreException = ignoreException;
        }

        protected override void Do()
        {
            _trashId = FolderDao.GetFolderIDTrash(true);
            Folder root = null;
            if (0 < Folders.Count)
            {
                root = FolderDao.GetRootFolder(Folders[0]);
            }
            else if (0 < Files.Count)
            {
                root = FolderDao.GetRootFolderByFile(Files[0]);
            }
            if (root != null)
            {
                Status += string.Format("folder_{0}{1}", root.ID, SplitCharacter);
            }

            DeleteFiles(Files);
            DeleteFolders(Folders);
        }

        private void DeleteFolders(List<object> folderIds)
        {
            if (folderIds.Count == 0) return;

            foreach (var folderId in folderIds)
            {
                if (Canceled) return;

                var folder = FolderDao.GetFolder(folderId);
                if (folder == null)
                {
                    Error = FilesCommonResource.ErrorMassage_FolderNotFound;
                }
                else if (!ignoreException && !FilesSecurity.CanDelete(folder))
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder;
                }
                else
                {
                    FileMarker.RemoveMarkAsNewForAll(folder);
                    if (FolderDao.UseTrashForRemove(folder))
                    {
                        var files = FolderDao.GetFiles(folder.ID, true);
                        if (!ignoreException && files.Exists(fid => ((new File {ID = fid, FileStatus = FileStatus.None}).FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing))
                        {
                            Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteEditingFolder;
                        }
                        else
                        {
                            FolderDao.MoveFolder(folder.ID, _trashId);
                            ProcessedFolder(folderId);
                        }
                    }
                    else
                    {
                        if (FolderDao.UseRecursiveOperation(folder.ID, null))
                        {
                            DeleteFiles(FolderDao.GetFiles(folder.ID, false));
                            DeleteFolders(FolderDao.GetFolders(folder.ID).Select(f => f.ID).ToList());

                            if (FolderDao.GetItemsCount(folder.ID, true) == 0)
                            {
                                FolderDao.DeleteFolder(folder.ID);
                                ProcessedFolder(folderId);
                            }
                        }
                        else
                        {
                            if (folder.ProviderEntry && folder.ID.Equals(folder.RootFolderId))
                            {
                                ProviderDao.RemoveProviderInfo(folder.ProviderId);
                            }
                            else
                            {
                                FolderDao.DeleteFolder(folder.ID);
                            }

                            ProcessedFolder(folderId);
                        }
                    }
                }
                ProgressStep();
            }
        }

        private void DeleteFiles(List<object> fileIds)
        {
            if (fileIds.Count == 0) return;

            foreach (var fileId in fileIds)
            {
                if (Canceled) return;

                var file = FileDao.GetFile(fileId);
                if (file == null)
                {
                    Error = FilesCommonResource.ErrorMassage_FileNotFound;
                }
                else if (!ignoreException && (file.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing)
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteEditingFile;
                }
                else if (!ignoreException && !FilesSecurity.CanDelete(file))
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteFile;
                }
                else
                {
                    FileMarker.RemoveMarkAsNewForAll(file);
                    if (FileDao.UseTrashForRemove(file))
                    {
                        FileDao.MoveFile(file.ID, _trashId);
                    }
                    else
                    {
                        try
                        {
                            FileDao.DeleteFile(file.ID);
                            FileDao.DeleteFolder(file.ID);
                        }
                        catch (Exception ex)
                        {
                            Error = ex.Message;

                            Logger.Error(Error, ex);
                        }
                    }
                    ProcessedFile(fileId);
                }
                ProgressStep();
            }
        }
    }
}