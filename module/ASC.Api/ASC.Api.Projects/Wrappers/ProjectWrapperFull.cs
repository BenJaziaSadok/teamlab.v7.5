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
    [DataContract(Name = "project", Namespace = "")]
    public class ProjectWrapperFull : ObjectWrapperFullBase
    {
        [DataMember]
        public bool CanEdit { get; set; }

        [DataMember]
        public ProjectSecurityInfo Security { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public object ProjectFolder { get; set; }

        [DataMember(Order = 32)]
        public bool IsPrivate { get; set; }

        [DataMember(Order = 33)]
        public int TaskCount { get; set; }

        [DataMember(Order = 34)]
        public int MilestoneCount { get; set; }

        [DataMember(Order = 35)]
        public int ParticipantCount { get; set; }


        private ProjectWrapperFull()
        {
        }

        public ProjectWrapperFull(Project project, object filesRoot)
        {
            Id = project.ID;
            Title = project.Title;
            Description = project.Description;
            Status = (int) project.Status;
            Responsible = EmployeeWraper.Get(project.Responsible);
            Created = (ApiDateTime) project.CreateOn;
            CreatedBy = EmployeeWraper.Get(project.CreateBy);
            Updated = (ApiDateTime) project.LastModifiedOn;
            if (project.CreateBy!=project.LastModifiedBy)
            {
                UpdatedBy = EmployeeWraper.Get(project.LastModifiedBy);
            }
            Security = new ProjectSecurityInfo(project);
            CanEdit = ProjectSecurity.CanEdit(project);
            ProjectFolder = filesRoot;
            IsPrivate = project.Private;
            TaskCount = project.TaskCount;
            MilestoneCount = project.MilestoneCount;
            ParticipantCount = project.ParticipantCount;
        }

        public ProjectWrapperFull(Project project) : this(project, 0)
        {
        }


        public static ProjectWrapperFull GetSample()
        {
            return new ProjectWrapperFull
                       {
                           Id = 10,
                           Title = "Sample Title",
                           Description = "Sample description",
                           Status = (int)MilestoneStatus.Open,
                           Responsible = EmployeeWraper.GetSample(),
                           Created = (ApiDateTime) DateTime.Now,
                           CreatedBy = EmployeeWraper.GetSample(),
                           Updated = (ApiDateTime) DateTime.Now,
                           UpdatedBy = EmployeeWraper.GetSample(),
                           ProjectFolder = 13234
                       };
        }
    }
}
