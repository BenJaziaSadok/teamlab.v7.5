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
using System.Collections.Generic;
using ASC.Core.Tenants;

#endregion

namespace ASC.Web.Community.News.Code
{
	[Serializable]
	public class FeedPoll : Feed
	{
		public FeedPollType PollType { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public List<FeedPollVariant> Variants { get; private set;}

		internal List<FeedPollAnswer> Answers;


		public FeedPoll()
		{
			FeedType = FeedType.Poll;
			PollType = FeedPollType.SimpleAnswer;
            StartDate = TenantUtil.DateTimeNow();
			EndDate = StartDate.AddYears(100);
			Variants = new List<FeedPollVariant>();
			Answers = new List<FeedPollAnswer>();
		}

		public int GetVariantVoteCount(long variantId)
		{
			return Answers.FindAll(a => a.VariantId == variantId).Count;
		}

		public bool IsUserVote(string userId)
		{
			return Answers.Exists(a => a.UserId == userId);
		}
	}
}
