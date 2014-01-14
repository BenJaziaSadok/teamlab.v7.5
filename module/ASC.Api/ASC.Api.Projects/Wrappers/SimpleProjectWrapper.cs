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

using System.Runtime.Serialization;
using ASC.Projects.Core.Domain;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "project", Namespace = "")]
    public class SimpleProjectWrapper
    {
        [DataMember(Order = 60)]
        public int Id { get; set; }

        [DataMember(Order = 61)]
        public string Title { get; set; }

        [DataMember(Order = 62)]
        public int Status { get; set; }

        [DataMember(Order = 63)]
        public bool IsPrivate {get; set;}

        public SimpleProjectWrapper()
        {
            
        }

        public SimpleProjectWrapper(Project project)
        {
            Id = project.ID;
            Title = project.Title;
            Status = (int)project.Status;
            IsPrivate = project.Private;
        }

        public SimpleProjectWrapper(int projectId, string projectTitle)
        {
            Id = projectId;
            Title = projectTitle;
        }

        public static SimpleProjectWrapper GetSample()
        {
            return new SimpleProjectWrapper(123, "Sample project");
        }
    }
}
