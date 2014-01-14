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
using System.Data;
using System.Linq;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Files.Core.Security;
using ASC.Web.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Feed.Aggregator.Modules.Documents
{
    internal class FilesModule : FeedModule
    {
        private const string fileItem = "file";
        private const string sharedFileItem = "sharedFile";


        protected override string Table
        {
            get { return "files_file"; }
        }

        protected override string LastUpdatedColumn
        {
            get { return "modified_on"; }
        }

        protected override string TenantColumn
        {
            get { return "tenant_id"; }
        }

        protected override string DbId
        {
            get { return Constants.FilesDbId; }
        }


        public override string Name
        {
            get { return Constants.FilesModule; }
        }

        public override string Product
        {
            get { return ModulesHelper.DocumentsProductName; }
        }

        public override Guid ProductID
        {
            get { return ModulesHelper.DocumentsProductID; }
        }


        public override bool VisibleFor(Feed feed, object data, Guid userId)
        {
            if (!WebItemSecurity.IsAvailableForUser(ProductID.ToString(), userId)) return false;

            var file = (FileEntry)data;

            bool targetCond;
            if (feed.Target != null)
            {
                if (!string.IsNullOrEmpty(file.SharedToMeBy) && file.SharedToMeBy == userId.ToString()) return false;

                var owner = new Guid((string)feed.Target);
                var groupUsers = CoreContext.UserManager.GetUsersByGroup(owner).Select(x => x.ID).ToList();
                if (!groupUsers.Any())
                {
                    groupUsers.Add(owner);
                }
                targetCond = groupUsers.Contains(userId);
            }
            else
            {
                targetCond = true;
            }

            return targetCond &&
                   new FileSecurity(new DaoFactory()).CanRead(file, userId);
        }

        public override IEnumerable<int> GetTenantsWithFeeds(DateTime fromTime)
        {
            var q1 = new SqlQuery("files_file")
                .Select("tenant_id")
                .Where(Exp.Gt("modified_on", fromTime))
                .GroupBy(1)
                .Having(Exp.Gt("count(*)", 0));

            var q2 = new SqlQuery("files_security")
                .Select("tenant_id")
                .Where(Exp.Gt("timestamp", fromTime))
                .GroupBy(1)
                .Having(Exp.Gt("count(*)", 0));

            using (var db = new DbManager(DbId))
            {
                return db.ExecuteList(q1).ConvertAll(r => Convert.ToInt32(r[0]))
                         .Union(db.ExecuteList(q2).ConvertAll(r => Convert.ToInt32(r[0])));
            }
        }

        public override IEnumerable<Tuple<Feed, object>> GetFeeds(FeedFilter filter)
        {
            var q1 = new SqlQuery("files_file f")
                .Select(FileColumns().Select(f => "f." + f).ToArray())
                .Select(DocumentsDbHelper.GetRootFolderType("folder_id"))
                .Select("null, null, null")
                .Where(
                    Exp.Eq("f.tenant_id", filter.Tenant)
                    & Exp.Eq("f.current_version", 1)
                    & Exp.Between("f.modified_on", filter.Time.From, filter.Time.To));

            var q2 = new SqlQuery("files_file f")
                .Select(FileColumns().Select(f => "f." + f).ToArray())
                .Select(DocumentsDbHelper.GetRootFolderType("folder_id"))
                .LeftOuterJoin("files_security s",
                               Exp.EqColumns("s.entry_id", "f.id") &
                               Exp.Eq("s.tenant_id", filter.Tenant) &
                               Exp.Eq("s.entry_type", (int)FileEntryType.File)
                )
                .Select("s.timestamp, s.owner, s.subject")
                .Where(Exp.Eq("f.tenant_id", filter.Tenant) &
                       Exp.Eq("f.current_version", 1) &
                       Exp.Lt("s.security", 3) &
                       Exp.Between("s.timestamp", filter.Time.From, filter.Time.To));

            using (var db = new DbManager(DbId))
            {
                var files = db.ExecuteList(q1.UnionAll(q2)).ConvertAll(ToFile);
                return files
                    .Where(f => f.RootFolderType != FolderType.TRASH && f.RootFolderType != FolderType.BUNCH)
                    .Select(f => new Tuple<Feed, object>(ToFeed(f), f));
            }
        }


        private static IEnumerable<string> FileColumns()
        {
            return new[]
                {
                    "id",
                    "version",
                    "folder_id",
                    "title",
                    "content_length",
                    "file_status",
                    "create_by",
                    "create_on",
                    "modified_by",
                    "modified_on",
                    "converted_type" // 10
                };
        }

        private static File ToFile(object[] r)
        {
            return new File
                {
                    ID = r[0],
                    Version = Convert.ToInt32(r[1]),
                    FolderID = r[2],
                    Title = Convert.ToString(r[3]),
                    ContentLength = Convert.ToInt64(r[4]),
                    FileStatus = (FileStatus)Convert.ToInt32(r[5]),
                    CreateBy = new Guid(Convert.ToString(r[6])),
                    CreateOn = Convert.ToDateTime(r[7]),
                    ModifiedBy = new Guid(Convert.ToString(r[8])),
                    ModifiedOn = Convert.ToDateTime(r[9]),
                    ConvertedType = Convert.ToString(r[10]),
                    RootFolderType = DocumentsDbHelper.ParseRootFolderType(r[11]),
                    RootFolderCreator = DocumentsDbHelper.ParseRootFolderCreator(r[11]),
                    RootFolderId = DocumentsDbHelper.ParseRootFolderId(r[11]),
                    SharedToMeOn = r[12] != null ? Convert.ToDateTime(r[12]) : (DateTime?)null,
                    SharedToMeBy = r[13] != null ? Convert.ToString(r[13]) : null,
                    // here stored subject of the file share 
                    CreateByString = r[14] != null ? Convert.ToString(r[14]) : null
                };
        }

        private Feed ToFeed(File file)
        {
            var rootFolder = new FolderDao(Tenant, DbId).GetFolder(file.FolderID);

            if (file.SharedToMeOn.HasValue)
            {
                var feed = new Feed(new Guid(file.SharedToMeBy), file.SharedToMeOn.Value, true)
                    {
                        Item = sharedFileItem,
                        ItemId = string.Format("{0}_{1}", file.ID, file.CreateByString),
                        ItemUrl = CommonLinkUtility.GetFileRedirectPreviewUrl(file.ID, true),
                        Product = Product,
                        Module = Name,
                        Title = file.Title,
                        AdditionalInfo = file.ContentLengthString,
                        AdditionalInfo2 = rootFolder.FolderType == FolderType.DEFAULT ? rootFolder.Title : string.Empty,
                        AdditionalInfo3 = rootFolder.FolderType == FolderType.DEFAULT ? CommonLinkUtility.GetFileRedirectPreviewUrl(file.FolderID, false) : string.Empty,
                        Keywords = string.Format("{0}", file.Title),
                        HasPreview = false,
                        CanComment = false,
                        Target = file.CreateByString,
                        GroupId = GetGroupId(sharedFileItem, new Guid(file.SharedToMeBy), file.FolderID.ToString())
                    };

                return feed;
            }

            var updated = file.Version != 1;
            return new Feed(file.ModifiedBy, file.ModifiedOn)
                {
                    Item = fileItem,
                    ItemId = string.Format("{0}_{1}", file.ID, file.Version > 1 ? 1 : 0),
                    ItemUrl = CommonLinkUtility.GetFileRedirectPreviewUrl(file.ID, true),
                    Product = Product,
                    Module = Name,
                    Action = updated ? FeedAction.Updated : FeedAction.Created,
                    Title = file.Title,
                    AdditionalInfo = file.ContentLengthString,
                    AdditionalInfo2 = rootFolder.FolderType == FolderType.DEFAULT ? rootFolder.Title : string.Empty,
                    AdditionalInfo3 = rootFolder.FolderType == FolderType.DEFAULT ? CommonLinkUtility.GetFileRedirectPreviewUrl(file.FolderID, false) : string.Empty,
                    Keywords = string.Format("{0}", file.Title),
                    HasPreview = false,
                    CanComment = false,
                    Target = null,
                    GroupId = GetGroupId(fileItem, file.ModifiedBy, file.FolderID.ToString(), updated ? 1 : 0)
                };
        }
    }
}