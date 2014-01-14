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
using ASC.Projects.Engine;
using ASC.Specific;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "milestone", Namespace = "")]
    public class MilestoneWrapper : ObjectWrapperFullBase
    {
        [DataMember(Order = 14)]
        public SimpleProjectWrapper ProjectOwner { get; set; }

        [DataMember(Order = 20)]
        public ApiDateTime Deadline { get; set; }

        [DataMember(Order = 20)]
        public bool IsKey { get; set; }

        [DataMember(Order = 20)]
        public bool IsNotify { get; set; }

        [DataMember]
        public bool CanEdit { get; set; }

        [DataMember(Order = 20)]
        public int ActiveTaskCount { get; set; }

        [DataMember(Order = 20)]
        public int ClosedTaskCount { get; set; }


        private MilestoneWrapper()
        {
        }

        public MilestoneWrapper(Milestone milestone)
        {
            Id = milestone.ID;
            ProjectOwner = new SimpleProjectWrapper(milestone.Project);
            Title = milestone.Title;
            Description = milestone.Description;
            Created = (ApiDateTime)milestone.CreateOn;
            CreatedBy = EmployeeWraper.Get(milestone.CreateBy);
            Updated = (ApiDateTime)milestone.LastModifiedOn;
            if (milestone.CreateBy != milestone.LastModifiedBy)
            {
                UpdatedBy = EmployeeWraper.Get(milestone.LastModifiedBy);
            }
            if (!milestone.Responsible.Equals(Guid.Empty))
            {
                Responsible = EmployeeWraper.Get(milestone.Responsible);
            }
            Status = (int)milestone.Status;
            Deadline = new ApiDateTime(milestone.DeadLine, TimeZoneInfo.Local);
            IsKey = milestone.IsKey;
            IsNotify = milestone.IsNotify;
            CanEdit = ProjectSecurity.CanEdit(milestone);
            ActiveTaskCount = milestone.ActiveTaskCount;
            ClosedTaskCount = milestone.ClosedTaskCount;
        }


        public static MilestoneWrapper GetSample()
        {
            return new MilestoneWrapper
                       {
                           Id = 10,
                           ProjectOwner = SimpleProjectWrapper.GetSample(),
                           Title = "Sample Title",
                           Description = "Sample description",
                           Created = (ApiDateTime) DateTime.Now,
                           CreatedBy = EmployeeWraper.GetSample(),
                           Updated = (ApiDateTime) DateTime.Now,
                           UpdatedBy = EmployeeWraper.GetSample(),
                           Responsible = EmployeeWraper.GetSample(),
                           Status = (int)MilestoneStatus.Open,
                           Deadline = (ApiDateTime) DateTime.Now,
                           IsKey = false,
                           IsNotify = false,
                           CanEdit = true,
                           ActiveTaskCount = 15,
                           ClosedTaskCount = 5
                       };
        }
    }
}
