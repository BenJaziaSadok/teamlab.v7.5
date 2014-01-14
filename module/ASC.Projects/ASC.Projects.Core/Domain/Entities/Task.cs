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
using System.Diagnostics;

#endregion

namespace ASC.Projects.Core.Domain
{
    [DebuggerDisplay("Task: ID = {ID}, Title = {Title}, Status = {Status}")]
    public class Task : ProjectEntity
    {
        public string Description { get; set; }

        public TaskPriority Priority { get; set; }

        public TaskStatus Status { get; set; }

        public int Milestone { get; set; }

        public int SortOrder { get; set; }

        public DateTime Deadline { get; set; }

        public List<Subtask> SubTasks { get; set; }

        public HashSet<Guid> Responsibles { get; set; }

        public List<TaskLink> Links { get; set; }

        public Milestone MilestoneDesc { get; set; }

        public DateTime StatusChangedOn { get; set; }

        public DateTime StartDate { get; set; }

        public Task()
        {
            Responsibles = new HashSet<Guid>();
            SubTasks = new List<Subtask>();
            Links = new List<TaskLink>();
        }
    }
}
