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
using ASC.Api.Employee;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "participant", Namespace = "")]
    public class ParticipantWrapper : EmployeeWraperFull
    {
        [DataMember]
        public bool CanReadFiles { get; set; }

        [DataMember]
        public bool CanReadMilestones { get; set; }

        [DataMember]
        public bool CanReadMessages { get; set; }

        [DataMember]
        public bool CanReadTasks { get; set; }

        [DataMember]
        public bool CanReadContacts { get; set; }
        
        [DataMember]
        public bool IsAdministrator { get; set; }

        public ParticipantWrapper(Participant participant) : base(participant.UserInfo)
        {  
            CanReadFiles = participant.CanReadFiles;
            CanReadMilestones = participant.CanReadMilestones;
            CanReadMessages = participant.CanReadMessages;
            CanReadTasks = participant.CanReadTasks;
            CanReadContacts = participant.CanReadContacts;
            IsAdministrator = ProjectSecurity.IsAdministrator(participant.ID);
        }
    }
}
