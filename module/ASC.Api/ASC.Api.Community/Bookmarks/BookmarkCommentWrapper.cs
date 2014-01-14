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
using ASC.Bookmarking.Pojo;
using ASC.Specific;

namespace ASC.Api.Bookmarks
{
    [DataContract(Name = "comment", Namespace = "")]
    public class BookmarkCommentWrapper : IApiSortableDate
    {
        public BookmarkCommentWrapper(Comment comment)
        {
            CreatedBy = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(comment.UserID));
            Updated=Created = (ApiDateTime) comment.Datetime;
            Id = comment.ID;
            Text = comment.Content;
            if (!string.IsNullOrEmpty(comment.Parent))
                ParentId = new Guid(comment.Parent);
        }

        private BookmarkCommentWrapper()
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

        [DataMember(Order = 2,EmitDefaultValue = false)]
        public Guid ParentId { get; set; }

        [DataMember(Order = 9)]
        public EmployeeWraper CreatedBy { get; set; }

        public static BookmarkCommentWrapper GetSample()
        {
            return new BookmarkCommentWrapper()
            {
                CreatedBy = EmployeeWraper.GetSample(),
                Created = (ApiDateTime)DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Text = "comment text",
                ParentId = Guid.NewGuid(),
                Updated = (ApiDateTime)DateTime.Now
            };
        }
    }
}