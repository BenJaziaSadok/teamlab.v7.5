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
using ASC.Core.Tenants;
using Newtonsoft.Json;

namespace ASC.Feed.Data
{
    public class FeedAggregateDataProvider
    {
        public static DateTime GetLastTimeAggregate(string key)
        {
            var q = new SqlQuery("feed_last")
                .Select("last_date")
                .Where("last_key", key);

            using (var db = new DbManager(Constants.FeedDbId))
            {
                var value = db.ExecuteScalar<DateTime>(q);
                return value != default(DateTime) ? value.AddSeconds(1) : value;
            }
        }

        public static void SaveFeeds(IEnumerable<FeedRow> feeds, string key, DateTime value)
        {
            using (var db = new DbManager(Constants.FeedDbId))
            using (var tx = db.BeginTransaction())
            {
                db.ExecuteNonQuery(new SqlInsert("feed_last", true).InColumnValue("last_key", key).InColumnValue("last_date", value));

                var now = DateTime.UtcNow;
                foreach (var f in feeds)
                {
                    if (0 >= f.Users.Count) continue;

                    var i = new SqlInsert("feed_aggregate", true)
                        .InColumnValue("id", f.Id)
                        .InColumnValue("tenant", f.Tenant)
                        .InColumnValue("product", f.ProductId)
                        .InColumnValue("module", f.ModuleId)
                        .InColumnValue("author", f.AuthorId)
                        .InColumnValue("group_id", f.GroupId)
                        .InColumnValue("created_date", f.CreatedDate)
                        .InColumnValue("json", f.Json)
                        .InColumnValue("keywords", f.Keywords)
                        .InColumnValue("aggregated_date", now);

                    db.ExecuteNonQuery(i);

                    if (f.ClearRightsBeforeInsert)
                    {
                        db.ExecuteNonQuery(
                            new SqlDelete("feed_users")
                                .Where("feed_id", f.Id)
                            );
                    }

                    foreach (var u in f.Users)
                    {
                        db.ExecuteNonQuery(
                            new SqlInsert("feed_users", true)
                                .InColumnValue("feed_id", f.Id)
                                .InColumnValue("user_id", u.ToString())
                            );
                    }
                }

                tx.Commit();
            }
        }

        public static void RemoveFeedAggregate(DateTime fromTime)
        {
            using (var db = new DbManager(Constants.FeedDbId))
            using (var command = db.Connection.CreateCommand())
            using (var tx = db.Connection.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                command.Transaction = tx;
                command.CommandTimeout = 60 * 60; // a hour
                var dialect = DbRegistry.GetSqlDialect(Constants.FeedDbId);
                if (dialect.SupportMultiTableUpdate)
                {
                    command.CommandText = "delete from feed_aggregate, feed_users using feed_aggregate, feed_users where id = feed_id and aggregated_date < @date";
                    command
                        .AddParameter("date", fromTime)
                        .ExecuteNonQuery();
                }
                else
                {
                    command.ExecuteNonQuery(new SqlDelete("feed_users").Where(Exp.In("feed_id", new SqlQuery("feed_aggregate").Select("id").Where(Exp.Lt("aggregated_date", fromTime)))), dialect);
                    command.ExecuteNonQuery(new SqlDelete("feed_aggregate").Where(Exp.Lt("aggregated_date", fromTime)), dialect);
                }
                tx.Commit();
            }
        }

        public static List<FeedResultItem> GetFeeds(FeedApiFilter filter)
        {
            var filterOffset = filter.Offset;
            var filterLimit = filter.Max > 0 && filter.Max < 1000 ? filter.Max : 1000;

            var feeds = new Dictionary<string, List<FeedResultItem>>();

            var tryCount = 0;
            List<FeedResultItem> feedsIteration;
            do
            {
                feedsIteration = GetFeedsInternal(filter);
                foreach (var feed in feedsIteration)
                {
                    if (feeds.ContainsKey(feed.GroupId))
                    {
                        feeds[feed.GroupId].Add(feed);
                    }
                    else
                    {
                        feeds[feed.GroupId] = new List<FeedResultItem> {feed};
                    }
                }
                filter.Offset += feedsIteration.Count;
            } while (feeds.Count < filterLimit
                     && feedsIteration.Count == filterLimit
                     && tryCount++ < 5);

            filter.Offset = filterOffset;
            return feeds.Take(filterLimit).SelectMany(group => group.Value).ToList();
        }

        private static List<FeedResultItem> GetFeedsInternal(FeedApiFilter filter)
        {
            var from = TenantUtil.DateTimeFromUtc(filter.From);
            from = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0);
            
