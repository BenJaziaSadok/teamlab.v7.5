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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using Ionic.Zip;
using Ionic.Zlib;
using File = ASC.Files.Core.File;

namespace ASC.Web.Files.Services.WCFService.FileOperations
{
    internal class FileDownloadOperation : FileOperation
    {
        private readonly Dictionary<object, string> _files;

        private readonly bool _quotaDocsEdition;

        protected override FileOperationType OperationType
        {
            get { return FileOperationType.Download; }
        }

        public FileDownloadOperation(Tenant tenant, Dictionary<object, string> folders, Dictionary<object, string> files)
            : base(tenant, folders.Select(f => f.Key).ToList(), files.Select(f => f.Key).ToList())
        {
            Id = Owner.ToString() + OperationType.ToString(); //one download per user

            _files = files;

            _quotaDocsEdition = TenantExtra.GetTenantQuota().DocsEdition;
        }

        protected override void Do()
        {
            var entriesPathId = GetEntriesPathId();
            if (entriesPathId == null || entriesPathId.Count == 0)
            {
                throw new Exception(0 < Files.Count ? FilesCommonResource.ErrorMassage_FileNotFound : FilesCommonResource.ErrorMassage_FolderNotFound);
            }

            ReplaceLongPath(entriesPathId);

            using (var stream = CompressToZip(entriesPathId))
            {
                if (stream != null)
                {
                    stream.Position = 0;
                    const string fileName = UrlConstant.DownloadTitle + ".zip";
                    var store = Global.GetStore();
                    store.Save(
                        FileConstant.StorageDomainTmp,
                        string.Format(@"{0}\{1}", Owner, fileName),
                        stream,
                        "application/zip",
                        "attachment; filename=\"" + fileName + "\"");
                    Status = string.Format("{0}?{1}=bulk", CommonLinkUtility.FileHandlerPath, CommonLinkUtility.Action);
                }
            }
        }

        private ItemNameValueCollection ExecPathFromFile(File file, string path)
        {
            FileMarker.RemoveMarkAsNew(file);

            var title = file.Title;

            if (_files.ContainsKey(file.ID.ToString()))
            {
                var convertToExt = string.Empty;
                if (_quotaDocsEdition || FileUtility.InternalExtension.Values.Contains(convertToExt))
                    convertToExt = _files[file.ID.ToString()];

                if (!string.IsNullOrEmpty(convertToExt))
                {
                    title = FileUtility.ReplaceFileExtension(title, convertToExt);
                }
            }

            var entriesPathId = new ItemNameValueCollection();
            entriesPathId.Add(path + title, file.ID.ToString());

            return entriesPathId;
        }

        private ItemNameValueCollection GetEntriesPathId()
        {
            var entriesPathId = new ItemNameValueCollection();
            if (0 < Files.Count)
            {
                var files = FileDao.GetFiles(Files.ToArray());
                files = FilesSecurity.FilterRead(files).ToList();
                files.ForEach(file => entriesPathId.Add(ExecPathFromFile(file, string.Empty)));

            }
            if (0 < Folders.Count)
            {

                FilesSecurity.FilterRead(FolderDao.GetFolders(Files.ToArray())).ToList().Cast<FileEntry>().ToList()
                             .ForEach(FileMarker.RemoveMarkAsNew);

                var filesInFolder = GetFilesInFolders(Folders, string.Empty);
                if (filesInFolder == null) return null;
                entriesPathId.Add(filesInFolder);
            }
            return entriesPathId;
        }

        private ItemNameValueCollection GetFilesInFolders(IEnumerable<object> folderIds, string path)
        {
            if (Canceled) return null;

            var entriesPathId = new ItemNameValueCollection();
            foreach (var folderId in folderIds)
            {
                var folder = FolderDao.GetFolder(folderId);
                if (folder == null || !FilesSecurity.CanRead(folder)) continue;
                var folderPath = path + folder.Title + "/";

                var files = FolderDao.GetFiles(folder.ID, null, FilterType.None, Guid.Empty, string.Empty);
                files = FilesSecurity.FilterRead(files).ToList();
                files.ForEach(file => entriesPathId.Add(ExecPathFromFile(file, folderPath)));

                FileMarker.RemoveMarkAsNew(folder);

                var nestedFolders = FolderDao.GetFolders(folder.ID);
                nestedFolders = FilesSecurity.FilterRead(nestedFolders).ToList();
                if (files.Count == 0 && nestedFolders.Count == 0)
                {
                    entriesPathId.Add(folderPath, String.Empty);
                }

                var filesInFolder = GetFilesInFolders(nestedFolders.ConvertAll(f => f.ID), folderPath);
                if (filesInFolder == null) return null;
                entriesPathId.Add(filesInFolder);
            }
            return entriesPathId;
        }

