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

namespace ASC.Web.Community.News.Code.DAO
{
	public interface IFeedStorage : IDisposable
	{
		List<FeedType> GetUsedFeedTypes();

		
		List<Feed> GetFeeds(FeedType feedType, Guid userId, int count, int offset);

		List<Feed> SearchFeeds(string s, FeedType feedType, Guid userId, int count, int offset);

		long GetFeedsCount(FeedType feedType, Guid userId);

		long SearchFeedsCount(string s, FeedType feedType, Guid userId);

		List<Feed> SearchFeeds(string s);

		Feed GetFeed(long id);

	    List<Feed> GetFeedByDate(DateTime from, DateTime to, Guid userId);

	    List<FeedComment> GetCommentsByDate(DateTime from, DateTime to);

		Feed SaveFeed(Feed feed, bool isEdit, FeedType poll);
		
		void RemoveFeed(long id);

		void ReadFeed(long feedId, string reader);

		void PollVote(string userId, ICollection<long> variantIds);

		
		List<FeedComment> GetFeedComments(long feedId);
		
		FeedComment GetFeedComment(long commentId);
		
		void RemoveFeedComment(long commentId);
        FeedComment SaveFeedComment(Feed feed, FeedComment comment);
        void RemoveFeedComment(FeedComment comment);
        void UpdateFeedComment(FeedComment comment);
    }
}