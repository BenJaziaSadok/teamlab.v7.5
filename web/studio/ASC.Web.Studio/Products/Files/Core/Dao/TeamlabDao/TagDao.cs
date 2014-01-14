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
using System.Linq;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core.Tenants;

namespace ASC.Files.Core.Data
{
    internal class TagDao : AbstractDao, ITagDao
    {
        private static readonly object syncRoot = new object();

        public TagDao(int tenant, string dbid)
            : base(tenant, dbid)
        {
        }

        public IEnumerable<Tag> GetTags(object entryID, FileEntryType entryType, TagType tagType)
        {
            var q = Query("files_tag t")
                .InnerJoin("files_tag_link l", Exp.EqColumns("l.tag_id", "t.id"))
                .Select("t.name", "t.flag", "t.owner", "entry_id", "entry_type", "tag_count", "t.id")
                .Where("l.tenant_id", TenantID)
                .Where("l.entry_type", (int) entryType)
                .Where(Exp.Eq("l.entry_id", MappingID(entryID)))
                .Where("t.flag", (int) tagType);

            return SelectTagByQuery(q);
        }

        public IEnumerable<Tag> GetTags(string[] names, TagType tagType)
        {
            if (names == null) throw new ArgumentNullException("names");

            var q = Query("files_tag t")
                .InnerJoin("files_tag_link l", Exp.EqColumns("l.tag_id", "t.id"))
                .Select("t.name", "t.flag", "t.owner", "entry_id", "entry_type", "tag_count", "t.id")
                .Where("l.tenant_id", TenantID)
                .Where("t.owner", Guid.Empty.ToString())
                .Where(Exp.In("t.name", names))
                .Where("t.flag", (int) tagType);

            return SelectTagByQuery(q);
        }

        private IEnumerable<Tag> SelectTagByQuery(SqlQuery q)
        {
            using (var DbManager = GetDbManager())
            {
                return DbManager.ExecuteList(q).ConvertAll(r => ToTag(r));
            }
        }

        public IEnumerable<Tag> GetTags(string name, TagType tagType)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            return GetTags(new[] {name}, tagType);
        }

        public IEnumerable<Tag> GetTags(Guid owner, TagType tagType)
        {
            var q = Query("files_tag t")
                .InnerJoin("files_tag_link l", Exp.EqColumns("l.tag_id", "t.id"))
                .Select("t.name", "t.flag", "t.owner", "entry_id", "entry_type", "tag_count", "t.id")
                .Where("l.tenant_id", TenantID)
                .Where("t.flag", (int) tagType);

            if (owner != Guid.Empty)
                q.Where("t.owner", owner.ToString());

            return SelectTagByQuery(q);
        }

        public IEnumerable<Tag> SaveTags(params Tag[] tags)
        {
            var result = new List<Tag>();

            if (tags == null) return result;

            tags = tags.Where(x => !x.EntryId.Equals(null) && !x.EntryId.Equals(0)).ToArray();

            if (tags.Length == 0) return result;

            lock (syncRoot)
            {
                using (var DbManager = GetDbManager())
                using (var tx = DbManager.BeginTransaction())
                {
                    var mustBeDeleted = DbManager.ExecuteList(Query("files_tag ft")
                                                                  .Select("ftl.tag_id",
                                                                          "ftl.entry_id",
                                                                          "ftl.entry_type")
                                                                  .Distinct()
                                                                  .InnerJoin("files_tag_link ftl",
                                                                             Exp.EqColumns("ft.tenant_id", "ftl.tenant_id") &
                                                                             Exp.EqColumns("ft.id", "ftl.tag_id"))
                                                                  .Where(Exp.Eq("ft.flag", (int) TagType.New) &
                                                                         Exp.Le("create_on", TenantUtil.DateTimeNow().AddMonths(-3))));

                    foreach (var row in mustBeDeleted)
                    {
                        DbManager.ExecuteNonQuery(Delete("files_tag_link")
                                                      .Where(Exp.Eq("tag_id", row[0]) &
                                                             Exp.Eq("entry_id", row[1]) &
                                                             Exp.Eq("entry_type", row[2])));
                    }

                    DbManager.ExecuteNonQuery(Delete("files_tag").Where(Exp.EqColumns("0", Query("files_tag_link l").SelectCount().Where(Exp.EqColumns("tag_id", "id")))));

                    var createOn = TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow());

                    var cacheTagId = new Dictionary<String, int>();

                    foreach (var t in tags)
                    {
                        int id;

                        var cacheTagIdKey = String.Join("/", new[] {TenantID.ToString(), t.Owner.ToString(), t.TagName, ((int) t.TagType).ToString(CultureInfo.InvariantCulture)});

                        if (!cacheTagId.TryGetValue(cacheTagIdKey, out id))
                        {
                            id = DbManager.ExecuteScalar<int>(Query("files_tag")
                                                                  .Select("id")
                                                                  .Where("owner", t.Owner.ToString())
                                                                  .Where("name", t.TagName)
                                                                  .Where("flag", (int) t.TagType));

                            if (id == 0)
                            {
                                var i1 = Insert("files_tag")
                                    .InColumnValue("id", 0)
                                    .InColumnValue("name", t.TagName)
                                    .InColumnValue("owner", t.Owner.ToString())
                                    .InColumnValue("flag", (int) t.TagType)
                                    .Identity(1, 0, true);
                                id = DbManager.ExecuteScalar<int>(i1);
                            }

                            cacheTagId.Add(cacheTagIdKey, id);
                        }

                        t.Id = id;
                        result.Add(t);

                        var i2 = Insert("files_tag_link")
                            .InColumnValue("tag_id", id)
                            .InColumnValue("entry_id", MappingID(t.EntryId, true))
                            .InColumnValue("entry_type", (int) t.EntryType)
                            .InColumnValue("create_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                            .InColumnValue("create_on", createOn)
                            .InColumnValue("tag_count", t.Count);

                        DbManager.ExecuteNonQuery(i2);
                    }

                    tx.Commit();
                }
            }

            return result;
        }

