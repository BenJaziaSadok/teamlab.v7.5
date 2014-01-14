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
    public class ProjectWrapper : ObjectWrapperBase
    {
        [DataMember(Order = 31)]
        public bool CanEdit { get; set; }

        [DataMember(Order = 32)]
        public bool IsPrivate { get; set; }

        public ProjectWrapper(Project project)
        {
            Id = project.ID;
            Title = project.Title;
            Description = project.Description;
            Responsible = EmployeeWraper.Get(project.Responsible);
            Status = (int) project.Status;
            CanEdit = ProjectSecurity.CanEdit(project);
            IsPrivate = project.Private;
        }

        private ProjectWrapper()
        {
        }


        public static ProjectWrapper GetSample()
        {
            return new ProjectWrapper
                       {

                           Id = 10,
                           Title = "Sample Title",
                           Description = "Sample description",
                           Responsible = EmployeeWraper.GetSample(),
                           Status = (int) ProjectStatus.Open,
                       };
        }
    }
}
