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

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ASC.Projects.Core.Domain
{
    [DebuggerDisplay("Milestone: ID = {ID}, Title = {Title}, DeadLine = {DeadLine}")]
    public class Milestone : ProjectEntity
    {
        public String Description { get; set; }

        public Guid Responsible { get; set; }

        public MilestoneStatus Status { get; set; }

        public bool IsNotify { get; set; }

        public bool IsKey { get; set; }

        public DateTime DeadLine { get; set; }

        public bool CurrentUserHasTasks { get; set; }

        public int ActiveTaskCount { get; set; }

        public int ClosedTaskCount { get; set; }

        public DateTime StatusChangedOn { get; set; }

        public List<Task> Tasks { get; set; }
    }
}
