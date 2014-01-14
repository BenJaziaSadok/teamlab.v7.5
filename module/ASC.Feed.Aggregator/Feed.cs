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

namespace ASC.Feed.Aggregator
{
    public class Feed
    {
        public Feed()
        {
        }

        
        
        public bool Variate { get; private set; }

        public Feed(Guid author, DateTime date, bool variate = false)
        {
            Author = Helper.GetUser(author);
            Date = date;
            LastModifiedBy = Author.UserInfo.ID;

            Action = FeedAction.Created;
            Variate = variate;
        }

        public string Item { get; set; }

        public string ItemId { get; set; }

        public string Id
        {
            get { return string.Format("{0}_{1}", Item, ItemId); }
        }

        public UserWrapper Author { get; private set; }

        public Guid LastModifiedBy { get; set; }

        public DateTime Date { get; private set; }

        public string Product { get; set; }

        public string Module { get; set; }

        public string Location { get; set; }

        public FeedAction Action { get; set; }

        public string Title { get; set; }

        public string ItemUrl { get; set; }

        public string Description { get; set; }

        public string AdditionalInfo { get; set; }

        public string AdditionalInfo2 { get; set; }

        public string AdditionalInfo3 { get; set; }

        public string AdditionalInfo4 { get; set; }

        public string Keywords { get; set; }

        public bool HasPreview { get; set; }

        public bool CanComment { get; set; }

        public object Target { get; set; }

        public string CommentApiUrl { get; set; }

        public IEnumerable<FeedComment> Comments { get; set; }

        public string GroupId { get; set; }
    }
}