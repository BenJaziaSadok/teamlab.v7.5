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
    [DataContract(Name = "thread", Namespace = "")]
    public class ForumThreadWrapperFull : ForumThreadWrapper
    {
       
        [DataMember(Order = 100)]
        public List<ForumTopicWrapper> Topics { get; set; }

        public ForumThreadWrapperFull(Thread thread, IEnumerable<Topic> topics):base(thread)
        {
            Topics = topics.Where(x=>x.IsApproved).Select(x => new ForumTopicWrapper(x)).ToList();
        }

        protected ForumThreadWrapperFull()
        {
        }

        public static new ForumThreadWrapperFull GetSample()
        {
            return new ForumThreadWrapperFull()
            {
                Created = (ApiDateTime)DateTime.Now,
                Updated = (ApiDateTime)DateTime.Now,
                Description = "Sample thread",
                Id = 10,
                UpdatedBy = EmployeeWraper.GetSample(),
                RecentTopicId = 1234,
                RecentTopicTitle = "Sample topic",
                Title = "The Thread",
                Topics = new List<ForumTopicWrapper>(){ForumTopicWrapper.GetSample()}
            };
        }
    }
}