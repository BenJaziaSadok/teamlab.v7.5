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
using ASC.Core;
using ASC.Web.Community.News.Code.DAO;
using ASC.Web.Studio.UserControls.Common.PollForm;

namespace ASC.Web.Community.News.Code
{
	public class PollVoteHandler : IVoteHandler
	{
		#region IVoteHandler Members

		public bool VoteCallback(string pollID, List<string> selectedVariantIDs, string additionalParams, out string errorMessage)
		{
			errorMessage = string.Empty;
			var userAnswersIDs = new List<long>();
			selectedVariantIDs.ForEach(strId => { if (!string.IsNullOrEmpty(strId)) userAnswersIDs.Add(Convert.ToInt64(strId)); });
			long pollId = Convert.ToInt64(additionalParams);
			var storage = FeedStorageFactory.Create();

			return VoteForPoll(userAnswersIDs, storage, pollId, out errorMessage);
		}

	    public static bool VoteForPoll(List<long> userAnswersIDs, IFeedStorage storage, long pollId, out string errorMessage)
	    {
	        errorMessage = string.Empty;
	        storage.PollVote(SecurityContext.CurrentAccount.ID.ToString(), userAnswersIDs);
	        var pollFeed = storage.GetFeed(pollId);
	        if (pollFeed == null)
	        {
	            errorMessage = Resources.NewsResource.ErrorAccessDenied;
	            return false;
	        }
	        return true;
	    }

	    #endregion
	}
}