            var to = TenantUtil.DateTimeFromUtc(filter.To);
            to = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);
            
            var query = new SqlQuery("feed_aggregate a")
                .InnerJoin("feed_users u", Exp.EqColumns("a.id", "u.feed_id"))
                .Select("a.json, a.module, a.author, a.group_id, a.created_date, a.aggregated_date")
                .Where("a.tenant", CoreContext.TenantManager.GetCurrentTenant().TenantId)
                .Where(
                    !Exp.Eq("a.author", SecurityContext.CurrentAccount.ID) &
                    Exp.Eq("u.user_id", SecurityContext.CurrentAccount.ID)
                )
                .OrderBy("a.created_date", false)
                .SetFirstResult(filter.Offset)
                .SetMaxResults(filter.Max);

            if (filter.OnlyNew)
            {
                query.Where(Exp.Ge("a.aggregated_date", filter.From));
            }
            else
            {
                if (1 < from.Year)
                {
                    query.Where(Exp.Ge("a.created_date", from));
                }
                if (to.Year < 9999)
                {
                    query.Where(Exp.Le("a.created_date", to));
                }
            }

            if (!string.IsNullOrEmpty(filter.Product))
            {
                query.Where("a.product", filter.Product);
            }
            if (filter.Author != Guid.Empty)
            {
                query.Where("a.author", filter.Author);
            }
            if (filter.SearchKeys != null && filter.SearchKeys.Length > 0)
            {
                var exp = filter.SearchKeys
                                .Where(s => !string.IsNullOrEmpty(s))
                                .Select(s => s.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_"))
                                .Aggregate(Exp.False, (cur, s) => cur | Exp.Like("a.keywords", s, SqlLike.AnyWhere));
                query.Where(exp);
            }

            using (var db = new DbManager(Constants.FeedDbId))
            {
                var news = db
                    .ExecuteList(query)
                    .ConvertAll(r => new FeedResultItem(
                                         Convert.ToString(r[0]),
                                         Convert.ToString(r[1]),
                                         new Guid(Convert.ToString(r[2])),
                                         Convert.ToString(r[3]),
                                         TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[4])),
                                         TenantUtil.DateTimeFromUtc(Convert.ToDateTime(r[5]))));
                return news;
            }
        }

        public static int GetNewFeedsCount(DateTime lastReadedTime)
        {
            var q = new SqlQuery("feed_aggregate a")
                .InnerJoin("feed_users u", Exp.EqColumns("a.id", "u.feed_id"))
                .SelectCount()
                .Where("a.tenant", CoreContext.TenantManager.GetCurrentTenant().TenantId)
                .Where("u.user_id", SecurityContext.CurrentAccount.ID)
                .Where(!Exp.Eq("a.author", SecurityContext.CurrentAccount.ID));
            if (1 < lastReadedTime.Year)
            {
                q.Where(Exp.Ge("a.aggregated_date", lastReadedTime));
            }

            using (var db = new DbManager(Constants.FeedDbId))
            {
                return db.ExecuteScalar<int>(q);
            }
        }

        public IEnumerable<int> GetTenants(TimeInterval interval)
        {
            using (var db = new DbManager(Constants.FeedDbId))
            {
                var q = new SqlQuery("feed_aggregate")
                    .Select("tenant")
                    .Where(Exp.Between("aggregated_date", interval.From, interval.To))
                    .GroupBy(1);
                return db.ExecuteList(q).ConvertAll(r => Convert.ToInt32(r[0]));
            }
        }
    }


    public class FeedResultItem
    {
        public FeedResultItem(string json, string module, Guid lastModifiedBy, string groupId, DateTime createOn, DateTime aggregatedDate)
        {
            var now = TenantUtil.DateTimeFromUtc(DateTime.UtcNow);

            Json = json;
            Module = module;
            LastModifiedBy = lastModifiedBy;
            GroupId = groupId;

            if (now.Year == createOn.Year && now.Date == createOn.Date)
            {
                IsToday = true;
            }
            else if (now.Year == createOn.Year && now.Date == createOn.Date.AddDays(1))
            {
                IsYesterday = true;
            }

            CreateOn = createOn;
            AggregatedDate = aggregatedDate;
        }

        public string Json { get; private set; }

        public string Module { get; private set; }

        public Guid LastModifiedBy { get; private set; }

        public string GroupId { get; private set; }

        public bool IsToday { get; private set; }

        public bool IsYesterday { get; private set; }

        public DateTime CreateOn { get; private set; }

        public DateTime AggregatedDate { get; private set; }

        public FeedMin ToFeedMin()
        {
            return JsonConvert.DeserializeObject<FeedMin>(Json);
        }
    }
}