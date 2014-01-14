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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Utils;

namespace ASC.Web.Files.Services.WCFService.FileOperations
{
    internal class FileMoveCopyOperation : FileOperation
    {
        private readonly string _toFolderId;
        private readonly bool _copy;
        private readonly FileConflictResolveType _resolveType;
        private readonly List<Guid> _markAsNewRecipientIDs;
        private readonly List<FileEntry> _needToMarkAsNew = new List<FileEntry>();

        protected override FileOperationType OperationType
        {
            get { return _copy ? FileOperationType.Copy : FileOperationType.Move; }
        }

        public FileMoveCopyOperation(Tenant tenant,
                                     List<object> folders,
                                     List<object> files,
                                     string toFolderId,
                                     bool copy,
                                     FileConflictResolveType resolveType)
            : base(tenant, folders, files)
        {
            _toFolderId = toFolderId;
            _copy = copy;
            _resolveType = resolveType;

            var toFolder = Global.DaoFactory.GetFolderDao().GetFolder(toFolderId);

            if (toFolder != null && toFolder.RootFolderType == FolderType.BUNCH)
                _markAsNewRecipientIDs = Global.GetProjectTeam(toFolder).ToList();
        }

        protected override void Do()
        {
            Status += string.Format("folder_{0}{1}", _toFolderId, SplitCharacter);

            //TODO: check on each iteration?
            var toFolder = FolderDao.GetFolder(_toFolderId);
            if (toFolder == null) return;
            if (!FilesSecurity.CanCreate(toFolder)) throw new System.Security.SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);

            if (_copy)
            {
                Folder rootFrom = null;
                if (0 < Folders.Count) rootFrom = FolderDao.GetRootFolder(Folders[0]);
                if (0 < Files.Count) rootFrom = FolderDao.GetRootFolderByFile(Files[0]);
                if (rootFrom != null && rootFrom.FolderType == FolderType.TRASH) throw new InvalidOperationException("Can not copy from Trash.");
                if (toFolder.RootFolderType == FolderType.TRASH) throw new InvalidOperationException("Can not copy to Trash.");
            }

            MoveOrCopyFolders(Folders, toFolder, _copy);
            MoveOrCopyFiles(Files, toFolder, _copy, _resolveType);

            _needToMarkAsNew.Distinct().ToList().ForEach(x => FileMarker.MarkAsNew(x, _markAsNewRecipientIDs));
        }

