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
using ASC.Web.Files.Classes;
using ASC.Web.Files.Import;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;

namespace ASC.Web.Files.Services.WCFService.FileOperations
{
    internal class FileImportOperation : FileOperation
    {
        private readonly IDocumentProvider _docProvider;
        private readonly List<DataToImport> _files;
        private readonly object _parentId;
        private readonly bool _overwrite;
        private readonly string _folderName;
        private readonly List<Guid> _markAsNewRecipientIDs;


        protected override FileOperationType OperationType
        {
            get { return FileOperationType.Import; }
        }


        public FileImportOperation(Tenant tenant, IDocumentProvider docProvider, List<DataToImport> files, object parentId, bool overwrite, string folderName)
            : base(tenant, null, null)
        {
            Id = Owner.ToString() + OperationType.ToString();
            Source = docProvider.Name;
            this._docProvider = docProvider;
            this._files = files ?? new List<DataToImport>();
            this._parentId = parentId;
            this._overwrite = overwrite;
            this._folderName = folderName;

            var toFolderObj = Global.DaoFactory.GetFolderDao().GetFolder(_parentId);

            if (toFolderObj != null && toFolderObj.RootFolderType == FolderType.BUNCH)
                _markAsNewRecipientIDs = Global.GetProjectTeam(toFolderObj).ToList();
        }


        protected override double InitProgressStep()
        {
            return _files.Count == 0 ? 100d : 100d/_files.Count;
        }

        protected override void Do()
        {
            if (_files.Count == 0) return;

            var parent = FolderDao.GetFolder(_parentId);
            if (parent == null) throw new Exception(FilesCommonResource.ErrorMassage_FolderNotFound);
            if (!FilesSecurity.CanCreate(parent)) throw new System.Security.SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);
            if (parent.RootFolderType == FolderType.TRASH) throw new Exception(FilesCommonResource.ErrorMassage_ImportToTrash);
            if (parent.ProviderEntry) throw new System.Security.SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);

            var to =
                FolderDao.GetFolder(_folderName, _parentId)
                ?? FolderDao.SaveFolder(
                    new Folder
                        {
                            FolderType = FolderType.DEFAULT,
                            ParentFolderID = _parentId,
                            Title = _folderName
                        });

            foreach (var f in _files)
            {
                if (Canceled) return;
                try
                {
                    long size;
                    using (var stream = _docProvider.GetDocumentStream(f.ContentLink, out size))
                    {
                        if (stream == null)
                            throw new Exception("Can not import document " + f.ContentLink + ". Empty stream.");

                        if (SetupInfo.MaxUploadSize < size)
                        {
                            throw FileSizeComment.FileSizeException;
                        }

                        var folderId = to.ID;
                        var pos = f.Title.LastIndexOf('/');
                        if (0 < pos)
                        {
                            folderId = GetOrCreateHierarchy(f.Title.Substring(0, pos), to);
                            f.Title = f.Title.Substring(pos + 1);
                        }

                        f.Title = Global.ReplaceInvalidCharsAndTruncate(f.Title);
                        var file = new File
                            {
                                Title = f.Title,
                                FolderID = folderId,
                                ContentLength = size,
                            };

                        var conflict = FileDao.GetFile(file.FolderID, file.Title);
                        if (conflict != null)
                        {
                            if (_overwrite)
                            {
                                file.ID = conflict.ID;
                                file.Version = conflict.Version + 1;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (size <= 0L)
                        {
                            using (var buffered = stream.GetBuffered())
                            {
                                size = buffered.Length;

                                if (SetupInfo.MaxUploadSize < size)
                                {
                                    throw FileSizeComment.FileSizeException;
                                }

                                file.ContentLength = size;
                                try
                                {
                                    file = FileDao.SaveFile(file, buffered);
                                }
                                catch (Exception error)
                                {
                                    FileDao.DeleteFile(file.ID);
                                    throw error;
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                file = FileDao.SaveFile(file, stream);
                            }
                            catch (Exception error)
                            {
                                FileDao.DeleteFile(file.ID);
                                throw error;
                            }
                        }

                        FileMarker.MarkAsNew(file, _markAsNewRecipientIDs);
                    }
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                    Logger.Error(Error, ex);
                }
                finally
                {
                    ProgressStep();
                }
            }
        }

        private object GetOrCreateHierarchy(string path, Folder root)
        {
            path = path != null ? path.Trim('/') : null;
            if (string.IsNullOrEmpty(path)) return root.ID;

            var pos = path.IndexOf("/");
            var title = 0 < pos ? path.Substring(0, pos) : path;
            path = 0 < pos ? path.Substring(pos + 1) : null;

            title = Global.ReplaceInvalidCharsAndTruncate(title);

            var folder =
                FolderDao.GetFolder(title, root.ID)
                ?? FolderDao.SaveFolder(
                    new Folder
                        {
                            ParentFolderID = root.ID,
                            Title = title,
                            FolderType = FolderType.DEFAULT
                        });

            return GetOrCreateHierarchy(path, folder);
        }
    }
}