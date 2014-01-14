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
using System.Diagnostics;

#endregion

namespace ASC.Projects.Core.Domain
{
    [DebuggerDisplay("SubTask: ID = {ID}, Title = {Title}")]
    public class Subtask
    {
        public int ID { get; set; }

        public String Title { get; set; }

        public Guid Responsible { get; set; }

        public TaskStatus Status { get; set; }

        public int Task { get; set; }

        public Task ParentTask { get; set; }

        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public Guid LastModifiedBy { get; set; }

        public DateTime StatusChangedOn { get; set; }
    }
}
