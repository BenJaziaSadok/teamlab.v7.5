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
using System.Runtime.Serialization;
using ASC.Bookmarking.Pojo;

namespace ASC.Api.Bookmarks
{
    [DataContract(Name = "tag", Namespace = "")]
    public class TagWrapper
    {

        public TagWrapper(Tag tagStat)
        {
            Name = tagStat.Name;
            Count = tagStat.Populatiry;
        }

        private TagWrapper()
        {
        }

        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 10)]
        public long Count { get; set; }

        public static TagWrapper GetSample()
        {
            return new TagWrapper()
            {
                Count = 10,
                Name = "Sample tag"
            };
        }
    }
}