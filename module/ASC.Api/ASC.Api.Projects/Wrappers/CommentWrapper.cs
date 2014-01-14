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

#region Usings

using System;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Specific;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "comment", Namespace = "")]
    public class CommentWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 12)]
        public Guid ParentId { get; set; }

        [DataMember(Order = 10)]
        public string Text { get; set; }

        [DataMember(Order = 50)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 9)]
        public EmployeeWraper CreatedBy { get; set; }

        [DataMember(Order = 10)]
        public string ProjectTitle { get; set; }

        [DataMember(Order = 50, EmitDefaultValue = false)]
        public ApiDateTime Updated { get; set; }

        [DataMember(Order = 13)]
        public bool Inactive { get; set; }

        private CommentWrapper()
        {
        }

        public CommentWrapper(Comment comment)
        {
            Id = comment.ID;
            ParentId = comment.Parent;
            Text = comment.Content;
            Created = Updated = (ApiDateTime)comment.CreateOn;
            CreatedBy = EmployeeWraper.Get(comment.CreateBy);
            Inactive = comment.Inactive;
        }


        public static CommentWrapper GetSample()
        {
            return new CommentWrapper
                       {
                           Id = Guid.NewGuid(),
                           ParentId = Guid.NewGuid(),
                           Text = "comment text",
                           Created = (ApiDateTime)DateTime.UtcNow,
                           CreatedBy = EmployeeWraper.GetSample(),
                           Updated = (ApiDateTime) DateTime.Now,
                           Inactive = false
                       };
        }
    }
}
