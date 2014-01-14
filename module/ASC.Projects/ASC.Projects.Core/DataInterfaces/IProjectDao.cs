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
using ASC.Projects.Core.Domain;
using System.Collections;

#endregion

namespace ASC.Projects.Core.DataInterfaces
{
    public interface IProjectDao
    {
        List<Project> GetAll(ProjectStatus? status, int max);

        List<Project> GetLast(ProjectStatus? status, int offset, int max);
        
        List<Project> GetByParticipiant(Guid participantId, ProjectStatus status);

        List<Project> GetFollowing(Guid participantId);

        List<Project> GetOpenProjectsWithTasks(Guid participantId);
            
        
        DateTime GetMaxLastModified();

        void UpdateLastModified(int projectID);
            
        Project GetById(int projectId);

        Project GetFullProjectById(int projectId);

        List<Project> GetById(ICollection projectIDs);

        bool IsExists(int projectId);

        bool IsFollow(int projectId, Guid participantId);
        
        int Count();

        List<int> GetTaskCount(List<int> projectId, TaskStatus? taskStatus, bool isAdmin);

        int GetMessageCount(int projectId);
        
        int GetTotalTimeCount(int projectId);
        
        int GetMilestoneCount(int projectId, params MilestoneStatus[] milestoneStatus);

        Project Save(Project project);

        void Delete(int projectId);


        void AddToTeam(int projectId, Guid participantId);

        void RemoveFromTeam(int projectId, Guid participantId);

        bool IsInTeam(int projectId, Guid participantId);

        List<Participant> GetTeam(int projectId);

        List<Participant> GetTeam(List<int> projectId);

        List<ParticipantFull> GetTeamUpdates(DateTime from, DateTime to);

        DateTime GetTeamMaxLastModified();

        void SetTeamSecurity(int projectId, Guid participantId, ProjectTeamSecurity teamSecurity);

        ProjectTeamSecurity GetTeamSecurity(int projectId, Guid participantId);


        List<Project> GetByFilter(TaskFilter filter, bool isAdmin);

        int GetByFilterCount(TaskFilter filter, bool isAdmin);


        List<Project> GetByContactID(int contactID);

        void AddProjectContact(int projectID, int contactID);

        void DeleteProjectContact(int projectID, int contactid);

        
        void SetTaskOrder(int projectID, string order);

        string GetTaskOrder(int projectID);
    }
}
