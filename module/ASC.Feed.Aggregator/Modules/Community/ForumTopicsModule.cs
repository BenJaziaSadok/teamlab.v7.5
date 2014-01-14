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
    internal class ForumTopicsModule : FeedModule
    {
        protected override string Table
        {
            get { return "forum_topic"; }
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

        public override IEnumerable<Tuple<Feed, object>> GetFeeds(FeedFilter filter)
        {
            var query = new SqlQuery("forum_post p")
                .Select(PostColumns().Select(p => "p." + p).ToArray())
                .InnerJoin("forum_topic t", Exp.EqColumns("p.topic_id", "t.id"))
                .Select(TopicColumns().Select(t => "t." + t).ToArray())
                .Where("t.tenantid", filter.Tenant)
                .Where(Exp.EqColumns("p.create_date", "t.create_date"))
                .Where(Exp.Between("p.create_date", filter.Time.From, filter.Time.To));

            using (var db = new DbManager(DbId))
            {
                var posts = db.ExecuteList(query).ConvertAll(ToPost);
                return posts.Select(c => new Tuple<Feed, object>(ToFeed(c), c));
            }
        }

        private static IEnumerable<string> PostColumns()
        {
            return new[]
                {
                    "id", // 0
                    "topic_id",
                    "poster_id",
                    "create_date",
                    "subject",
                    "text" // 5
                };
        }

        private static IEnumerable<string> TopicColumns()
        {
            return new[]
                {
                    "id", // 6
                    "title",
                    "type",
                    "recent_post_id",
                    "create_date",
                    "poster_id" // 11
                };
        }

        private static Post ToPost(object[] r)
        {
            return new Post
                {
                    ID = Convert.ToInt32(r[0]),
                    TopicID = Convert.ToInt32(r[1]),
                    PosterID = new Guid(Convert.ToString(r[2])),
                    CreateDate = Convert.ToDateTime(r[3]),
                    Subject = Convert.ToString(r[4]),
                    Text = Convert.ToString(r[5]),
                    Topic = new Topic
                        {
                            ID = Convert.ToInt32(r[6]),
                            Title = Convert.ToString(r[7]),
                            Type = (TopicType)Convert.ToInt32(r[8]),
                            RecentPostID = Convert.ToInt32(r[9]),
                            CreateDate = Convert.ToDateTime(r[10]),
                            PosterID = new Guid(Convert.ToString(r[11]))
                        }
                };
        }

        private Feed ToFeed(Post post)
        {
            var item = string.Empty;
            if (post.Topic.Type == TopicType.Informational)
            {
                item = "forumTopic";
            }
            else if (post.Topic.Type == TopicType.Poll)
            {
                item = "forumPoll";
            }

            var itemUrl = "/products/community/modules/forum/posts.aspx?t=" + post.Topic.ID + "&post=" + post.ID;
            return new Feed(post.Topic.PosterID, post.Topic.CreateDate)
                {
                    Item = item,
                    ItemId = post.Topic.ID.ToString(CultureInfo.InvariantCulture),
                    ItemUrl = CommonLinkUtility.ToAbsolute(itemUrl),
                    Product = Product,
                    Module = Name,
                    Title = post.Topic.Title,
                    Description = HtmlSanitizer.Sanitize(post.Text),
                    Keywords = string.Format("{0} {1}", post.Topic.Title, post.Text),
                    HasPreview = false,
                    CanComment = false,
                    GroupId = string.Format("{0}_{1}", item, post.Topic.ID)
                };
        }
    }
}