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
using ASC.Data.Backup.Tasks.Data;

namespace ASC.Data.Backup.Tasks.Modules
{
    internal class CommunityModuleSpecifics : ModuleSpecificsBase
    {
        private readonly TableInfo[] _tables = new[]
            {
                new TableInfo("bookmarking_bookmark") {AutoIncrementColumn = "ID", TenantColumn = "tenant", UserIDColumns = new[]{"UserCreatorID"}},
                new TableInfo("bookmarking_bookmarktag") {TenantColumn =  "tenant"},
                new TableInfo("bookmarking_comment") {TenantColumn = "tenant", GuidIDColumn = "ID", UserIDColumns = new[] {"UserID"}},
                new TableInfo("bookmarking_tag"){AutoIncrementColumn = "TagID", TenantColumn = "tenant"},
                new TableInfo("bookmarking_userbookmark") {AutoIncrementColumn = "UserBookmarkID", TenantColumn = "tenant", UserIDColumns = new[]{"UserID"}},
                new TableInfo("bookmarking_userbookmarktag"){TenantColumn = "tenant"},
                new TableInfo("blogs_comments") {TenantColumn = "Tenant", GuidIDColumn = "id", UserIDColumns = new[] {"created_by"}},
                new TableInfo("blogs_posts") {TenantColumn = "Tenant", GuidIDColumn = "id", UserIDColumns = new[] {"created_by"}},
                new TableInfo("blogs_reviewposts") {TenantColumn = "Tenant", UserIDColumns = new[] {"reviewed_by"}},
                new TableInfo("blogs_tags") {TenantColumn = "Tenant"},
                new TableInfo("events_comment") {AutoIncrementColumn = "Id", TenantColumn = "Tenant", UserIDColumns = new[] {"Creator"}},
                new TableInfo("events_feed") {AutoIncrementColumn = "Id", TenantColumn = "Tenant", UserIDColumns = new[] {"Creator"}},
                new TableInfo("events_poll"){TenantColumn = "Tenant"},
                new TableInfo("events_pollanswer") {TenantColumn = "Tenant", UserIDColumns = new[] {"User"}}, //todo: check //varchar(64)?
                new TableInfo("events_pollvariant") {AutoIncrementColumn = "Id", TenantColumn = "Tenant"},
                new TableInfo("events_reader") {TenantColumn = "Tenant", UserIDColumns = new[] {"Reader"}}, //todo: check
                new TableInfo("forum_answer") {AutoIncrementColumn = "id", TenantColumn = "TenantID", UserIDColumns = new[] {"user_id"}},
                new TableInfo("forum_answer_variant"),
                new TableInfo("forum_attachment") {AutoIncrementColumn = "id", TenantColumn = "TenantID"},
                new TableInfo("forum_category") {AutoIncrementColumn = "id", TenantColumn = "TenantID", UserIDColumns = new[] {"poster_id"}},
                new TableInfo("forum_lastvisit") {TenantColumn = "tenantid", UserIDColumns = new[] {"user_id"}},
                new TableInfo("forum_post") {AutoIncrementColumn = "id", TenantColumn = "TenantID", UserIDColumns = new[] {"poster_id", "editor_id"}},
                new TableInfo("forum_question") {AutoIncrementColumn = "id", TenantColumn = "TenantID"},
                new TableInfo("forum_tag") {AutoIncrementColumn = "id", TenantColumn = "TenantID"},
                new TableInfo("forum_thread") {AutoIncrementColumn = "id", TenantColumn = "TenantID", UserIDColumns = new[] {"recent_poster_id"}},
                new TableInfo("forum_topic") {AutoIncrementColumn = "id", TenantColumn = "TenantID", UserIDColumns = new[] {"poster_id"}},
                new TableInfo("forum_topicwatch") {TenantColumn = "TenantID", UserIDColumns = new[] {"UserID"}},
                new TableInfo("forum_topic_tag"),
                new TableInfo("forum_variant") {AutoIncrementColumn = "id"},
                new TableInfo("wiki_categories") {TenantColumn =  "Tenant"},
                new TableInfo("wiki_comments") {TenantColumn = "Tenant", GuidIDColumn = "Id", UserIDColumns = new[] {"UserId"}},
                new TableInfo("wiki_files") {TenantColumn = "Tenant", UserIDColumns = new[] {"UserID"}},
                new TableInfo("wiki_pages") {TenantColumn = "tenant", UserIDColumns = new[] {"modified_by"}},
                new TableInfo("wiki_pages_history") {TenantColumn = "Tenant", UserIDColumns = new[] {"create_by"}} 
            };

