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
using ASC.Core.Users;
using System.Collections.Generic;

namespace ASC.Feed
{
    public class FeedMin
    {
        public string Id { get; set; }

        public FeedMinUser Author { get; set; }

        public DateTime Date { get; set; }

        public string Product { get; set; }

        public string Item { get; set; }

        public string Title { get; set; }

        public string ItemUrl { get; set; }

        public string Description { get; set; }

        public string AdditionalInfo { get; set; }

        public string AdditionalInfo2 { get; set; }

        public string Module { get; set; }

        public IEnumerable<FeedComment> Comments { get; set; }

        public class FeedMinUser
        {
            public UserInfo UserInfo { get; set; }
        }

        public class FeedComment
        {
            public FeedMinUser Author { get; set; }

            public string Description { get; set; }

            public DateTime Date { get; set; }


            public FeedMin ToFeedMin()
            {
                return new FeedMin { Author = this.Author, Title = this.Description, Date = this.Date };
            }
        }
    }
}
