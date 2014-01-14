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
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Forum;
using ASC.Web.Core.Security;
using ASC.Web.Studio.Utility;
using System.Linq;

namespace ASC.Feed.Aggregator.Modules.Community
{
    internal class ForumsModule : FeedModule
    {
        protected override string Table
        {
            get { return "forum_post"; }
        }

        protected override string LastUpdatedColumn
        {
            get { return "create_date"; }
        }

        protected override string TenantColumn
        {
            get { return "tenantId"; }
        }

        protected override string DbId
        {
            get { return Constants.CommunityDbId; }
        }


        public override string Name
        {
            get { return Constants.ForumsModule; }
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
            var q1 = new SqlQuery("forum_topic")
                .Select("tenantId")
                .Where(Exp.Gt("create_date", fromTime))
                .GroupBy(1)
                .Having(Exp.Gt("count(*)", 0));

            var q2 = new SqlQuery("forum_post")
                .Select("tenantId")
                .Where(Exp.Gt("create_date", fromTime))
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
            var query = new SqlQuery("forum_topic t")
                .Select(TopicColumns().Select(t => "t." + t).ToArray())
                .LeftOuterJoin("forum_post p",
                               Exp.EqColumns("t.id", "p.topic_id") &
                               Exp.Eq("p.tenantId", filter.Tenant)
                )
                .Select(PostColumns().Select(p => "p." + p).ToArray())
                .Where("t.tenantid", filter.Tenant)
                .Where(Exp.Between("p.create_date", filter.Time.From, filter.Time.To));

            using (var db = new DbManager(DbId))
            {
                var posts = db.ExecuteList(query).ConvertAll(ToPost);
                var groupedTopics = posts.GroupBy(c => c.Topic.ID);

                return groupedTopics
                    .Select(b => new Tuple<Topic, IEnumerable<Post>>(b.First().Topic, b))
                    .Select(ToFeed);
            }
        }

        private static IEnumerable<string> TopicColumns()
        {
            return new[]
                {
                    "id", // 0
                    "title",
                    "type",
                    "recent_post_id",
                    "create_date",
                    "poster_id" // 5
                };
        }

        private static IEnumerable<string> PostColumns()
        {
            return new[]
                {
                    "id", // 6
                    "topic_id",
                    "poster_id",
                    "create_date",
                    "subject",
                    "text" // 11
                };
        }


        private static Post ToPost(object[] r)
        {
            var post = new Post
                {
                    Topic = new Topic
                        {
                            ID = Convert.ToInt32(r[0]),
                            Title = Convert.ToString(r[1]),
                            Type = (TopicType)Convert.ToInt32(r[2]),
                            RecentPostID = Convert.ToInt32(r[3]),
                            CreateDate = Convert.ToDateTime(r[4]),
                            PosterID = new Guid(Convert.ToString(r[5]))
                        }
                };

            if (r[6] != null)
            {
                post.ID = Convert.ToInt32(r[6]);
                post.TopicID = Convert.ToInt32(r[7]);
                post.PosterID = new Guid(Convert.ToString(r[8]));
                post.CreateDate = Convert.ToDateTime(r[9]);
                post.Subject = Convert.ToString(r[10]);
                post.Text = Convert.ToString(r[11]);
            }
            return post;
        }

        private Tuple<Feed, object> ToFeed(Tuple<Topic, IEnumerable<Post>> t)
        {
            var topic = t.Item1;

            var item = string.Empty;
            if (topic.Type == TopicType.Informational)
            {
                item = "forum";
            }
            else if (topic.Type == TopicType.Poll)
            {
                item = "forumPoll";
            }

            var itemUrl = "/products/community/modules/forum/posts.aspx?t=" + topic.ID + "&post=" + topic.RecentPostID;
            var commentApiUrl = "/api/2.0/community/forum/topic/" + topic.ID + ".json";

            var comments = t.Item2.Where(p => p.CreateDate != topic.CreateDate).OrderBy(p => p.CreateDate).ToList();
            var feedDate = comments.Any() ? comments.First().CreateDate : topic.CreateDate;
            var feedAutohor = comments.Any() ? comments.Last().PosterID : topic.PosterID;

            var feed = new Feed(topic.PosterID, feedDate, true)
                {
                    Item = item,
                    ItemId = topic.ID.ToString(CultureInfo.InvariantCulture),
                    ItemUrl = CommonLinkUtility.ToAbsolute(itemUrl),
                    LastModifiedBy = feedAutohor,
                    Product = Product,
                    Module = Name,
                    Action = comments.Any() ? FeedAction.Commented : FeedAction.Created,
                    Title = topic.Title,
                    HasPreview = false,
                    CanComment = true,
                    CommentApiUrl = CommonLinkUtility.ToAbsolute(commentApiUrl),
                    Comments = comments.Select(ToFeedComment),
                    GroupId = string.Format("{0}_{1}", item, topic.ID)
                };
            feed.Keywords = string.Format("{0} {1}",
                                          topic.Title,
                                          string.Join(" ", feed.Comments.Select(x => x.Description)));

            var firstPost = t.Item2.FirstOrDefault(p => p.CreateDate == topic.CreateDate);
            if (firstPost != null)
            {
                feed.Description = HtmlSanitizer.Sanitize(firstPost.Text);
            }

            return new Tuple<Feed, object>(feed, topic);
        }

        private static FeedComment ToFeedComment(Post comment)
        {
            return new FeedComment(comment.PosterID)
                {
                    Id = comment.ID.ToString(CultureInfo.InvariantCulture),
                    Description = HtmlSanitizer.Sanitize(comment.Text),
                    Date = comment.CreateDate
                };
        }
    }
}