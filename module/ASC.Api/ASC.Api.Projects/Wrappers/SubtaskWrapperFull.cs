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

using System.Runtime.Serialization;
using ASC.Projects.Core.Domain;

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "subtask")]
    public class SubtaskWrapperFull : SubtaskWrapper
    {
        [DataMember(Name = "parent")]
        public SimpleTaskWrapper ParentTask { get; set; }

        public SubtaskWrapperFull(Subtask subtask)
            : base(subtask, subtask.ParentTask)
        {
            ParentTask = new SimpleTaskWrapper(subtask.ParentTask);
        }
    }
}
