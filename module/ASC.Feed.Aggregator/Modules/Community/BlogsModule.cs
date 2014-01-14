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
using ASC.Blogs.Core.Domain;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Web.Core.Security;
using ASC.Web.Studio.Utility;
using System.Linq;

namespace ASC.Feed.Aggregator.Modules.Community
{
    internal class BlogsModule : FeedModule
    {
        private const string item = "blog";


        protected override string Table
        {
            get { return "blogs_posts"; }
        }

        protected override string LastUpdatedColumn
        {
            get { return "created_when"; }
        }

        protected override string TenantColumn
        {
            get { return "tenant"; }
        }

        protected override string DbId
        {
            get { return Constants.CommunityDbId; }
        }


        public override string Name
        {
            get { return Constants.BlogsModule; }
        }

        public override string Product
        {
            get { return ModulesHelper.CommunityProductName; }
        }

        public override Guid ProductID
        {
            get { return ModulesHelper.CommunityProductID; }
        }

        public override IEnumerable<int> GetTenantsWithFeeds(DateTime fromTime)
        {
            var q1 = new SqlQuery("blogs_posts")
                .Select("tenant")
                .Where(Exp.Gt("created_when", fromTime))
                .GroupBy(1)
                .Having(Exp.Gt("count(*)", 0));

            var q2 = new SqlQuery("blogs_comments")
                .Select("tenant")
                .Where(Exp.Gt("created_when", fromTime))
                .GroupBy(1)
                .Having(Exp.Gt("count(*)", 0));

            using (var db = new DbManager(DbId))
            {
                return db.ExecuteList(q1)
                         .ConvertAll(r => Convert.ToInt32(r[0]))
                         .Union(db.ExecuteList(q2).ConvertAll(r => Convert.ToInt32(r[0])));
            }
        }

        public override IEnumerable<Tuple<Feed, object>> GetFeeds(FeedFilter filter)
        {
            var query = new SqlQuery("blogs_posts p")
                .Select(BlogColumns().Select(p => "p." + p).ToArray())
                .LeftOuterJoin("blogs_comments c",
                               Exp.EqColumns("c.post_id", "p.id") &
                               Exp.Eq("c.inactive", 0) &
                               Exp.Eq("c.tenant", filter.Tenant)
                )
                .Select(BlogCommentColumns().Select(c => "c." + c).ToArray())
                .Where("p.tenant", filter.Tenant)
                .Where(Exp.Between("p.created_when", filter.Time.From, filter.Time.To) |
                       Exp.Between("c.created_when", filter.Time.From, filter.Time.To));

            using (var db = new DbManager(DbId))
            {
                var comments = db.ExecuteList(query)
                    .ConvertAll(ToComment);
                var groupedBlogs = comments.GroupBy(c => c.Post.ID);

                return groupedBlogs
                    .Select(b => new Tuple<Post, IEnumerable<Comment>>(b.First().Post, b))
                    .Select(ToFeed);
            }
        }


        private static IEnumerable<string> BlogColumns()
        {
            return new[]
                {
                    "id",
                    "title",
                    "content",
                    "created_by",
                    "created_when" // 4
                };
        }

        private static IEnumerable<string> BlogCommentColumns()
        {
            return new[]
                {
                    "id", // 5
                    "content",
                    "created_by",
                    "created_when" // 8
                };
        }

        private static Comment ToComment(object[] r)
        {
            var comment = new Comment
                {
                    Post = new Post
                        {
                            ID = new Guid(Convert.ToString(r[0])),
                            Title = Convert.ToString(r[1]),
                            Content = Convert.ToString(r[2]),
                            UserID = new Guid(Convert.ToString(r[3])),
                            Datetime = Convert.ToDateTime(r[4])
                        }
                };

            if (r[5] != null)
            {
                comment.ID = new Guid(Convert.ToString(r[5]));
                comment.Content = Convert.ToString(r[6]);
                comment.UserID = new Guid(Convert.ToString(r[7]));
                comment.Datetime = Convert.ToDateTime(r[8]);
            }
            return comment;
        }

        private Tuple<Feed, object> ToFeed(Tuple<Post, IEnumerable<Comment>> p)
        {
            var post = p.Item1;

            var itemUrl = "/products/community/modules/blogs/viewblog.aspx?blogid=" + post.ID;
            var commentApiUrl = "/api/2.0/community/blog/" + post.ID + "/comment.json";

            var comments = p.Item2.Where(c => c.ID != Guid.Empty).OrderBy(c => c.Datetime).ToList();
            var feedDate = comments.Any() ? comments.First().Datetime : post.Datetime;
            var feedAutohor = comments.Any() ? comments.Last().UserID : post.UserID;

            var feed = new Feed(post.UserID, feedDate, true)
                {
                    Item = item,
                    ItemId = post.ID.ToString(),
                    ItemUrl = CommonLinkUtility.ToAbsolute(itemUrl),
                    LastModifiedBy = feedAutohor,
                    Product = Product,
                    Module = Name,
                    Action = comments.Any() ? FeedAction.Commented : FeedAction.Created,
                    Title = post.Title,
                    Description = HtmlSanitizer.Sanitize(post.Content),
                    HasPreview = post.Content.Contains("class=\"asccut\""),
                    CanComment = true,
                    CommentApiUrl = CommonLinkUtility.ToAbsolute(commentApiUrl),
                    Comments = comments.Select(ToFeedComment),
                    GroupId = string.Format("{0}_{1}", item, post.ID)
                };
            feed.Keywords = string.Format("{0} {1} {2}",
                                          post.Title,
                                          Helper.GetText(post.Content),
                                          string.Join(" ", feed.Comments.Select(x => x.Description)));

            return new Tuple<Feed, object>(feed, post);
        }

        private static FeedComment ToFeedComment(Comment comment)
        {
            return new FeedComment(comment.UserID)
                {
                    Id = comment.ID.ToString(),
                    Description = HtmlSanitizer.Sanitize(comment.Content),
                    Date = comment.Datetime
                };
        }
    }
}