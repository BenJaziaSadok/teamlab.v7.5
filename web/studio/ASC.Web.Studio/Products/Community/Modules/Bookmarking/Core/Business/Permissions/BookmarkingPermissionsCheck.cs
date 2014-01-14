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

using ASC.Bookmarking.Common;
using ASC.Bookmarking.Pojo;
using ASC.Web.Community.Product;

#endregion

namespace ASC.Bookmarking.Business.Permissions
{
    public static class BookmarkingPermissionsCheck
    {
        public static bool PermissionCheckCreateBookmark()
        {
            return CommunitySecurity.CheckPermissions(BookmarkingBusinessConstants.BookmarkCreateAction);
        }

        public static bool PermissionCheckAddToFavourite()
        {
            return CommunitySecurity.CheckPermissions(BookmarkingBusinessConstants.BookmarkAddToFavouriteAction);
        }

        public static bool PermissionCheckRemoveFromFavourite(UserBookmark b)
        {
            return CommunitySecurity.CheckPermissions(new BookmarkPermissionSecurityObject(b.UserID), BookmarkingBusinessConstants.BookmarkRemoveFromFavouriteAction);
        }

        public static bool PermissionCheckCreateComment()
        {
            return CommunitySecurity.CheckPermissions(BookmarkingBusinessConstants.BookmarkCreateCommentAction);
        }

        public static bool PermissionCheckEditComment()
        {
            return CommunitySecurity.CheckPermissions(BookmarkingBusinessConstants.BookmarkEditCommentAction);
        }

        public static bool PermissionCheckEditComment(Comment c)
        {
            return CommunitySecurity.CheckPermissions(new BookmarkPermissionSecurityObject(c.UserID, c.ID), BookmarkingBusinessConstants.BookmarkEditCommentAction);
        }
    }
}
