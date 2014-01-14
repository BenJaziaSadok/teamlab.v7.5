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

using System.Collections.Generic;
using System.Linq;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Web.Files.Utils;

namespace ASC.Web.Files.Services.WCFService.FileOperations
{
    internal class FileMarkAsReadOperation : FileOperation
    {
        public FileMarkAsReadOperation(Tenant tenant, List<object> folders, List<object> files)
            : base(tenant, folders, files)
        {
            CountWithoutSubitems = true;
        }

        protected override FileOperationType OperationType
        {
            get { return FileOperationType.MarkAsRead; }
        }

        protected override void Do()
        {
            Percentage = 0;

            var entries = Enumerable.Empty<FileEntry>();

            if (Folders.Any())
                entries = entries.Concat(FolderDao.GetFolders(Folders.ToArray()).Cast<FileEntry>());

            if (Files.Any())
                entries = entries.Concat(FileDao.GetFiles(Files.ToArray()).Cast<FileEntry>());

            entries.ToList().ForEach(x =>
                                         {
                                             if (Canceled) return;

                                             FileMarker.RemoveMarkAsNew(x, Owner);

                                             if (x is File)
                                                 ProcessedFile(x.ID.ToString());
                                             else
                                                 ProcessedFolder(x.ID.ToString());

                                             ProgressStep();
                                         });

            var rootFolderIdAsNew =
                FileMarker.GetRootFoldersIdMarkedAsNew()
                          .Select(item => string.Format("new_{{\"key\"? \"{0}\", \"value\"? \"{1}\"}}", item.Key, item.Value));

            Status += string.Join(SplitCharacter, rootFolderIdAsNew.ToArray());

            IsCompleted = true;
        }
    }
}