        public void UpdateNewTags(params Tag[] tags)
        {
            if (tags == null || tags.Length == 0) return;

            lock (syncRoot)
            {
                using (var DbManager = GetDbManager())
                using (var tx = DbManager.BeginTransaction(true))
                {
                    var createOn = TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow());

                    foreach (var tag in tags)
                    {
                        DbManager.ExecuteNonQuery(
                            Update("files_tag_link")
                                .Set("create_by", ASC.Core.SecurityContext.CurrentAccount.ID)
                                .Set("create_on", createOn)
                                .Set("tag_count", tag.Count)
                                .Where("tag_id", tag.Id)
                                .Where("entry_type", (int) tag.EntryType)
                                .Where("entry_id", MappingID(tag.EntryId))
                            );
                    }
                    tx.Commit();
                }
            }
        }

        public void RemoveTags(params Tag[] tags)
        {
            if (tags == null || tags.Length == 0) return;

            lock (syncRoot)
            {
                using (var DbManager = GetDbManager())
                using (var tx = DbManager.BeginTransaction(true))
                {
                    foreach (var t in tags)
                    {
                        var id = DbManager.ExecuteScalar<int>(Query("files_tag")
                                                                  .Select("id")
                                                                  .Where("name", t.TagName)
                                                                  .Where("owner", t.Owner.ToString())
                                                                  .Where("flag", (int) t.TagType));
                        if (id != 0)
                        {
                            var d = Delete("files_tag_link")
                                .Where("tag_id", id)
                                .Where("entry_id", MappingID(t.EntryId))
                                .Where("entry_type", (int) t.EntryType);
                            DbManager.ExecuteNonQuery(d);

                            var count = DbManager.ExecuteScalar<int>(Query("files_tag_link").SelectCount().Where("tag_id", id));
                            if (count == 0)
                            {
                                d = Delete("files_tag").Where("id", id);
                                DbManager.ExecuteNonQuery(d);
                            }
                        }
                    }
                    tx.Commit();
                }
            }
        }

        public void RemoveTags(params int[] ids)
        {
            if (ids == null || ids.Length == 0) return;

            lock (syncRoot)
            {
                using (var DbManager = GetDbManager())
                using (var tx = DbManager.BeginTransaction())
                {
                    DbManager.ExecuteNonQuery(Delete("files_tag_link").Where(Exp.In("tag_id", ids)));
                    DbManager.ExecuteNonQuery(Delete("files_tag").Where(Exp.In("id", ids)));
                    tx.Commit();
                }
            }
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, params FileEntry[] fileEntries)
        {
            using (var DbManager = GetDbManager())
            {
                var result = new List<Tag>();

                if (!fileEntries.Any())
                    return result;

                using (var tx = DbManager.BeginTransaction())
                {
                    var sqlQuery = Query("files_tag ft")
                        .Select("ft.name",
                                "ft.flag",
                                "ft.owner",
                                "ftl.entry_id",
                                "ftl.entry_type",
                                "ftl.tag_count",
                                "ft.id")
                        .Distinct()
                        .InnerJoin("files_tag_link ftl",
                                   Exp.EqColumns("ft.tenant_id", "ftl.tenant_id") &
                                   Exp.EqColumns("ft.id", "ftl.tag_id"))
                        .Where(Exp.Eq("ft.flag", (int) TagType.New));

                    if (subject != Guid.Empty)
                        sqlQuery.Where(Exp.Eq("ft.owner", subject));

                    var sqlQueryStr =
                        @"
                                    CREATE  TEMPORARY TABLE IF NOT EXISTS `files_tag_temporary` (
                                    `tenant_id` INT(10) NOT NULL,
	                                `entry_id` VARCHAR(255) NOT NULL,
	                                `entry_type` INT(10) NOT NULL,
	                                PRIMARY KEY (`tenant_id`, `entry_id`, `entry_type`)
                                );";

                    DbManager.ExecuteNonQuery(sqlQueryStr);

                    foreach (var fileEntrie in fileEntries)
                        DbManager.ExecuteNonQuery(
                            Insert("files_tag_temporary")
                                .InColumnValue("entry_id", MappingID(fileEntrie.ID))
                                .InColumnValue("entry_type", (fileEntrie is File) ? (int) FileEntryType.File : (int) FileEntryType.Folder));

                    var resultSql = sqlQuery
                        .InnerJoin("files_tag_temporary ftt", Exp.EqColumns("ftl.tenant_id", "ftt.tenant_id") &
                                                              Exp.EqColumns("ftl.entry_id", "ftt.entry_id") &
                                                              Exp.EqColumns("ftl.entry_type", "ftt.entry_type"));

                    result = DbManager.ExecuteList(resultSql).ConvertAll(x => ToTag(x)).Where(x => x.EntryId != null).ToList();

                    DbManager.ExecuteNonQuery(Delete("files_tag_temporary"));

                    tx.Commit();
                }

                return result;
            }
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, Folder parentFolder, bool deepSearch)
        {
            using (var DbManager = GetDbManager())
            {
                if (parentFolder == null || parentFolder.ID == null)
                    throw new ArgumentException("folderId");

                var result = new List<Tag>();

                var monitorFolderIds = new[] {parentFolder.ID}.AsEnumerable();

                var getBaseSqlQuery = new Func<SqlQuery>(() =>
                                                             {
                                                                 var fnResult =
                                                                     Query("files_tag ft")
                                                                         .Select("ft.name",
                                                                                 "ft.flag",
                                                                                 "ft.owner",
                                                                                 "ftl.entry_id",
                                                                                 "ftl.entry_type",
                                                                                 "ftl.tag_count",
                                                                                 "ft.id")
                                                                         .Distinct()
                                                                         .InnerJoin("files_tag_link ftl",
                                                                                    Exp.EqColumns("ft.tenant_id", "ftl.tenant_id") &
                                                                                    Exp.EqColumns("ft.id", "ftl.tag_id"))
                                                                         .Where(Exp.Eq("ft.flag", (int) TagType.New));

                                                                 if (subject != Guid.Empty)
                                                                     fnResult.Where(Exp.Eq("ft.owner", subject));

                                                                 return fnResult;
                                                             });

                var tempTags = Enumerable.Empty<Tag>();

                if (parentFolder.FolderType == FolderType.SHARE)
                {
                    var shareQuery =
                        new Func<SqlQuery>(() => getBaseSqlQuery().InnerJoin("files_security fs",
                                                                             Exp.EqColumns("fs.tenant_id", "ftl.tenant_id") &
                                                                             Exp.EqColumns("fs.entry_id", "ftl.entry_id") &
                                                                             Exp.EqColumns("fs.entry_type", "ftl.entry_type")));

                    var tmpShareFileTags = DbManager.ExecuteList(
                        shareQuery().InnerJoin("files_file f",
                                               !Exp.Eq("f.create_by", subject) &
                                               Exp.EqColumns("f.id", "ftl.entry_id") &
                                               Exp.Eq("ftl.entry_type", FileEntryType.File))
                                    .Select(GetRootFolderType("folder_id")))
                                                    .Where(r => ParseRootFolderType(r[7]) == FolderType.USER).ToList()
                                                    .ConvertAll(r => ToTag(r));
                    tempTags = tempTags.Concat(tmpShareFileTags);


                    var tmpShareFolderTags = DbManager.ExecuteList(
                        shareQuery().InnerJoin("files_folder f",
                                               !Exp.Eq("f.create_by", subject) &
                                               Exp.EqColumns("f.id", "ftl.entry_id") &
                                               Exp.Eq("ftl.entry_type", FileEntryType.Folder))
                                    .Select(GetRootFolderType("parent_id")))
                                                      .Where(r => ParseRootFolderType(r[7]) == FolderType.USER).ToList()
                                                      .ConvertAll(r => ToTag(r));
                    tempTags = tempTags.Concat(tmpShareFolderTags);

                    var tmpShareSboxTags = DbManager.ExecuteList(
                        shareQuery()
                            .InnerJoin("files_thirdparty_id_mapping m",
                                       Exp.EqColumns("m.hash_id", "ftl.entry_id"))
                            .InnerJoin("files_thirdparty_account ac",
                                       Exp.Sql("m.id Like concat('sbox-', ac.id, '%')") &
                                       !Exp.Eq("ac.user_id", subject) &
                                       Exp.Eq("ac.folder_type", FolderType.USER)
                            )
                        ).ConvertAll(r => ToTag(r));

                    tempTags = tempTags.Concat(tmpShareSboxTags);
                }
                else if (parentFolder.FolderType == FolderType.Projects)
                    tempTags = DbManager.ExecuteList(getBaseSqlQuery().InnerJoin("files_bunch_objects fbo",
                                                                                 Exp.EqColumns("fbo.tenant_id", "ftl.tenant_id") &
                                                                                 Exp.EqColumns("fbo.left_node", "ftl.entry_id") &
                                                                                 Exp.Eq("ftl.entry_type", (int) FileEntryType.Folder))
                                                                      .Where(Exp.Eq("fbo.tenant_id", TenantID) & Exp.Like("fbo.right_node", "projects/project/", SqlLike.StartWith)))
                                        .ConvertAll(r => ToTag(r));

                if (tempTags.Any())
                {
                    if (!deepSearch) return tempTags;

                    monitorFolderIds = monitorFolderIds.Concat(tempTags.Where(x => x.EntryType == FileEntryType.Folder).Select(x => x.EntryId));
                    result.AddRange(tempTags);
                }

                var subFoldersSqlQuery = new SqlQuery("files_folder_tree")
                    .Select("folder_id")
                    .Where(Exp.In("parent_id", monitorFolderIds.ToArray()));

                if (!deepSearch)
                    subFoldersSqlQuery.Where(Exp.Eq("level", 1));

                monitorFolderIds = monitorFolderIds.Concat(DbManager.ExecuteList(subFoldersSqlQuery).ConvertAll(x => x[0]));

                var newTagsForFolders = DbManager.ExecuteList(getBaseSqlQuery()
                                                                  .Where(Exp.In("ftl.entry_id", monitorFolderIds.ToArray()) &
                                                                         Exp.Eq("ftl.entry_type", (int) FileEntryType.Folder)))
                                                 .ConvertAll(r => ToTag(r));

                result.AddRange(newTagsForFolders);

                var newTagsForFiles = DbManager.ExecuteList(getBaseSqlQuery()
                                                                .InnerJoin("files_file ff",
                                                                           Exp.EqColumns("ftl.tenant_id", "ff.tenant_id") &
                                                                           Exp.EqColumns("ftl.entry_id", "ff.id"))
                                                                .Where(Exp.In("ff.folder_id", (deepSearch
                                                                                                   ? monitorFolderIds.ToArray()
                                                                                                   : new[] {parentFolder.ID})) &
                                                                       Exp.Eq("ftl.entry_type", (int) FileEntryType.File)))
                                               .ConvertAll(r => ToTag(r));

                result.AddRange(newTagsForFiles);

                if (parentFolder.FolderType == FolderType.USER || parentFolder.FolderType == FolderType.COMMON)
                {
                    var folderType = parentFolder.FolderType;

                    var querySelect = new SqlQuery("files_thirdparty_account")
                        .Select("id")
                        .Where("tenant_id", TenantID)
                        .Where("folder_type", (int) folderType);

                    if (folderType == FolderType.USER)
                        querySelect = querySelect.Where(Exp.Eq("user_id", subject.ToString()));

                    var sboxFolderIds = DbManager.ExecuteList(querySelect).ConvertAll(r => "sbox-" + r[0]);

                    var newTagsForSBox = DbManager.ExecuteList(getBaseSqlQuery()
                                                                   .InnerJoin("files_thirdparty_id_mapping mp", Exp.EqColumns("ftl.entry_id", "mp.hash_id"))
                                                                   .Where(Exp.In("mp.id", sboxFolderIds) &
                                                                          Exp.Eq("ft.owner", subject) &
                                                                          Exp.Eq("ftl.entry_type", (int) FileEntryType.Folder)))
                                                  .ConvertAll(r => ToTag(r));

                    result.AddRange(newTagsForSBox);
                }

                return result;
            }
        }

        private Tag ToTag(object[] r)
        {
            var result = new Tag((string) r[0], (TagType) Convert.ToInt32(r[1]), new Guid((string) r[2]), null, Convert.ToInt32(r[5]))
                {
                    EntryId = MappingID(r[3]),
                    EntryType = (FileEntryType) Convert.ToInt32(r[4]),
                    Id = Convert.ToInt32(r[6]),
                };

            return result;
        }
    }
}