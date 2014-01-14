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
using ASC.Projects.Engine;
using ASC.Specific;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "search_item", Namespace = "")]
    public class SearchItemWrapper
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 3)]
        public EntityType EntityType { get; set; }

        [DataMember(Order = 5)]
        public string Title { get; set; }

        [DataMember(Order = 10)]
        public string Description { get; set; }

        [DataMember(Order = 20)]
        public ApiDateTime Created { get; set; }


        private SearchItemWrapper()
        {
        }

        public SearchItemWrapper(SearchItem searchItem)
        {
            Id = searchItem.ID;
            Title = searchItem.Title;
            EntityType = searchItem.EntityType;
            Created = (ApiDateTime) searchItem.CreateOn;
            Description = searchItem.Description;
        }


        public static SearchItemWrapper GetSample()
        {
            return new SearchItemWrapper
                       {
                           Id = "345",
                           EntityType = EntityType.Project,
                           Title = "Sample title",
                           Description = "Sample desription",
                           Created = (ApiDateTime) DateTime.Now,
                       };
        }
    }
}
