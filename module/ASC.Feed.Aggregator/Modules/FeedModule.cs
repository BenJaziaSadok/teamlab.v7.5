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
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Web.Core;

namespace ASC.Feed.Aggregator.Modules
{
    internal abstract class FeedModule : IFeedModule
    {
        public abstract string Name { get; }
        public abstract string Product { get; }
        public abstract Guid ProductID { get; }

        protected abstract string Table { get; }
        protected abstract string LastUpdatedColumn { get; }
        protected abstract string TenantColumn { get; }
        protected abstract string DbId { get; }

        protected int Tenant
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TenantId; }
        }

        protected string GetGroupId(string item, Guid author, string rootId, int action = 0)
        {
            const int interval = 2;

            var now = DateTime.UtcNow;
            var hours = now.Hour;
            var groupIdHours = hours - (hours % interval);

            // groupId = {item}_{author}_{date}_{rootId}_{action}
            return string.Format("{0}_{1}_{2}_{3}_{4}",
                                 item,
                                 author,
                                 now.ToString("yyyy.MM.dd.") + groupIdHours,
                                 rootId,
                                 action);
        }


        public virtual IEnumerable<int> GetTenantsWithFeeds(DateTime fromTime)
        {
            var q = new SqlQuery(Table)
                .Select(TenantColumn)
                .Where(Exp.Gt(LastUpdatedColumn, fromTime))
                .GroupBy(1)
                .Having(Exp.Gt("count(*)", 0));

            using (var db = new DbManager(DbId))
            {
                return db
                    .ExecuteList(q)
                    .ConvertAll(r => Convert.ToInt32(r[0]));
            }
        }

        public abstract IEnumerable<Tuple<Feed, object>> GetFeeds(FeedFilter filter);

        public virtual bool VisibleFor(Feed feed, object data, Guid userId)
        {
            return WebItemSecurity.IsAvailableForUser(ProductID.ToString(), userId);
        }
    }
}