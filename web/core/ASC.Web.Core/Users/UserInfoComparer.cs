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
using ASC.Core.Users;

namespace ASC.Web.Core.Users
{
    public class UserInfoComparer : IComparer<UserInfo>
    {
        public static readonly IComparer<UserInfo> Default = new UserInfoComparer(UserSortOrder.DisplayName, false);


        public UserSortOrder SortOrder { get; set; }
        public bool Descending { get; set; }


        public UserInfoComparer(UserSortOrder sortOrder)
            : this(sortOrder, false)
        {
        }

        public UserInfoComparer(UserSortOrder sortOrder, bool descending)
        {
            SortOrder = sortOrder;
            Descending = descending;
        }


        public int Compare(UserInfo x, UserInfo y)
        {
            int result = 0;
            switch (SortOrder)
            {
                case UserSortOrder.DisplayName:
                    result = UserFormatter.Compare(x, y);
                    break;
            }

            return !Descending ? result : -result;
        }
    }
}
