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
    public class TaskLinkWrapper
    {
        [DataMember]
        public int DependenceTaskId { get; set; }

        [DataMember]
        public int ParentTaskId { get; set; }

        [DataMember]
        public TaskLinkType LinkType { get; set; }

        public TaskLinkWrapper(TaskLink link)
        {
            DependenceTaskId = link.DependenceTaskId;
            ParentTaskId = link.ParentTaskId;
            LinkType = link.LinkType;
        }
    }
}
