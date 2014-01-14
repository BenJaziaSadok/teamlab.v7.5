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

using System.Runtime.Serialization;
using ASC.Core;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "project_security", Namespace = "")]
    public class ProjectSecurityInfo : CommonSecurityInfo
    {
        [DataMember]
        public bool CanCreateMilestone { get; set; }

        [DataMember]
        public bool CanCreateMessage { get; set; }

        [DataMember]
        public bool CanCreateTask { get; set; }

        [DataMember]
        public bool CanEditTeam { get; set; }

        [DataMember]
        public bool CanReadFiles { get; set; }

        [DataMember]
        public bool CanReadMilestones { get; set; }

        [DataMember]
        public bool CanReadMessages { get; set; }

        [DataMember]
        public bool CanReadTasks { get; set; }

        [DataMember]
        public bool CanLinkContact { get; set; }

        [DataMember]
        public bool IsInTeam { get; set; }

        private ProjectSecurityInfo()
        {
        }

        public ProjectSecurityInfo(Project project)
        {
            CanCreateMilestone = ProjectSecurity.CanCreateMilestone(project);
            CanCreateMessage = ProjectSecurity.CanCreateMessage(project);
            CanCreateTask = ProjectSecurity.CanCreateTask(project);
            CanEditTeam = ProjectSecurity.CanEditTeam(project);
            CanReadFiles = ProjectSecurity.CanReadFiles(project);
            CanReadMilestones = ProjectSecurity.CanReadMilestones(project);
            CanReadMessages = ProjectSecurity.CanReadMessages(project);
            CanReadTasks = ProjectSecurity.CanReadTasks(project);
            IsInTeam = ProjectSecurity.IsInTeam(project, SecurityContext.CurrentAccount.ID, false);
            CanLinkContact = ProjectSecurity.CanLinkContact(project);

        }


        public static ProjectSecurityInfo GetSample()
        {
            return new ProjectSecurityInfo();
        }
    }
}
