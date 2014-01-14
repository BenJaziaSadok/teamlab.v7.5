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

namespace ASC.Projects.Core.Domain.Entities.Feed
{
    [DataContract(Name = "comment")]
    public class ProjectComment
    {
        public Comment Comment { get; set; }
        
        public int CommentedId { get; set; }
        
        public string CommentedTitle { get; set; }
        
        public int ProjectId { get; set; }

        public Message Discussion { get; set; }
        
        public Task Task { get; set; }

        public EntityType CommentedType { get; set; }
    }
}
