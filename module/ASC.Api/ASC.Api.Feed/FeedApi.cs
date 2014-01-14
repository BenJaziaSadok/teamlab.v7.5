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
using ASC.Api.Attributes;
using ASC.Api.Impl;
using ASC.Api.Interfaces;
using ASC.Core;
using ASC.Feed;
using ASC.Feed.Data;
using ASC.Specific;
using System.Linq;

namespace ASC.Api.Feed
{
    public class FeedApi : IApiEntryPoint
    {
        private readonly ApiContext context;

        public string Name
        {
            get { return "feed"; }
        }

        public FeedApi(ApiContext context)
        {
            this.context = context;
        }

        [Update("/read")]
        public void Read()
        {
            new FeedReadedDataProvider().SetTimeReaded(SecurityContext.CurrentAccount.ID);
        }

        [Read("/filter")]
        public object GetFeed(
            string product,
            ApiDateTime from,
            ApiDateTime to,
            Guid? author,
            bool? onlyNew,
            ApiDateTime timeReaded)
        {
            var filter = new FeedApiFilter
                {
                    Product = product,
                    From = from != null ? from.UtcTime : DateTime.MinValue,
                    To = to != null ? to.UtcTime : DateTime.MaxValue,
                    Offset = (int)context.StartIndex,
                    Max = (int)context.Count - 1,
                    Author = author ?? Guid.Empty,
                    SearchKeys = context.FilterValues,
                    OnlyNew = onlyNew.HasValue && onlyNew.Value
                };

            var feedReadedProvider = new FeedReadedDataProvider();

            var lastTimeReaded = feedReadedProvider.GetTimeReaded();

            var readedDate = timeReaded != null ? (ApiDateTime)timeReaded.UtcTime : (ApiDateTime)lastTimeReaded;
            var commentReadedDate = (ApiDateTime)feedReadedProvider.GetTimeReaded("comments");

            if (filter.OnlyNew)
            {
                filter.From = (ApiDateTime)lastTimeReaded;
                filter.Max = 100;
            }
            else if (timeReaded == null)
            {
                feedReadedProvider.SetTimeReaded();
                feedReadedProvider.SetTimeReaded("comments");
            }

            var feeds = FeedAggregateDataProvider
                .GetFeeds(filter)
                .GroupBy(n => n.GroupId,
                         n => new FeedWrapper(n),
                         (n, group) =>
                             {
                                 var firstFeed = group.First();
                                 firstFeed.GroupedFeeds = group.Skip(1);
                                 return firstFeed;
                             })
                .ToList();

            context.SetDataPaginated();
            return new {feeds, readedDate, commentReadedDate};
        }

        [Read("/newfeedscount")]
        public object GetFreshNewsCount()
        {
            var feedReadedProvider = new FeedReadedDataProvider();
            var lastTimeReaded = feedReadedProvider.GetTimeReaded();

            return FeedAggregateDataProvider.GetNewFeedsCount(lastTimeReaded);
        }
    }
}