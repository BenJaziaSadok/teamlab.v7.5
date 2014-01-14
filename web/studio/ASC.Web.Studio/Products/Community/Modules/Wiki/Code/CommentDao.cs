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

namespace ASC.Web.UserControls.Wiki.Data
{
    class CommentDao : BaseDao
    {
        public CommentDao(string dbid, int tenant)
            : base(dbid, tenant)
        {
        }


        public List<Comment> GetComments(string pageName)
        {
            var q = Query("wiki_comments")
                .Select("Id", "ParentId", "PageName", "Body", "UserId", "Date", "Inactive")
                .Where("PageName", pageName)
                .OrderBy("Date", true);

            return db.ExecuteList(q)
                .ConvertAll(r => ToComment(r));
        }

        public Comment GetComment(Guid id)
        {
            var q = Query("wiki_comments")
                .Select("Id", "ParentId", "PageName", "Body", "UserId", "Date", "Inactive")
                .Where("Id", id.ToString());

            return db.ExecuteList(q)
                .ConvertAll(r => ToComment(r))
                .SingleOrDefault();
        }

        public Comment SaveComment(Comment comment)
        {
            if (comment == null) throw new NotImplementedException("comment");

            if (comment.Id == Guid.Empty) comment.Id = Guid.NewGuid();
            var i = Insert("wiki_comments")
                .InColumnValue("id", comment.Id.ToString())
                .InColumnValue("ParentId", comment.ParentId.ToString())
                .InColumnValue("PageName", comment.PageName)
                .InColumnValue("Body", comment.Body)
                .InColumnValue("UserId", comment.UserId.ToString())
                .InColumnValue("Date", TenantUtil.DateTimeToUtc(comment.Date))
                .InColumnValue("Inactive", comment.Inactive);

            db.ExecuteNonQuery(i);

            return comment;
        }

        public void RemoveComment(Guid id)
        {
            var d = Delete("wiki_comments").Where("id", id.ToString());
            db.ExecuteNonQuery(d);
        }


        private Comment ToComment(object[] r)
        {
            return new Comment
            {
                Id = new Guid((string)r[0]),
                ParentId = new Guid((string)r[1]),
                PageName = (string)r[2],
                Body = (string)r[3],
                UserId = new Guid((string)r[4]),
                Date = TenantUtil.DateTimeFromUtc((DateTime)r[5]),
                Inactive = Convert.ToBoolean(r[6]),
            };
        }
    }
}