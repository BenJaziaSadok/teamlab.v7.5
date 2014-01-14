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
    [DataContract(Name = "post", Namespace = "")]
    public class ForumTopicPostWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Subject { get; set; }

        [DataMember(Order = 2)]
        public string Text { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Created { get; set; }

        private ApiDateTime updated;

        [DataMember(Order = 3)]
        public ApiDateTime Updated
        {
            get { return updated >= Created ? updated : Created; }
            set { updated = value; }
        }

        [DataMember(Order = 9)]
        public EmployeeWraper CreatedBy { get; set; }

        [DataMember(Order = 10)]
        public string ThreadTitle { get; set; }

        [DataMember(Order = 100, EmitDefaultValue = false)]
        public List<ForumTopicPostAttachmentWrapper> Attachments { get; set; }

        public ForumTopicPostWrapper(Post post)
        {
            Id = post.ID;
            Text = post.Text;
            Created = (ApiDateTime)post.CreateDate;
            Updated = (ApiDateTime)post.EditDate;
            Subject = post.Subject;
            CreatedBy = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(post.PosterID));
            Attachments = post.Attachments.Select(x => new ForumTopicPostAttachmentWrapper(x)).ToList();
        }

        private ForumTopicPostWrapper()
        {
        }

        public static ForumTopicPostWrapper GetSample()
        {
            return new ForumTopicPostWrapper
                {
                    Id = 123,
                    CreatedBy = EmployeeWraper.GetSample(),
                    Created = (ApiDateTime)DateTime.Now,
                    Updated = (ApiDateTime)DateTime.Now,
                    Subject = "Sample subject",
                    Text = "Post text",
                    Attachments =
                        new List<ForumTopicPostAttachmentWrapper>
                            {
                                ForumTopicPostAttachmentWrapper.GetSample()
                            }
                };
        }
    }
}