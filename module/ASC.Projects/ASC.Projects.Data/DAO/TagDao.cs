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
using ASC.Projects.Core.DataInterfaces;

namespace ASC.Projects.Data.DAO
{
    class TagDao : BaseDao, ITagDao
    {
        public TagDao(string dbId, int tenantID)
            : base(dbId, tenantID)
        {
        }

        public string GetById(int id)
        {
            using (var db = new DbManager(DatabaseId))
            {
                return db.ExecuteScalar<string>(Query(TagsTable).Select("title").Where("id", id));
            }
        }

        public Dictionary<int, string> GetTags()
        {
            using (var db = new DbManager(DatabaseId))
            {
                return db.ExecuteList(GetTagQuery()).ToDictionary(r => Convert.ToInt32(r[0]), n => n[1].ToString().HtmlEncode());
            }
        }

        public Dictionary<int, string> GetTags(string prefix)
        {
            using (var db = new DbManager(DatabaseId))
            {
                var query = GetTagQuery().Where(Exp.Like("title", prefix, SqlLike.StartWith));

                return db.ExecuteList(query).ToDictionary(r => Convert.ToInt32(r[0]), n => n[1].ToString());
            }
        }

        public int[] GetTagProjects(string tagName)
        {
            using (var db = new DbManager(DatabaseId))
            {
                var query = new SqlQuery(ProjectTagTable)
                    .Select("project_id")
                    .InnerJoin(TagsTable, Exp.EqColumns("id", "tag_id") & Exp.Eq("projects_tags.tenant_id", Tenant))
                    .Where(Exp.Eq("lower(title)", tagName.ToLower()))
                    .Where("tenant_id", Tenant);

                return db.ExecuteList(query).ConvertAll(r => Convert.ToInt32(r[0])).ToArray();
            }
        }

        public int[] GetTagProjects(int tagID)
        {
            using (var db = new DbManager(DatabaseId))
            {
                var query = new SqlQuery(ProjectTagTable)
                    .Select("project_id")
                    .Where("tag_id", tagID);

                return db.ExecuteList(query).ConvertAll(r => Convert.ToInt32(r[0])).ToArray();
            }
        }

        public Dictionary<int, string> GetProjectTags(int projectId)
        {
            using (var db = new DbManager(DatabaseId))
            {
                var query = GetTagQuery()
                    .InnerJoin(ProjectTagTable, Exp.EqColumns("id", "tag_id"))
                    .Where(Exp.Eq("project_id", projectId));

                return db.ExecuteList(query).ToDictionary(r => Convert.ToInt32(r[0]), n => n[1].ToString());
            }
        }

        public void SetProjectTags(int projectId, string[] tags)
        {
            using (var db = new DbManager(DatabaseId))
            {
                using (var tx = db.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    var tagsToDelete = db.ExecuteList(
                            new SqlQuery(ProjectTagTable).Select("tag_id").Where("project_id", projectId),
                            r => (int) r[0]);

                    db.ExecuteNonQuery(new SqlDelete(ProjectTagTable).Where("project_id", projectId));

                    foreach (var tag in tagsToDelete)
                    {
                        if (db.ExecuteScalar<int>(new SqlQuery(ProjectTagTable).Select("project_id").Where("tag_id", tag)) == 0)
                        {
                            db.ExecuteNonQuery(Delete(TagsTable).Where("id", tag));
                        }
                    }


                    foreach (var tag in tags)
                    {
                        var tagId = db.ExecuteScalar<int>(Query(TagsTable)
                                                             .Select("id")
                                                             .Where("lower(title)", tag.ToLower()));
                        if (tagId == 0)
                        {
                            tagId = db.ExecuteScalar<int>(
                                Insert(TagsTable)
                                    .InColumnValue("id", 0)
                                    .InColumnValue("title", tag)
                                    .InColumnValue("last_modified_by", DateTime.UtcNow)
                                    .Identity(1, 0, true));
                        }

                        db.ExecuteNonQuery(new SqlInsert(ProjectTagTable, true).InColumnValue("tag_id", tagId).InColumnValue("project_id", projectId));
                    }
                    tx.Commit();
                }
            }
        }


        private SqlQuery GetTagQuery()
        {
            return Query(TagsTable)
                .Select("id", "title")
                .OrderBy("title", true);
        }
    }
}
