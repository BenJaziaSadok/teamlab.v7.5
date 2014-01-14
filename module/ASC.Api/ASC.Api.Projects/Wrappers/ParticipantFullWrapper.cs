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

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "participant_full", Namespace = "")]
    public class ParticipantFullWrapper
    {
        [DataMember(Order = 100)]
        public ProjectWrapper Project { get; set; }

        [DataMember(Order = 200)]
        public EmployeeWraper Participant{ get; set; }

        [DataMember(Order = 201)]
        public DateTime Created { get; set; }

        [DataMember(Order = 202)]
        public DateTime Updated { get; set; }

        [DataMember(Order = 203)]
        public bool Removed { get; set; }


        public ParticipantFullWrapper(ParticipantFull participant)
        {
            Project = new ProjectWrapper(participant.Project);
            Participant = EmployeeWraper.Get(participant.ID);
            Created = participant.Created;
            Updated = participant.Updated;
            Removed = participant.Removed;
        }
    }
}
