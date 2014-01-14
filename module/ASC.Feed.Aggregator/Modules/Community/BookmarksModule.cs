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
using System.Globalization;
using System.Web;
using ASC.Bookmarking.Pojo;
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Web.Core.Security;
using ASC.Web.Studio.Utility;
using System.Linq;

namespace ASC.Feed.Aggregator.Modules.Community
{
    internal class BookmarksModule : FeedModule
    {
        private const string item = "bookmark";


        protected override string Table
        {
            get { return "bookmarking_bookmark"; }
        }

        protected override string LastUpdatedColumn
        {
            get { return "date"; }
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
            get { return Constants.BookmarksModule; }
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
            var q1 = new SqlQuery("bookmarking_bookmark")
                .Select("tenant")
                .Where(Exp.Gt("date", fromTime))
                .GroupBy(1)
                .Having(Exp.Gt("count(*)", 0));

            var q2 = new SqlQuery("bookmarking_comment")
                .Select("tenant")
                .Where(Exp.Gt("datetime", fromTime))
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
            var q = new SqlQuery("bookmarking_bookmark b")
                .Select("b.id", "b.url", "b.name", "b.description", "b.usercreatorid", "b.date")
                .LeftOuterJoin("bookmarking_comment c",
                               Exp.EqColumns("b.id", "c.bookmarkId") &
                               Exp.Eq("c.inactive", 0) &
                               Exp.Eq("c.tenant", filter.Tenant)
                )
                .Select("c.id", "c.content", "c.userId", "c.datetime")
                .Where("b.tenant", filter.Tenant)
                .Where(Exp.Between("b.date", filter.Time.From, filter.Time.To) |
                       Exp.Between("c.datetime", filter.Time.From, filter.Time.To));

            using (var db = new DbManager(DbId))
            {
                var comments = db.ExecuteList(q).ConvertAll(ToComment);
                var groupedBookmarks = comments.GroupBy(c => c.Bookmark.ID);

                return groupedBookmarks
                    .Select(b => new Tuple<Bookmark, IEnumerable<Comment>>(b.First().Bookmark, b))
                    .Select(ToFeed);
            }
        }


        private static Comment ToComment(object[] r)
        {
            var comment = new Comment
                {
                    Bookmark = new Bookmark
                        {
                            ID = Convert.ToInt64(r[0]),
                            URL = Convert.ToString(r[1]),
                            Name = Convert.ToString(r[2]),
                            Description = Convert.ToString(r[3]),
                            UserCreatorID = new Guid(Convert.ToString(r[4])),
                            Date = Convert.ToDateTime(Convert.ToString(r[5]))
                        }
                };

            if (r[6] != null)
            {
                comment.ID = new Guid(Convert.ToString(r[6]));
                comment.Content = Convert.ToString(r[7]);
                comment.UserID = new Guid(Convert.ToString(r[8]));
                comment.Datetime = Convert.ToDateTime(Convert.ToString(r[9]));
            }
            return comment;
        }

        private Tuple<Feed, object> ToFeed(Tuple<Bookmark, IEnumerable<Comment>> b)
        {
            var bookmark = b.Item1;

            var itemUrl = "/products/community/modules/bookmarking/bookmarkinfo.aspx?url=" + HttpUtility.UrlEncode(bookmark.URL);
            var commentApiUrl = "/api/2.0/community/bookmark/" + bookmark.ID + "/comment.json";

            var comments = b.Item2.Where(c => c.ID != Guid.Empty).OrderBy(c => c.Datetime).ToList();
            var feedDate = comments.Any() ? comments.First().Datetime : bookmark.Date;
            var feedAutohor = comments.Any() ? comments.Last().UserID : bookmark.UserCreatorID;

            var feed = new Feed(bookmark.UserCreatorID, feedDate, true)
                {
                    Item = item,
                    ItemId = bookmark.ID.ToString(CultureInfo.InvariantCulture),
                    ItemUrl = CommonLinkUtility.ToAbsolute(itemUrl),
                    LastModifiedBy = feedAutohor,
                    Product = Product,
                    Module = Name,
                    Action = comments.Any() ? FeedAction.Commented : FeedAction.Created,
                    Title = bookmark.Name,
                    Description = bookmark.Description,
                    HasPreview = false,
                    CanComment = true,
                    CommentApiUrl = CommonLinkUtility.ToAbsolute(commentApiUrl),
                    Comments = comments.Select(ToFeedComment),
                    GroupId = string.Format("{0}_{1}", item, bookmark.ID)
                };
            feed.Keywords = string.Format("{0} {1} {2} {3}",
                                          bookmark.Name,
                                          bookmark.URL,
                                          bookmark.Description,
                                          string.Join(" ", feed.Comments.Select(x => x.Description)));

            return new Tuple<Feed, object>(feed, bookmark);
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