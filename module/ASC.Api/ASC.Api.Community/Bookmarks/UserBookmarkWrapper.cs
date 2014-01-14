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
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.Specific;
using ASC.Bookmarking.Pojo;

namespace ASC.Api.Bookmarks
{
    [DataContract(Name = "userbookmark", Namespace = "")]
    public class UserBookmarkWrapper
    {
        [DataMember(Order = 0)]
        public long BookmarkId { get; set; }

        [DataMember(Order = 1)]
        public EmployeeWraper CreatedBy { get; set; }

        [DataMember(Order = 1)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 2)]
        public string Title { get; set; }

        [DataMember(Order = 3)]
        public string Description { get; set; }

        public UserBookmarkWrapper(UserBookmark userBookmark)
        {
            BookmarkId = userBookmark.BookmarkID;
            CreatedBy = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(userBookmark.UserID));
            Created = (ApiDateTime) userBookmark.DateAdded;
            Title = userBookmark.Name;
            Description = userBookmark.Description;
        }
    }
}