        private readonly RelationInfo[] _tableRelations = new[]
            {
                new RelationInfo("bookmarking_bookmark", "ID", "bookmarking_bookmarktag", "BookmarkID"),
                new RelationInfo("bookmarking_tag", "TagID", "bookmarking_bookmarktag", "TagID"),
                new RelationInfo("bookmarking_bookmark", "ID", "bookmarking_comment", "BookmarkID"),
                new RelationInfo("bookmarking_comment", "ID", "bookmarking_comment", "Parent"), 
                new RelationInfo("bookmarking_bookmark", "ID", "bookmarking_userbookmark", "BookmarkID"),
                new RelationInfo("bookmarking_tag", "TagID", "bookmarking_userbookmarktag", "TagID"),
                new RelationInfo("bookmarking_userbookmark", "UserBookmarkID", "bookmarking_userbookmarktag", "UserBookmarkID"),
                new RelationInfo("blogs_comments", "id", "blogs_comments", "parent_id"),
                new RelationInfo("blogs_posts", "id", "blogs_comments", "post_id"),
                new RelationInfo("blogs_comments", "id", "blogs_posts", "LastCommentId", null, null, RelationImportance.Low),
                new RelationInfo("blogs_posts", "id", "blogs_reviewposts", "post_id"),
                new RelationInfo("blogs_posts", "id", "blogs_tags", "post_id"),
                new RelationInfo("events_feed", "Id", "events_comment", "Feed"),
                new RelationInfo("events_comment", "Id", "events_comment", "Parent"),
                new RelationInfo("events_feed", "Id", "events_poll", "Id"), 
                new RelationInfo("events_pollvariant", "Id", "events_pollanswer", "Variant"),
                new RelationInfo("events_feed", "Id", "events_pollvariant", "Poll"),
                new RelationInfo("events_feed", "Id", "events_reader", "Feed"),
                new RelationInfo("forum_question", "id", "forum_answer", "question_id"),
                new RelationInfo("forum_answer", "id", "forum_answer_variant", "answer_id"),
                new RelationInfo("forum_variant", "id", "forum_answer_variant", "variant_id"),
                new RelationInfo("forum_post", "id", "forum_attachment", "post_id"),
                new RelationInfo("forum_category", "id", "forum_attachment", "path"),
                new RelationInfo("forum_thread", "id", "forum_attachment", "path"), 
                new RelationInfo("forum_thread", "id", "forum_lastvisit", "thread_id"),
                new RelationInfo("forum_topic", "id", "forum_post", "topic_id"),
                new RelationInfo("forum_post", "id", "forum_post", "parent_post_id"),
                new RelationInfo("forum_topic", "id", "forum_question", "topic_id"),
                new RelationInfo("forum_category", "id", "forum_thread", "category_id"),
                new RelationInfo("forum_post", "id", "forum_thread", "recent_post_id", null, null, RelationImportance.Low),
                new RelationInfo("forum_topic", "id", "forum_thread", "recent_topic_id", null, null, RelationImportance.Low),
                new RelationInfo("forum_thread", "id", "forum_topic", "thread_id"),
                new RelationInfo("forum_question", "id", "forum_topic", "question_id", null, null, RelationImportance.Low),
                new RelationInfo("forum_post", "id", "forum_topic", "recent_post_id", null, null, RelationImportance.Low),
                new RelationInfo("forum_topic", "id", "forum_topicwatch", "TopicID"),
                new RelationInfo("forum_topic", "id", "forum_topic_tag", "topic_id"),
                new RelationInfo("forum_tag", "id", "forum_topic_tag", "tag_id"),
                new RelationInfo("forum_question", "id", "forum_variant", "question_id"),
                new RelationInfo("wiki_comments", "Id", "wiki_comments", "ParentId") 
            };

        public override ModuleName ModuleName
        {
            get { return ModuleName.Community; }
        }

        public override IEnumerable<TableInfo> Tables
        {
            get { return _tables; }
        }

        public override IEnumerable<RelationInfo> TableRelations
        {
            get { return _tableRelations; }
        }

        public override bool TryAdjustFilePath(ColumnMapper columnMapper, ref string filePath)
        {
            filePath = PreparePath(columnMapper, "/", filePath);
            return filePath != null;
        }

        protected override bool TryPrepareValue(IDbConnection connection, ColumnMapper columnMapper, TableInfo table, string columnName, IEnumerable<RelationInfo> relations, ref object value)
        {
            relations = relations.ToList();
            
            if (relations.All(x => x.ChildTable == "forum_attachment" && x.ChildColumn == "path"))
            {
                value = PreparePath(columnMapper, "\\", Convert.ToString(value));
                return value != null;
            }

            return base.TryPrepareValue(connection, columnMapper, table, columnName, relations, ref value);
        }

        protected override string GetSelectCommandConditionText(int tenantId, TableInfo table)
        {
            if (table.Name == "forum_answer_variant")
                return "inner join forum_answer as t1 on t1.id = t.answer_id where t1.TenantID = " + tenantId;

            if (table.Name == "forum_variant")
                return "inner join forum_question as t1 on t1.id = t.question_id where t1.TenantID = " + tenantId;

            if (table.Name == "forum_topic_tag")
                return "inner join forum_topic as t1 on t1.id = t.topic_id where t1.TenantID = " + tenantId;

            return base.GetSelectCommandConditionText(tenantId, table);
        }


        private static string PreparePath(ColumnMapper columnMapper, string partsSeparator, string path)
        {
            string[] parts = path.Split(new[] {partsSeparator}, StringSplitOptions.None);

            if (parts.Length != 4)
                return null;

            var categoryId = columnMapper.GetMapping("forum_category", "id", parts[0]);
            if (categoryId == null)
                return null;

            var threadId = columnMapper.GetMapping("forum_thread", "id", parts[1]);
            if (threadId == null)
                return null;

            parts[0] = categoryId.ToString();
            parts[1] = threadId.ToString();

            return string.Join(partsSeparator, parts);
        }
    }
}
