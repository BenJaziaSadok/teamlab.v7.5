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

namespace ASC.Feed.Aggregator
{
    public class FeedComment
    {
        public UserWrapper Author { get; private set; }

        public FeedComment(Guid author)
        {
            Author = Helper.GetUser(author);
        }

        public string Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}