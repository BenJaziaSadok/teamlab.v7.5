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
using System.Text;
using System.Web;
using System.Web.Caching;
using ASC.Core;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Web.Files.Api;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Import;
using ASC.Web.Files.Resources;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using Newtonsoft.Json.Linq;

namespace ASC.Web.Files.Utils
{
    public class EntryManager
    {
        public static IEnumerable<FileEntry> GetEntries(IFolderDao folderDao, Folder parent, FilterType filter, Guid subjectId, OrderBy orderBy, String searchText, int from, int count, out int total)
        {
            total = 0;

            if (parent == null) throw new ArgumentNullException(FilesCommonResource.ErrorMassage_FolderNotFound);

            var fileSecurity = Global.GetFilesSecurity();
            var entries = Enumerable.Empty<FileEntry>();

            if (parent.FolderType == FolderType.Projects && parent.ID.Equals(Global.FolderProjects))
            {
                var apiServer = new ASC.Api.ApiServer();
                var apiUrl = String.Format("{0}project/maxlastmodified.json", SetupInfo.WebApiBaseUrl);

                var responseApi = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(apiServer.GetApiResponse(apiUrl, "GET"))));

                var projectLastModified = responseApi["response"].Value<String>();
                const string projectLastModifiedCacheKey = "documents/projectFolders/projectLastModified";
                if (HttpRuntime.Cache.Get(projectLastModifiedCacheKey) == null || !HttpRuntime.Cache.Get(projectLastModifiedCacheKey).Equals(projectLastModified))
                    HttpRuntime.Cache.Insert(projectLastModifiedCacheKey, projectLastModified);

                var projectListCacheKey = String.Format("documents/projectFolders/{0}", SecurityContext.CurrentAccount.ID);
                var fromCache = HttpRuntime.Cache.Get(projectListCacheKey);

                if (fromCache == null)
                {
                    apiUrl = String.Format("{0}project/filter.json?sortBy=title&sortOrder=ascending", SetupInfo.WebApiBaseUrl);

                    responseApi = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(apiServer.GetApiResponse(apiUrl, "GET"))));

                    var responseData = responseApi["response"];

                    if (!(responseData is JArray)) return entries.ToList();

                    var folderIDProjectTitle = new Dictionary<object, String>();

                    foreach (JObject projectInfo in responseData.Children())
                    {
                        var projectID = projectInfo["id"].Value<String>();
                        var projectTitle = Global.ReplaceInvalidCharsAndTruncate(projectInfo["title"].Value<String>());
                        int projectFolderID;

                        JToken projectSecurityJToken;
                        if (projectInfo.TryGetValue("security", out projectSecurityJToken))
                        {
                            var projectSecurity = projectInfo["security"].Value<JObject>();
                            JToken projectCanFileReadJToken;
                            if (projectSecurity.TryGetValue("canReadFiles", out projectCanFileReadJToken))
                            {
                                if (!projectSecurity["canReadFiles"].Value<bool>())
                                {
                                    continue;
                                }
                            }
                        }

                        JToken projectFolderIDJToken;

                        if (projectInfo.TryGetValue("projectFolder", out projectFolderIDJToken))
                            projectFolderID = projectInfo["projectFolder"].Value<int>();
                        else
                            projectFolderID = (int)FilesIntegration.RegisterBunch("projects", "project", projectID);

                        if (!folderIDProjectTitle.ContainsKey(projectFolderID))
                            folderIDProjectTitle.Add(projectFolderID, projectTitle);

                        HttpRuntime.Cache.Insert("documents/folders/" + projectFolderID.ToString(), projectTitle, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(30));
                    }

                    var folders = folderDao.GetFolders(folderIDProjectTitle.Keys.ToArray());
                    folders.ForEach(x =>
                                        {
                                            x.Title = folderIDProjectTitle[x.ID];
                                            x.Access = FileShare.Read;
                                            x.FolderUrl = PathProvider.GetFolderUrl(x);
                                        });

                    entries = entries.Concat(folders);