        private void MoveOrCopyFolders(ICollection folderIds, Folder toFolder, bool copy)
        {
            if (folderIds.Count == 0) return;

            var toFolderId = toFolder.ID;
            var isToFolder = Equals(toFolderId.ToString(), _toFolderId);

            foreach (var folderId in folderIds)
            {
                if (Canceled) return;

                var folder = FolderDao.GetFolder(folderId);
                if (folder == null)
                {
                    Error = FilesCommonResource.ErrorMassage_FolderNotFound;
                }
                else if (!FilesSecurity.CanRead(folder))
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_ReadFolder;
                }
                else if (!Equals((folder.ParentFolderID ?? string.Empty).ToString(), toFolderId.ToString()))
                {
                    try
                    {
                        //if destination folder contains folder with same name then merge folders
                        var conflictFolder = FolderDao.GetFolder(folder.Title, toFolderId);

                        if (copy || conflictFolder != null)
                        {
                            Folder newFolder;
                            if (conflictFolder != null)
                            {
                                newFolder = conflictFolder;

                                if (isToFolder)
                                    _needToMarkAsNew.Add(conflictFolder);
                            }
                            else
                            {
                                newFolder = FolderDao.CopyFolder(folder.ID, toFolderId);

                                if (isToFolder)
                                    _needToMarkAsNew.Add(newFolder);

                                ProcessedFolder(folderId);
                            }

                            if (FolderDao.UseRecursiveOperation(folder.ID, toFolderId))
                            {
                                MoveOrCopyFiles(FolderDao.GetFiles(folder.ID, false), newFolder, copy, _resolveType);
                                MoveOrCopyFolders(FolderDao.GetFolders(folder.ID).Select(f => f.ID).ToList(), newFolder, copy);

                                if (!copy)
                                {
                                    if (FolderDao.GetItemsCount(folder.ID, true) == 0 && FilesSecurity.CanDelete(folder))
                                    {
                                        FolderDao.DeleteFolder(folder.ID);
                                        ProcessedFolder(folderId);
                                    }
                                }
                            }
                            else
                            {
                                if (conflictFolder != null)
                                {
                                    if (copy)
                                    {
                                        newFolder = FolderDao.CopyFolder(folder.ID, toFolderId);

                                        if (isToFolder)
                                            _needToMarkAsNew.Add(newFolder);
                                    }
                                    else
                                    {
                                        FolderDao.MoveFolder(folder.ID, toFolderId);

                                        if (isToFolder)
                                            _needToMarkAsNew.Add(FolderDao.GetFolder(folder.ID));
                                    }

                                    ProcessedFolder(folderId);
                                }
                            }
                        }
                        else
                        {
                            if (FilesSecurity.CanDelete(folder))
                            {
                                FileMarker.RemoveMarkAsNewForAll(folder);

                                FolderDao.MoveFolder(folder.ID, toFolderId);

                                if (isToFolder)
                                    _needToMarkAsNew.Add(FolderDao.GetFolder(folder.ID));

                                ProcessedFolder(folderId);
                            }
                            else
                            {
                                Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = ex.Message;

                        Logger.Error(Error, ex);
                    }
                }
                ProgressStep();
            }
        }

        private void MoveOrCopyFiles(ICollection fileIds, Folder toFolder, bool copy, FileConflictResolveType resolveType)
        {
            if (fileIds.Count == 0) return;

            var toFolderId = toFolder.ID;
            foreach (var fileId in fileIds)
            {
                if (Canceled) return;

                var file = FileDao.GetFile(fileId);
                if (file == null)
                {
                    Error = FilesCommonResource.ErrorMassage_FileNotFound;
                }
                else if (!FilesSecurity.CanRead(file))
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_ReadFile;
                }
                else if (Global.EnableUploadFilter
                         && !Studio.Utility.FileUtility.ExtsUploadable.Contains(Studio.Utility.FileUtility.GetFileExtension(file.Title)))
                {
                    Error = FilesCommonResource.ErrorMassage_NotSupportedFormat;
                }
                else if (!Equals(file.FolderID.ToString(), toFolderId))
                {
                    try
                    {
                        var conflict = FileDao.GetFile(toFolderId, file.Title);
                        if (conflict != null && !FilesSecurity.CanEdit(conflict))
                        {
                            Error = FilesCommonResource.ErrorMassage_SecurityException;
                        }
                        else if (conflict == null)
                        {
                            if (copy)
                            {
                                File newFile = null;
                                try
                                {
                                    newFile = FileDao.CopyFile(file.ID, toFolderId); //Stream copy will occur inside dao

                                    if (Equals(newFile.FolderID.ToString(), _toFolderId))
                                        _needToMarkAsNew.Add(newFile);

                                    ProcessedFile(fileId);
                                }
                                catch
                                {
                                    if (newFile != null) FileDao.DeleteFile(newFile.ID);
                                    throw;
                                }
                            }
                            else
                            {
                                if ((file.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing)
                                {
                                    Error = FilesCommonResource.ErrorMassage_SecurityException_UpdateEditingFile;
                                }
                                else if (FilesSecurity.CanDelete(file))
                                {
                                    FileMarker.RemoveMarkAsNewForAll(file);

                                    var newFileId = FileDao.MoveFile(file.ID, toFolderId);

                                    if (Equals(toFolderId.ToString(), _toFolderId))
                                        _needToMarkAsNew.Add(FileDao.GetFile(newFileId));

                                    ProcessedFile(fileId);
                                }
                                else
                                {
                                    Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteFile;
                                }
                            }
                        }
                        else
                        {
                            if (resolveType == FileConflictResolveType.Overwrite)
                            {
                                conflict.Version++;
                                using (var stream = FileDao.GetFileStream(file))
                                {
                                    conflict.ContentLength = stream.Length;
                                    conflict = FileDao.SaveFile(conflict, stream);

                                    _needToMarkAsNew.Add(conflict);
                                }

                                if (copy)
                                {
                                    ProcessedFile(fileId);
                                }
                                else
                                {
                                    if ((file.FileStatus & FileStatus.IsEditing) == FileStatus.IsEditing)
                                    {
                                        Error = FilesCommonResource.ErrorMassage_SecurityException_UpdateEditingFile;
                                    }
                                    else if (FilesSecurity.CanDelete(file))
                                    {
                                        FileDao.DeleteFile(file.ID);
                                        FileDao.DeleteFolder(file.ID);
                                        ProcessedFile(fileId);
                                    }
                                    else
                                    {
                                        Error = FilesCommonResource.ErrorMassage_SecurityException_DeleteFile;
                                    }
                                }
                            }
                            else if (resolveType == FileConflictResolveType.Skip)
                            {
                                //nothing
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = ex.Message;
                        Logger.Error(Error, ex);
                    }
                }
                ProgressStep();
            }
        }
    }
}