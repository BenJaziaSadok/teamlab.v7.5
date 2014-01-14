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

using System.Collections.Generic;
using ASC.Feed.Data;
using ASC.Specific;

namespace ASC.Api.Feed
{
    public struct FeedWrapper
    {
        public FeedWrapper(FeedResultItem item)
            : this()
        {
            Feed = item.Json;
            Module = item.Module;
            GroupId = item.GroupId;
            IsToday = item.IsToday;
            IsYesterday = item.IsYesterday;
            CreateOn = (ApiDateTime)item.CreateOn;
            AggregatedDate = (ApiDateTime)item.AggregatedDate;
        }

        public string Feed { get; private set; }

        public string Module { get; private set; }

        public bool IsToday { get; private set; }

        public bool IsYesterday { get; private set; }

        public ApiDateTime CreateOn { get; private set; }

        public ApiDateTime AggregatedDate { get; private set; }

        public ApiDateTime TimeReaded { get; private set; }

        public string GroupId { get; set; }

        public IEnumerable<FeedWrapper> GroupedFeeds { get; set; }
    }
}