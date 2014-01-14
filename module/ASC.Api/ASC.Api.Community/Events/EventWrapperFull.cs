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
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Specific;
using ASC.Web.Community.News.Code;

namespace ASC.Api.Events
{
    [DataContract(Name = "event", Namespace = "")]
    public class EventWrapperFull : EventWrapper
    {
        [DataMember(Order = 100)]
        public string Text { get; set; }



        [DataMember(Order = 200, EmitDefaultValue = false)]
        public PollWrapper Poll { get; set; }

        public EventWrapperFull(ASC.Web.Community.News.Code.Feed feed)
            : base(feed)
        {
            if (feed is FeedPoll)
            {
                //Add poll info
                var poll = feed as FeedPoll;
                Poll = new PollWrapper(poll);
            }
            Text = feed.Text;
        }

        private EventWrapperFull()
        {

        }

        public static new EventWrapperFull GetSample()
        {
            return new EventWrapperFull()
            {
                CreatedBy = EmployeeWraper.GetSample(),
                Created = (ApiDateTime)DateTime.UtcNow,
                Id = 10,
                Type = FeedType.News,
                Title = "Sample news",
                Updated = (ApiDateTime)DateTime.Now,
                Text = "Text of feed",
                Poll = PollWrapper.GetSample()
            };
        }
    }
}