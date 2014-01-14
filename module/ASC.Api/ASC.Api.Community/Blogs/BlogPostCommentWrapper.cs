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
using ASC.Blogs.Core.Domain;
using ASC.Specific;

namespace ASC.Api.Blogs
{
    [DataContract(Name = "comment", Namespace = "")]
    public class BlogPostCommentWrapper : IApiSortableDate
    {
        public BlogPostCommentWrapper(Comment comment)
        {
            CreatedBy = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(comment.UserID));
            Updated = Created = (ApiDateTime)comment.Datetime;
            Id = comment.ID;
            Text = comment.Content;
            ParentId = comment.ParentId;
           
        }


        private BlogPostCommentWrapper()
        {

        }

        [DataMember(Order = 10)]
        public string Text { get; set; }

        [DataMember(Order = 6)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 6)]
        public ApiDateTime Updated { get; set; }


        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 9)]
        public EmployeeWraper CreatedBy { get; set; }

        [DataMember(Order = 16)]
        protected Guid ParentId { get; set; }

        public static BlogPostCommentWrapper GetSample()
        {
            return new BlogPostCommentWrapper()
            {
                CreatedBy = EmployeeWraper.GetSample(),
                Created = (ApiDateTime)DateTime.UtcNow,
                Id = Guid.NewGuid(),
                ParentId = Guid.NewGuid(),
                Text = "comment text",
                Updated = (ApiDateTime)DateTime.Now
            };
        }
    }
}