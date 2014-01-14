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
using System.Linq;
using System.Runtime.Serialization;
using ASC.Core;
using ASC.Specific;
using ASC.Web.Community.News.Code;

namespace ASC.Api.Events
{
    [DataContract(Name = "poll", Namespace = "")]
    public class PollWrapper
    {
        public PollWrapper(FeedPoll poll)
        {
            Voted = poll.IsUserVote(SecurityContext.CurrentAccount.ID.ToString());
            if (Voted)
            {
                //Get results
            }
            StartDate = (ApiDateTime)poll.StartDate;
            EndDate = (ApiDateTime)poll.EndDate;
            PollType = poll.PollType;
            Votes = poll.Variants.Select(x=>new VoteWrapper(){Id = x.ID, Name = x.Name, Votes = poll.GetVariantVoteCount(x.ID)});
        }

        internal PollWrapper()
        {

        }

        [DataMember(Order = 200, EmitDefaultValue = true)]
        public FeedPollType PollType { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public ApiDateTime EndDate { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = false)]
        public ApiDateTime StartDate { get; set; }

        [DataMember(Order = 200, EmitDefaultValue = true)]
        public bool Voted { get; set; }

        [DataMember(Order = 300)]
        public IEnumerable<VoteWrapper> Votes { get; set; }

        public static PollWrapper GetSample()
        {
            return new PollWrapper()
                       {
                           EndDate = (ApiDateTime) DateTime.Now,
                           PollType = FeedPollType.SimpleAnswer,
                           StartDate = (ApiDateTime) DateTime.Now,
                           Voted = false,
                           Votes = new List<VoteWrapper>(){VoteWrapper.GetSample()}

                       };
        }
    }
}