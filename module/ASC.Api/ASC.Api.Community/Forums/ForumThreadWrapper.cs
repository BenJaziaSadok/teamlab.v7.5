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
using ASC.Forum;
using ASC.Specific;

namespace ASC.Api.Forums
{
    [DataContract(Name = "thread", Namespace = "")]
    public class ForumThreadWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Title { get; set; }

        [DataMember(Order = 10)]
        public string Description { get; set; }

        [DataMember(Order = 20)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 21)]
        public ApiDateTime Updated { get; set; }

        [DataMember(Order = 30)]
        public int RecentTopicId { get; set; }

        [DataMember(Order = 30)]
        public string RecentTopicTitle { get; set; }

        [DataMember(Order = 9)]
        public EmployeeWraper UpdatedBy { get; set; }

        public ForumThreadWrapper(Thread thread)
        {
            Id = thread.ID;
            Title = thread.Title;
            Description = thread.Description;
            Created = (ApiDateTime)thread.RecentTopicCreateDate;
            Updated = (ApiDateTime)thread.RecentPostCreateDate;
            RecentTopicTitle = thread.RecentTopicTitle;
            UpdatedBy = EmployeeWraper.Get(thread.RecentPosterID);
        }

        protected ForumThreadWrapper()
        {
        }

        public static ForumThreadWrapper GetSample()
        {
            return new ForumThreadWrapper()
                       {
                           Created = (ApiDateTime)DateTime.Now,
                           Updated = (ApiDateTime)DateTime.Now,
                           Description = "Sample thread",
                           Id = 10,
                           UpdatedBy = EmployeeWraper.GetSample(),
                           RecentTopicId = 1234,
                           RecentTopicTitle = "Sample topic",
                           Title = "The Thread"
                       };
        }
    }
}