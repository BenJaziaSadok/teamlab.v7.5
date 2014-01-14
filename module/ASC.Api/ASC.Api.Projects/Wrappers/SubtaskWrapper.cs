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
    [DataContract(Name = "subtask", Namespace = "")]
    public class SubtaskWrapper : ObjectWrapperFullBase
    {
        [DataMember]
        public bool CanEdit { get; set; }

        private SubtaskWrapper()
        {
        }

        public SubtaskWrapper(Subtask subtask, Task task)
        {
            Id = subtask.ID;
            Title = subtask.Title;
            Status = (int)subtask.Status;
            if (subtask.Responsible != Guid.Empty)
            {
                Responsible = EmployeeWraper.Get(subtask.Responsible);
            }
            Created = (ApiDateTime)subtask.CreateOn;
            CreatedBy = EmployeeWraper.Get(subtask.CreateBy);
            Updated = (ApiDateTime)subtask.LastModifiedOn;
            if (subtask.CreateBy != subtask.LastModifiedBy)
            {
                UpdatedBy = EmployeeWraper.Get(subtask.LastModifiedBy);
            }
            CanEdit = ProjectSecurity.CanEdit(task, subtask);
        }


        public static SubtaskWrapper GetSample()
        {
            return new SubtaskWrapper
                       {
                           Id = 1233,
                           Title = "Sample subtask",
                           Description = "Sample description",
                           Status = (int) TaskStatus.Open,
                           Responsible = EmployeeWraper.GetSample(),
                           Created = (ApiDateTime)DateTime.Now,
                           CreatedBy = EmployeeWraper.GetSample(),
                           Updated = (ApiDateTime)DateTime.Now,
                           UpdatedBy = EmployeeWraper.GetSample(),
                       };
        }
    }
}
