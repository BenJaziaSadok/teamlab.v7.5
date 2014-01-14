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
    public class ForumTopicWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Title { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Created { get; set; }

        private ApiDateTime _updated;

        [DataMember(Order = 3)]
        public ApiDateTime Updated
        {
            get { return _updated >= Created ? _updated : Created; }
            set { _updated = value; }
        }

        [DataMember(Order = 8)]
        public string Text { get; set; }

        [DataMember(Order = 9)]
        public EmployeeWraper UpdatedBy { get; set; }

        [DataMember(Order = 10)]
        public string ThreadTitile { get; set; }
               

        public ForumTopicWrapper(Topic topic)
        {
            Id = topic.ID;
            Title = topic.Title;
            Created = (ApiDateTime)topic.CreateDate;
            Text = ASC.Common.Utils.HtmlUtil.GetText(topic.RecentPostText, 160);
            Updated = (ApiDateTime)topic.RecentPostCreateDate;
            UpdatedBy = EmployeeWraper.Get(topic.RecentPostAuthorID);
            Status = topic.Status;
            Type = topic.Type;
            Tags = topic.Tags.Where(x => x.IsApproved).Select(x => x.Name).ToList();
            ThreadTitile = topic.ThreadTitle;
        }

        protected ForumTopicWrapper()
        {

        }

        [DataMember(Order = 30)]
        public TopicStatus Status { get; set; }
        [DataMember(Order = 30)]
        public TopicType Type { get; set; }

        [DataMember(Order = 100)]
        public List<string> Tags { get; set; }

        public static ForumTopicWrapper GetSample()
        {
            return new ForumTopicWrapper()
                       {
                           Created = (ApiDateTime)DateTime.Now,
                           Updated = (ApiDateTime)DateTime.Now,
                           Id = 10,
                           UpdatedBy = EmployeeWraper.GetSample(),
                           Text = "This is sample post",
                           Status = TopicStatus.Sticky,
                           Tags = new List<string>() { "Tag1", "Tag2" },
                           Title = "Sample topic",
                           Type = TopicType.Informational
                       };
        }
    }
}