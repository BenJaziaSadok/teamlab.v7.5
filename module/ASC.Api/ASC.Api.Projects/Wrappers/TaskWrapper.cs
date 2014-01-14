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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Specific;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "task", Namespace = "")]
    public class TaskWrapper : ObjectWrapperFullBase
    {
        [DataMember]
        public bool CanEdit { get; set; }

        [DataMember]
        public bool CanCreateSubtask { get; set; }

        [DataMember]
        public bool CanCreateTimeSpend { get; set; }

        [DataMember]
        public bool CanDelete { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public ApiDateTime Deadline { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ApiDateTime StartDate { get; set; }

        [DataMember(Order = 13, EmitDefaultValue = false)]
        public int MilestoneId { get; set; }

        [DataMember(Order = 12)]
        public TaskPriority Priority { get; set; }

        [DataMember(Order = 14)]
        public SimpleProjectWrapper ProjectOwner { get; set; }
        
        [DataMember(Order = 20, EmitDefaultValue = false)]
        public List<SubtaskWrapper> Subtasks { get; set; }
        
        [DataMember(Order = 21, EmitDefaultValue = false)]
        public IEnumerable<TaskLinkWrapper> Links { get; set; }

        [DataMember(Order = 53)]
        public List<EmployeeWraper> Responsibles { get; set; }

        [DataMember(Order = 54, EmitDefaultValue = false)]
        public SimpleMilestoneWrapper Milestone { get; set; }


        private TaskWrapper()
        {
        }

        public TaskWrapper(Task task)
        {
            Id = task.ID;
            Title = task.Title;
            Description = task.Description;
            Status = (int)task.Status;

            if (task.Responsibles != null)
            {
                Responsibles = task.Responsibles.Select(EmployeeWraper.Get).OrderBy(r=> r.DisplayName).ToList();
            }


            Deadline = (task.Deadline == DateTime.MinValue ? null : new ApiDateTime(task.Deadline, TimeZoneInfo.Local));
            Priority = task.Priority;
            ProjectOwner = new SimpleProjectWrapper(task.Project);
            MilestoneId = task.Milestone;
            Created = (ApiDateTime)task.CreateOn;
            CreatedBy = EmployeeWraper.Get(task.CreateBy);
            Updated = (ApiDateTime)task.LastModifiedOn;
            StartDate = task.StartDate.Equals(DateTime.MinValue) ? null : (ApiDateTime)task.StartDate;

            if (task.CreateBy != task.LastModifiedBy)
            {
                UpdatedBy = EmployeeWraper.Get(task.LastModifiedBy);
            }

            if (task.SubTasks != null)
            {
                Subtasks = task.SubTasks.Select(x => new SubtaskWrapper(x, task)).ToList();
            }

            CanEdit = ProjectSecurity.CanEdit(task);
            CanCreateSubtask = ProjectSecurity.CanCreateSubtask(task);
            CanCreateTimeSpend = ProjectSecurity.CanCreateTimeSpend(task);
            CanDelete = ProjectSecurity.CanDelete(task);

            if (task.Milestone != 0 && task.MilestoneDesc != null)
            {
                Milestone = new SimpleMilestoneWrapper(task.MilestoneDesc);
            }

            if (task.Links.Any())
            {
                Links = task.Links.Select(r=> new TaskLinkWrapper(r));
            }
        }

        public TaskWrapper(Task task, Milestone milestone) : this(task)
        {
            if (task.Milestone != 0)
                Milestone = new SimpleMilestoneWrapper(milestone);
        }


        public static TaskWrapper GetSample()
        {
            return new TaskWrapper
                       {
                           Id = 10,
                           Title = "Sample Title",
                           Description = "Sample description",
                           Deadline = (ApiDateTime)DateTime.Now,
                           Priority = TaskPriority.High,
                           Status = (int)MilestoneStatus.Open,
                           Responsible = EmployeeWraper.GetSample(),
                           ProjectOwner = SimpleProjectWrapper.GetSample(),
                           MilestoneId = 123,
                           Created = (ApiDateTime) DateTime.Now,
                           CreatedBy = EmployeeWraper.GetSample(),
                           Updated = (ApiDateTime) DateTime.Now,
                           UpdatedBy = EmployeeWraper.GetSample(),
                           StartDate = (ApiDateTime)DateTime.Now
                       };
        }
    }
}
