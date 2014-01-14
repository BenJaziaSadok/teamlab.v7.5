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
using ASC.Blogs.Core.Domain;

namespace ASC.Api.Blogs
{
    [DataContract(Name = "tag", Namespace = "")]
    public class BlogTagWrapper
    {

        public BlogTagWrapper(TagStat tagStat)
        {
            Name = tagStat.Name;
            Count = tagStat.Count;
        }

        private BlogTagWrapper()
        {
        }

        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 10)]
        public int Count { get; set; }

        public static BlogTagWrapper GetSample()
        {
            return new BlogTagWrapper()
                       {
                           Count = 10,
                           Name = "Sample tag"
                       };
        }
    }
}