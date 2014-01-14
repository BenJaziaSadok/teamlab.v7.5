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
    public interface ITaskDao
    {
        List<Task> GetAll();

        List<Task> GetByProject(int projectId, TaskStatus? status, Guid participant);

        List<Task> GetByResponsible(Guid responsibleId, IEnumerable<TaskStatus> statuses);

        List<Task> GetMilestoneTasks(int milestoneId);

        List<Task> GetByFilter(TaskFilter filter, bool isAdmin);

        TaskFilterCountOperationResult GetByFilterCount(TaskFilter filter, bool isAdmin);

        List<Task> GetById(ICollection<int> ids);

        Task GetById(int id);

        bool IsExists(int id);

        List<object[]> GetTasksForReminder(DateTime deadline);


        Task Save(Task task);

        void Delete(int id);


        void SaveRecurrence(Task task, string cron, DateTime startDate, DateTime endDate);

        void DeleteReccurence(int taskId);

        List<object[]> GetRecurrence(DateTime date);


        void AddLink(TaskLink link);

        void RemoveLink(TaskLink link);

        IEnumerable<TaskLink> GetLinks(int taskID);

        IEnumerable<TaskLink> GetLinks(List<Task> tasks);

        bool IsExistLink(TaskLink link);
    }
}
