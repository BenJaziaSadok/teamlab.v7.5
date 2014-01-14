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

namespace ASC.Feed.Data
{
    public class FeedRow
    {
        public string Id { get; set; }

        public bool ClearRightsBeforeInsert { get; set; }

        public int Tenant { get; set; }

        public string ProductId { get; set; }

        public string ModuleId { get; set; }

        public Guid AuthorId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string GroupId { get; set; }

        public string Json { get; set; }

        public string Keywords { get; set; }

        public DateTime AggregatedDate { get; set; }

        public IList<Guid> Users { get; set; }


        public FeedRow()
        {
            Users = new List<Guid>();
        }
    }
}