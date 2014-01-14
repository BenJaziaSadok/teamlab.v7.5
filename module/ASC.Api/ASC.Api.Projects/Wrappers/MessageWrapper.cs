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
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Specific;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "message", Namespace = "")]
    public class MessageWrapper : IApiSortableDate
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 14)]
        public SimpleProjectWrapper ProjectOwner { get; set; }

        [DataMember(Order = 9)]
        public string Title { get; set; }

        [DataMember(Order = 10)]
        public string Text { get; set; }

        [DataMember(Order = 50)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 51)]
        public EmployeeWraper CreatedBy { get; set; }

        private ApiDateTime updated;

        [DataMember(Order = 50)]
        public ApiDateTime Updated
        {
            get { return updated >= Created ? updated : Created; }
            set { updated = value; }
        }

        [DataMember(Order = 41)]
        public EmployeeWraper UpdatedBy { get; set; }

        [DataMember]
        public bool CanEdit { get; set; }

        [DataMember(Order = 15)]
        public int CommentsCount { get; set; }


        private MessageWrapper()
        {
        }

        public MessageWrapper(Message message)
        {
            Id = message.ID;
            if (message.Project != null)
            {
                ProjectOwner = new SimpleProjectWrapper(message.Project);
            }
            Title = message.Title;
            Text = message.Content;
            Created = (ApiDateTime)message.CreateOn;
            CreatedBy = new EmployeeWraperFull(CoreContext.UserManager.GetUsers(message.CreateBy));
            Updated = (ApiDateTime)message.LastModifiedOn;
            if (message.CreateBy != message.LastModifiedBy)
            {
                UpdatedBy = EmployeeWraper.Get(message.LastModifiedBy);
            }
            CanEdit = ProjectSecurity.CanEdit(message);
            CommentsCount = message.CommentsCount;
        }


        public static MessageWrapper GetSample()
        {
            return new MessageWrapper
                       {
                           Id = 10,
                           ProjectOwner = SimpleProjectWrapper.GetSample(),
                           Title = "Sample Title",
                           Text = "Hello, this is sample message",
                           Created = (ApiDateTime) DateTime.Now,
                           CreatedBy = EmployeeWraper.GetSample(),
                           Updated = (ApiDateTime) DateTime.Now,
                           UpdatedBy = EmployeeWraper.GetSample(),
                           CanEdit = true,
                           CommentsCount = 5
                       };
        }
    }
}
