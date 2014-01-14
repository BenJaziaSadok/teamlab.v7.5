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
using ASC.Bookmarking.Pojo;
using System.Web;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Bookmarking.Common.Util
{
    public static class BookmarkingBusinessUtil
    {
        public static string GenerateBookmarkInfoUrl(Bookmark b)
        {
            return Business.BookmarkingService.ModifyBookmarkUrl(b);
        }

        public static string GenerateBookmarksUrl(Bookmark b)
        {
            return VirtualPathUtility.ToAbsolute(BookmarkingBusinessConstants.BookmarkingBasePath + "/default.aspx");
        }

        public static string RenderProfileLink(Guid userID)
        {
            return CoreContext.UserManager.GetUsers(userID).RenderCustomProfileLink(BookmarkingBusinessConstants.CommunityProductID, "describe-text", "link gray");
        }
    }
}