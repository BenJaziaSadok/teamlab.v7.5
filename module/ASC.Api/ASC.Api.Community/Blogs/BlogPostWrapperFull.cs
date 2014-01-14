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
    [DataContract(Name = "post", Namespace = "")]
    public class BlogPostWrapperFull:IApiSortableDate
    {
        public BlogPostWrapperFull(Post post)
        {
            CreatedBy = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(post.UserID));
            Updated = Created = (ApiDateTime) post.Datetime;
            Id = post.ID;
            Tags = post.TagList.Select(x=>x.Content).ToList();
            Title = post.Title;
            Text = post.Content;
        }

        private BlogPostWrapperFull()
        {
            
        }

        [DataMember(Order = 100)]
        public string Text { get; set; }

        [DataMember(Order = 5)]
        public string Title { get; set; }

        [DataMember(Order = 6)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 6)]
        public ApiDateTime Updated { get; set; }

        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 11)]
        protected List<string> Tags { get; set; }

        [DataMember(Order = 9)]
        public EmployeeWraper CreatedBy { get; set; }

        public static BlogPostWrapperFull GetSample()
        {
            return new BlogPostWrapperFull()
            {
                CreatedBy = EmployeeWraper.GetSample(),
                Created = (ApiDateTime)DateTime.UtcNow,
                Updated = (ApiDateTime)DateTime.Now,
                Id = Guid.NewGuid(),
                Text = "Post text",
                Tags = new List<string>() { "Tag1", "Tag2" },
                Title = "Example post",
                
            };
        }
    }
}