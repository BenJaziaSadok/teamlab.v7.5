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

using System.Collections.Generic;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Core.DataInterfaces
{
    public interface ITimeSpendDao
    {
        List<TimeSpend> GetByTask(int taskId);

        List<TimeSpend> GetByProject(int projectId);

        TimeSpend GetById(int id);

        List<TimeSpend> GetByFilter(TaskFilter filter);

        TimeSpend Save(TimeSpend timeSpend);

        void Delete(int id);
    }
}
