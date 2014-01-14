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
using ASC.Core.Tenants;

#endregion

namespace ASC.Web.Community.News.Code.DAO
{
	static class Mappers
	{
		public static Feed ToFeed(object[] row)
		{
		    return new Feed
		               {
		                   Id = Convert.ToInt64(row[0]),
		                   FeedType = (FeedType) Convert.ToInt32(row[1]),
                           Caption = Convert.ToString(row[2]),
		                   Date = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[3])),
		                   Creator = Convert.ToString(row[4]),
		                   Readed = Convert.ToBoolean(row[5])
		               };
		}

        public static Feed ToFeed2(object[] row)
        {
            return new Feed
            {
                Id = Convert.ToInt64(row[0]),
                FeedType = (FeedType)Convert.ToInt32(row[1]),
                Caption = Convert.ToString(row[2]),
                Text = Convert.ToString(row[3]),
                Date = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[4])),
                Creator = Convert.ToString(row[5]),
                Readed = Convert.ToBoolean(row[6])
            };
        }

		public static Feed ToFeedFinded(object[] row)
		{
		    return new Feed
		               {
		                   Id = Convert.ToInt64(row[0]),
		                   Caption = Convert.ToString(row[1]),
		                   Text = Convert.ToString(row[2]),
		                   Date = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[3]))
		               };
		}

		public static Feed ToNewsOrPoll(object[] row)
		{
			var feedType = (FeedType)Convert.ToInt32(row[1]);
			var feed = feedType == FeedType.Poll ? new FeedPoll() : (Feed)new FeedNews();

			feed.Id = Convert.ToInt64(row[0]);
			feed.FeedType = feedType;
			feed.Caption = Convert.ToString(row[2]);
			feed.Text = Convert.ToString(row[3]);
			feed.Date = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[4]));
			feed.Creator = Convert.ToString(row[5]);
			feed.Readed = Convert.ToBoolean(row[6]);
			
            return feed;
		}

		public static FeedComment ToFeedComment(object[] row)
		{
		    return new FeedComment(Convert.ToInt64(row[1]))
		               {
		                   Id = Convert.ToInt64(row[0]),
		                   Comment = Convert.ToString(row[2]),
		                   ParentId = Convert.ToInt64(row[3]),
		                   Date = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[4])),
		                   Creator = Convert.ToString(row[5]),
		                   Inactive = Convert.ToBoolean(row[6]),
		               };
		}

        public static FeedComment ToFeedComment(object[] row, bool withFeed)
        {
            if (!withFeed) return ToFeedComment(row);

            var feed = new Feed
                           {
                               Id = Convert.ToInt64(row[6]),
                               FeedType = (FeedType) Convert.ToInt32(row[7]),
                               Caption = Convert.ToString(row[8]),
                               Date = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[9])),
                               Creator = Convert.ToString(row[10]),
                               Readed = Convert.ToBoolean(row[11])
                           };

            return new FeedComment(Convert.ToInt64(row[6]), feed)
            {
                Id = Convert.ToInt64(row[0]),
                Comment = Convert.ToString(row[1]),
                ParentId = Convert.ToInt64(row[2]),
                Date = TenantUtil.DateTimeFromUtc(Convert.ToDateTime(row[3])),
                Creator = Convert.ToString(row[4]),
                Inactive = Convert.ToBoolean(row[5]),
            };
        }
	}
}