        private Stream CompressToZip(ItemNameValueCollection entriesPathId)
        {
            var stream = TempStream.Create();
            using (var zip = new ZipOutputStream(stream, true))
            {
                zip.CompressionLevel = CompressionLevel.Level3;
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                zip.AlternateEncoding = Encoding.GetEncoding(Thread.CurrentThread.CurrentCulture.TextInfo.OEMCodePage);

                foreach (var path in entriesPathId.AllKeys)
                {
                    if (Canceled)
                    {
                        zip.Dispose();
                        stream.Dispose();
                        return null;
                    }

                    var counter = 0;
                    foreach (var entryId in entriesPathId[path])
                    {
                        var newtitle = path;

                        File file = null;
                        var convertToExt = string.Empty;

                        if (!string.IsNullOrEmpty(entryId))
                        {
                            file = FileDao.GetFile(entryId);

                            if (file.ContentLength > SetupInfo.AvailableFileSize)
                            {
                                Error = string.Format(FilesCommonResource.ErrorMassage_FileSizeZip, FileSizeComment.FilesSizeToString(SetupInfo.AvailableFileSize));
                                continue;
                            }

                            if (_files.ContainsKey(file.ID.ToString()))
                            {
                                if (_quotaDocsEdition || FileUtility.InternalExtension.Values.Contains(convertToExt))
                                    convertToExt = _files[file.ID.ToString()];

                                if (!string.IsNullOrEmpty(convertToExt))
                                {
                                    newtitle = FileUtility.ReplaceFileExtension(path, convertToExt);
                                }
                            }
                        }

                        if (0 < counter)
                        {
                            var suffix = " (" + counter + ")";

                            if (!string.IsNullOrEmpty(entryId))
                            {
                                newtitle = 0 < newtitle.IndexOf('.')
                                               ? newtitle.Insert(newtitle.LastIndexOf('.'), suffix)
                                               : newtitle + suffix;
                            }
                            else
                            {
                                break;
                            }
                        }

                        zip.PutNextEntry(newtitle);

                        if (!string.IsNullOrEmpty(entryId) && file != null)
                        {
                            if (file.ConvertedType != null || !string.IsNullOrEmpty(convertToExt))
                            {
                                //Take from converter
                                try
                                {
                                    using (var readStream = !string.IsNullOrEmpty(convertToExt)
                                                                ? FileConverter.Exec(file, convertToExt)
                                                                : FileConverter.Exec(file))
                                    {
                                        if (readStream != null)
                                        {
                                            readStream.StreamCopyTo(zip);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Error = ex.Message;

                                    Logger.Error(Error, ex);
                                }
                            }
                            else
                            {
                                using (var readStream = FileDao.GetFileStream(file))
                                {
                                    readStream.StreamCopyTo(zip);
                                }
                            }
                        }
                        counter++;
                    }

                    ProgressStep();
                }
                return stream;
            }
        }

        private static void ReplaceLongPath(ItemNameValueCollection entriesPathId)
        {
            foreach (var path in new List<string>(entriesPathId.AllKeys))
            {
                if (200 >= path.Length || 0 >= path.IndexOf('/')) continue;

                var ids = entriesPathId[path];
                entriesPathId.Remove(path);

                var newtitle = "LONG_FOLDER_NAME" + path.Substring(path.LastIndexOf('/'));
                entriesPathId.Add(newtitle, ids);
            }
        }


        private class ItemNameValueCollection
        {
            private readonly Dictionary<string, List<string>> _dictionaryName;

            internal ItemNameValueCollection()
            {
                _dictionaryName = new Dictionary<string, List<string>>();
            }

            public IEnumerable<string> AllKeys
            {
                get { return _dictionaryName.Keys.ToArray(); }
            }

            public IEnumerable<string> this[string name]
            {
                get { return _dictionaryName[name].ToArray(); }
            }

            public int Count
            {
                get { return _dictionaryName.Keys.Count; }
            }

            public void Add(string name, string value)
            {
                if (!_dictionaryName.ContainsKey(name))
                {
                    _dictionaryName.Add(name, new List<string>());
                }
                _dictionaryName[name].Add(value);
            }

            public void Add(ItemNameValueCollection itemNameValueCollection)
            {
                foreach (var key in itemNameValueCollection.AllKeys)
                {
                    foreach (var value in itemNameValueCollection[key])
                    {
                        Add(key, value);
                    }
                }
            }

            internal void Add(string name, IEnumerable<string> values)
            {
                if (!_dictionaryName.ContainsKey(name))
                {
                    _dictionaryName.Add(name, new List<string>());
                }
                _dictionaryName[name].AddRange(values);
            }

            public void Remove(string name)
            {
                _dictionaryName.Remove(name);
            }
        }
    }
}