                    if (entries.Any())
                        HttpRuntime.Cache.Insert(projectListCacheKey, entries, new CacheDependency(null, new[] { projectLastModifiedCacheKey }), Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(15));
                }
                else
                {
                    entries = entries.Concat((IEnumerable<FileEntry>)fromCache);
                }

                entries = FilterEntries(entries, filter, subjectId, searchText);

                parent.TotalFiles = entries.Aggregate(0, (a, f) => a + (f is Folder ? ((Folder)f).TotalFiles : 1));
                parent.TotalSubFolders = entries.Aggregate(0, (a, f) => a + (f is Folder ? ((Folder)f).TotalSubFolders + 1 : 0));
            }
            else if (parent.FolderType == FolderType.SHARE)
            {
                //share
                var shared = (IEnumerable<FileEntry>)fileSecurity.GetSharesForMe();
                shared = FilterEntries(shared, filter, subjectId, searchText)
                    .Where(f => f.CreateBy != SecurityContext.CurrentAccount.ID && // don't show my files
                                f.RootFolderType == FolderType.USER); // don't show common files (common files can read)
                entries = entries.Concat(shared);

                parent.TotalFiles = entries.Aggregate(0, (a, f) => a + (f is Folder ? ((Folder)f).TotalFiles : 1));
                parent.TotalSubFolders = entries.Aggregate(0, (a, f) => a + (f is Folder ? ((Folder)f).TotalSubFolders + 1 : 0));
            }
            else
            {
                var folders = folderDao.GetFolders(parent.ID, orderBy, filter, subjectId, searchText).Cast<FileEntry>();
                folders = fileSecurity.FilterRead(folders);
                entries = entries.Concat(folders);

                var files = folderDao.GetFiles(parent.ID, orderBy, filter, subjectId, searchText).Cast<FileEntry>();
                files = fileSecurity.FilterRead(files);
                entries = entries.Concat(files);

                if (ImportConfiguration.SupportInclusion && (parent.ID.Equals(Global.FolderMy) || parent.ID.Equals(Global.FolderCommon)))
                {
                    using (var securityDao = Global.DaoFactory.GetSecurityDao())
                    using (var providerDao = Global.DaoFactory.GetProviderDao())
                    {
                        var providers = providerDao.GetProvidersInfo(parent.RootFolderType);
                        var folderList = providers
                            .Select(providerInfo =>
                                    //Fake folder. Don't send request to third party
                                    new Folder
                                        {
                                            ID = providerInfo.RootFolderId,
                                            ParentFolderID = parent.ID,
                                            CreateBy = providerInfo.Owner,
                                            CreateOn = providerInfo.CreateOn,
                                            FolderType = FolderType.DEFAULT,
                                            ModifiedBy = providerInfo.Owner,
                                            ModifiedOn = providerInfo.CreateOn,
                                            ProviderId = providerInfo.ID,
                                            ProviderKey = providerInfo.ProviderKey,
                                            RootFolderCreator = providerInfo.Owner,
                                            RootFolderId = providerInfo.RootFolderId,
                                            RootFolderType = providerInfo.RootFolderType,
                                            Shareable = false,
                                            Title = providerInfo.CustomerTitle,
                                            TotalFiles = 0,
                                            TotalSubFolders = 0
                                        }
                            )
                            .Where(fileSecurity.CanRead).ToList();

                        if (folderList.Any())
                            securityDao.GetPureShareRecords(folderList.Cast<FileEntry>().ToArray())
                                       .Where(x => x.Owner == SecurityContext.CurrentAccount.ID)
                                       .Select(x => x.EntryId).Distinct().ToList()
                                       .ForEach(id =>
                                                    {
                                                        folderList.First(y => y.ID.Equals(id)).SharedByMe = true;
                                                    });

                        var thirdPartyFolder = FilterEntries(folderList, filter, subjectId, searchText);
                        entries = entries.Concat(thirdPartyFolder);
                    }
                }
            }

            if (orderBy.SortedBy != SortedByType.New)
            {
                entries = SortEntries(entries, orderBy);

                total = entries.Count();
                if (0 < from) entries = entries.Skip(from);
                if (0 < count) entries = entries.Take(count);
            }

            entries = FileMarker.SetTagsNew(folderDao, parent, entries);

            //sorting after marking
            if (orderBy.SortedBy == SortedByType.New)
            {
                entries = SortEntries(entries, orderBy);

                total = entries.Count();
                if (0 < from) entries = entries.Skip(from);
                if (0 < count) entries = entries.Take(count);
            }

            return entries;
        }

        public static IEnumerable<FileEntry> FilterEntries(IEnumerable<FileEntry> entries, FilterType filter, Guid subjectId, String searchText)
        {
            Func<FileEntry, bool> where = _ => true;

            switch (filter)
            {
                case FilterType.ByUser:
                    where = f => f.CreateBy == subjectId;
                    break;
                case FilterType.ByDepartment:
                    where = f => CoreContext.UserManager.GetUsersByGroup(subjectId).Any(s => s.ID == f.CreateBy);
                    break;
                case FilterType.SpreadsheetsOnly:
                case FilterType.PresentationsOnly:
                case FilterType.ImagesOnly:
                case FilterType.DocumentsOnly:
                case FilterType.FilesOnly:
                    where = f => f is File && (((File)f).FilterType == filter || filter == FilterType.FilesOnly);
                    break;
                case FilterType.FoldersOnly:
                    where = f => f is Folder;
                    break;
            }
            return entries.Where(where).Where(f => f.Title.ToLower().Contains((searchText ?? string.Empty).ToLower().Trim()));
        }

        public static IEnumerable<FileEntry> SortEntries(IEnumerable<FileEntry> entries, OrderBy orderBy)
        {
            Comparison<FileEntry> sorter;

            if (orderBy == null) orderBy = new OrderBy(SortedByType.DateAndTime, false);

            var c = orderBy.IsAsc ? 1 : -1;
            switch (orderBy.SortedBy)
            {
                case SortedByType.Type:
                    sorter = (x, y) =>
                                 {
                                     var cmp = 0;
                                     if (x is File && y is File)
                                         cmp = c*(FileUtility.GetFileExtension((x.Title)).CompareTo(FileUtility.GetFileExtension(y.Title)));
                                     return cmp == 0 ? x.Title.CompareTo(y.Title) : cmp;
                                 };
                    break;
                case SortedByType.Author:
                    sorter = (x, y) =>
                                 {
                                     var cmp = c*string.Compare(x.ModifiedByString, y.ModifiedByString);
                                     return cmp == 0 ? x.Title.CompareTo(y.Title) : cmp;
                                 };
                    break;
                case SortedByType.Size:
                    sorter = (x, y) =>
                                 {
                                     var cmp = 0;
                                     if (x is File && y is File)
                                         cmp = c*((File)x).ContentLength.CompareTo(((File)y).ContentLength);
                                     return cmp == 0 ? x.Title.CompareTo(y.Title) : cmp;
                                 };
                    break;
                case SortedByType.AZ:
                    sorter = (x, y) => c*x.Title.CompareTo(y.Title);
                    break;
                case SortedByType.DateAndTime:
                    sorter = (x, y) =>
                                 {
                                     var cmp = c*DateTime.Compare(x.ModifiedOn, y.ModifiedOn);
                                     return cmp == 0 ? x.Title.CompareTo(y.Title) : cmp;
                                 };
                    break;
                case SortedByType.New:
                    sorter = (x, y) =>
                                 {
                                     var isNew = new Func<FileEntry, int>(
                                         val => val is File
                                                    ? ((((File)val).FileStatus & FileStatus.IsNew) == FileStatus.IsNew ? 1 : 0)
                                                    : ((Folder)val).NewForMe);

                                     var isNewSortResult = c*isNew(x).CompareTo(isNew(y));

                                     if (isNewSortResult == 0)
                                     {
                                         var dataTimeSortResult = (-1)*DateTime.Compare(x.ModifiedOn, y.ModifiedOn);

                                         return dataTimeSortResult == 0
                                                    ? x.Title.CompareTo(y.Title)
                                                    : dataTimeSortResult;
                                     }

                                     return isNewSortResult;
                                 };
                    break;
                default:
                    sorter = (x, y) => c*x.Title.CompareTo(y.Title);
                    break;
            }

            if (orderBy.SortedBy != SortedByType.New)
            {
                // folders on top
                var folders = entries.OfType<Folder>().Cast<FileEntry>().ToList();
                var files = entries.OfType<File>().Cast<FileEntry>().ToList();
                folders.Sort(sorter);
                files.Sort(sorter);

                return folders.Concat(files);
            }

            var result = entries.ToList();

            result.Sort(sorter);

            return result;
        }


        public static List<Folder> GetBreadCrumbs(object folderId)
        {
            using (var folderDao = Global.DaoFactory.GetFolderDao())
            {
                return GetBreadCrumbs(folderId, folderDao);
            }
        }

        public static List<Folder> GetBreadCrumbs(object folderId, IFolderDao folderDao)
        {
            var breadCrumbs = Global.GetFilesSecurity().FilterRead(folderDao.GetParentFolders(folderId)).ToList();

            var firstVisible = breadCrumbs.ElementAtOrDefault(0);

            object rootId = null;
            if (firstVisible == null)
            {
                rootId = Global.FolderShare;
            }
            else
            {
                switch (firstVisible.FolderType)
                {
                    case FolderType.DEFAULT:
                        if (!firstVisible.ProviderEntry)
                        {
                            rootId = Global.FolderShare;
                        }
                        else
                        {
                            switch (firstVisible.RootFolderType)
                            {
                                case FolderType.USER:
                                    rootId = SecurityContext.CurrentAccount.ID == firstVisible.RootFolderCreator
                                                 ? Global.FolderMy
                                                 : Global.FolderShare;
                                    break;
                                case FolderType.COMMON:
                                    rootId = Global.FolderCommon;
                                    break;
                            }
                        }
                        break;

                    case FolderType.BUNCH:
                        rootId = Global.FolderProjects;
                        break;
                }
            }

            if (rootId != null)
            {
                breadCrumbs.Insert(0, folderDao.GetFolder(rootId));
            }

            return breadCrumbs;
        }
    }
}