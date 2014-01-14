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

namespace ASC.Projects.Core.Domain
{
    public class TaskFilterCountOperationResult
    {
        public int TasksOpen { get; set; }

        public int TasksClosed { get; set; }

        public int TasksTotal
        {
            get { return TasksOpen + TasksClosed; }
        }
    }

    public class TaskFilterOperationResult
    {
        public List<Task> FilterResult { get; set; }
        
        public TaskFilterCountOperationResult FilterCount { get; set; }

        public TaskFilterOperationResult()
            : this(new TaskFilterCountOperationResult())
        {
        }

        public TaskFilterOperationResult(TaskFilterCountOperationResult count)
            : this(new List<Task>(), count)
        {
        }

        public TaskFilterOperationResult(List<Task> tasks, TaskFilterCountOperationResult count)
        {
            FilterResult = tasks;
            FilterCount = count;
        }
    }
}
