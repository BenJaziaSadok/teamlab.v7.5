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
using System.Runtime.Serialization;
using ASC.Projects.Core.Domain;
using ASC.Specific;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "milestone", Namespace = "")]
    public class SimpleMilestoneWrapper
    {
        [DataMember(Order = 60)]
        public int Id { get; set; }

        [DataMember(Order = 61)]
        public string Title { get; set; }

        [DataMember(Order = 62)]
        public ApiDateTime Deadline { get; set; }


        public SimpleMilestoneWrapper()
        {
        }

        public SimpleMilestoneWrapper(Milestone milestone)
        {
            Id = milestone.ID;
            Title = milestone.Title;
            Deadline = (ApiDateTime)milestone.DeadLine;
        }


        public static SimpleMilestoneWrapper GetSample()
        {
            return new SimpleMilestoneWrapper
                       {
                           Id = 123,
                           Title = "Milestone",
                           Deadline = (ApiDateTime) DateTime.Now,
                       };
        }
    }
}
