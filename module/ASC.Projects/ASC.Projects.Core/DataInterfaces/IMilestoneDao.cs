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

#endregion

namespace ASC.Projects.Core.DataInterfaces
{
    public interface IMilestoneDao
    {
        List<Milestone> GetAll();

        List<Milestone> GetByProject(int projectId);

        List<Milestone> GetByStatus(int projectId, MilestoneStatus milestoneStatus);

        List<Milestone> GetUpcomingMilestones(int offset, int max, params int[] projects);

        List<Milestone> GetLateMilestones(int offset, int max, params int[] projects);

        List<Milestone> GetByDeadLine(DateTime deadline);

        List<Milestone> GetByFilter(TaskFilter filter, bool isAdmin);

        int GetByFilterCount(TaskFilter filter, bool isAdmin);

        List<object[]> GetInfoForReminder(DateTime deadline);
        
        Milestone GetById(int id);

        List<Milestone> GetById(int[] id);

        bool IsExists(int id);

        Milestone Save(Milestone milestone);

        void Delete(int id);

        string GetLastModified();
    }
}
