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
    public interface ISubtaskDao
    {
        List<Subtask> GetSubtasks(int taskid);

        void GetSubtasks(ref List<Task> tasks);

        void CloseAllSubtasks(Task task);

        List<Subtask> GetById(ICollection<int> ids);

        Subtask GetById(int id);

        List<Subtask> GetUpdates(DateTime from, DateTime to); 

        int GetSubtaskCount(int taskid, params TaskStatus[] statuses);

        Subtask Save(Subtask task);

        void Delete(int id);
    }
}
