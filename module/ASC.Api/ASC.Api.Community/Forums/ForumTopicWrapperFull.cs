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
using ASC.Api.Employee;
using ASC.Forum;
using ASC.Specific;

namespace ASC.Api.Forums
{
    [DataContract(Name = "topic", Namespace = "")]
    public class ForumTopicWrapperFull:ForumTopicWrapper
    {
        [DataMember(Order = 100)]
        public List<ForumTopicPostWrapper> Posts { get; set; }


        public ForumTopicWrapperFull(Topic topic,IEnumerable<Post> posts) : base(topic)
        {
            if (topic.Type==TopicType.Poll)
            {
                //TODO: Deal with polls
            }
            Posts = posts.Where(x=>x.IsApproved).Select(x => new ForumTopicPostWrapper(x)).ToList();
        }

        private ForumTopicWrapperFull()
        {

        }

        public static new ForumTopicWrapperFull GetSample()
        {
            return new ForumTopicWrapperFull()
            {
                Created = (ApiDateTime)DateTime.Now,
                Updated = (ApiDateTime)DateTime.Now,
                Id = 10,
                UpdatedBy = EmployeeWraper.GetSample(),
                Text = "This is sample post",
                Status = TopicStatus.Sticky,
                Tags = new List<string>() { "Tag1", "Tag2" },
                Title = "Sample topic",
                Type = TopicType.Informational,
                Posts = new List<ForumTopicPostWrapper>() { ForumTopicPostWrapper.GetSample()}
            };
        }
    }
}