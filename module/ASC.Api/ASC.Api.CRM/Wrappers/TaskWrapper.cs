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

#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Api.Employee;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Specific;
using ASC.Core.Tenants;

#endregion

namespace ASC.Api.CRM.Wrappers
{
    /// <summary>
    ///  Task
    /// </summary>
    [DataContract(Name = "task", Namespace = "")]
    public class TaskWrapper : ObjectWrapperBase
    {

        public TaskWrapper(int id): base(id)
        {
            
        }

        public TaskWrapper(Task task): base(task.ID)
        {
            CreateBy = EmployeeWraper.Get(task.CreateBy);
            Created = (ApiDateTime)task.CreateOn;
            Title = task.Title;
            Description = task.Description;
            DeadLine = (ApiDateTime)task.DeadLine;
            Responsible = EmployeeWraper.Get(task.ResponsibleID);
            IsClosed = task.IsClosed;
            AlertValue = task.AlertValue;
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime Created { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactBaseWithEmailWrapper Contact { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Title { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Description { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime DeadLine { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int AlertValue { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper Responsible { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsClosed { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TaskCategoryBaseWrapper Category { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EntityWrapper Entity { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool CanEdit { get; set; }

        public static TaskWrapper GetSample()
        {
            return new TaskWrapper(0)
                       {
                           Created = (ApiDateTime)DateTime.UtcNow,
                           CreateBy = EmployeeWraper.GetSample(),
                           DeadLine = (ApiDateTime)DateTime.UtcNow.AddMonths(1),
                           IsClosed = false,
                           Responsible = EmployeeWraper.GetSample(),
                           Category = TaskCategoryBaseWrapper.GetSample(),
                           CanEdit = true,
                           Title = "Send a commercial offer",
                           AlertValue = 0
                       };
        }
    }

    [DataContract(Name = "taskBase", Namespace = "")]
    public class TaskBaseWrapper : ObjectWrapperBase
    {
        public TaskBaseWrapper(int id): base(id)
        {

        }

        public TaskBaseWrapper(Task task): base(task.ID)
        {
            Title = task.Title;
            Description = task.Description;
            DeadLine = (ApiDateTime)task.DeadLine;
            Responsible = EmployeeWraper.Get(task.ResponsibleID);
            IsClosed = task.IsClosed;
            AlertValue = task.AlertValue;
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Title { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Description { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime DeadLine { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int AlertValue { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper Responsible { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsClosed { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TaskCategoryBaseWrapper Category { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EntityWrapper Entity { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool CanEdit { get; set; }

        public static TaskBaseWrapper GetSample()
        {
            return new TaskBaseWrapper(0)
                       {
                           DeadLine = (ApiDateTime)DateTime.UtcNow.AddMonths(1),
                           IsClosed = false,
                           Responsible = EmployeeWraper.GetSample(),
                           Category = TaskCategoryBaseWrapper.GetSample(),
                           CanEdit = true,
                           Title = "Send a commercial offer",
                           AlertValue = 0
                       };
        }
    }
}
