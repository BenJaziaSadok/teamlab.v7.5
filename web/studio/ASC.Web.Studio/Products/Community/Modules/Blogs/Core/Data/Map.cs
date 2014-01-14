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

#region Usings

using System;
using System.Data;
using ASC.Common.Data;
using ASC.Blogs.Core.Domain;

#endregion

namespace ASC.Blogs.Core.Data
{
    static class RowMappers
    {
        public static Blog ToBlog(IDataRecord row)
        {
            return new Blog
                       {
                           BlogID = row.Get<int>("id"),
                           Name = row.Get<string>("name"),
                           UserID = row.Get<Guid>("user_id"),
                           GroupID = row.Get<Guid>("group_id"),
                       };
        }

        public static string ToString(IDataRecord row)
        {
            return row.Get<string>("name");
        }

        public static Tag ToTag(IDataRecord row)
        {
            return new Tag
                       {
                           Content = row.Get<string>("name"),
                           PostId = row.Get<Guid>("post_id"),
                       };
        }

        public static Comment ToComment(IDataRecord row)
        {
            return new Comment
                       {
                           ID = row.Get<Guid>("id"),
                           PostId = row.Get<Guid>("post_id"),
                           Content = row.Get<string>("content"),
                           UserID = row.Get<Guid>("created_by"),
                           Datetime = ASC.Core.Tenants.TenantUtil.DateTimeFromUtc(row.Get<DateTime>("created_when")),
                           ParentId = row.Get<Guid>("parent_id"),
                           Inactive = row.Get<int>("inactive") > 0
                       };
        }

        public static Post ToPost(IDataRecord row, bool withContent)
        {
            return new Post
                       {
                           ID = row.Get<Guid>("id"),
                           Title = row.Get<string>("title"),
                           Content = withContent ? row.Get<string>("content") : null,
                           UserID = row.Get<Guid>("created_by"),
                           Datetime = ASC.Core.Tenants.TenantUtil.DateTimeFromUtc(row.Get<DateTime>("created_when")),
                           BlogId = row.Get<int>("blog_id")
                       };
        }
    }
}
