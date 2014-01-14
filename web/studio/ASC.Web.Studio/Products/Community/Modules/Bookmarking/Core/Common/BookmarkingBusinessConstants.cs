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
using ASC.Notify.Model;
using ASC.Web.Community.Product;
using Action = ASC.Common.Security.Authorizing.Action;

namespace ASC.Bookmarking.Common
{
    public static class BookmarkingBusinessConstants
    {
        public const string BookmarkingDbID = "community";

        public static Guid CommunityProductID = CommunityProduct.ID;

        #region Notify Action String Constants

        public const string BookmarkCreatedID = "new bookmark created";
        public const string BookmarkCommentCreatedID = "new bookmark comment created";

        #endregion

        public const string BookmarkingActionPattern = "ASC.Web.Community.Bookmarking.Core.Patterns.action_pattern.xml";


        public static INotifyAction NotifyActionNewBookmark = new NotifyAction(BookmarkCreatedID, "new-bookmark");
        internal static Guid NotifyActionNewBookmarkID = Guid.NewGuid();

        public static INotifyAction NotifyActionNewComment = new NotifyAction(BookmarkCommentCreatedID, "new-bookmark-comment");
        internal static Guid NotifyActionNewCommentID = Guid.NewGuid();

        public const string SubscriptionRecentBookmarkID = null;


        public static string TagURL = "URL";

        public static string TagUserName = "UserName";
        public static string TagUserURL = "UserURL";
        public static string TagDate = "Date";

        public static string TagPostPreview = "PostPreview";

        public static string TagCommentBody = "CommentBody";
        public static string TagCommentURL = "CommentURL";


        public const int MostPopularBookmarksByTagLimit = 3;

        public const string BookmarkingBasePath = "~/products/community/modules/bookmarking";

        #region Check Permissions

        /// <summary>
        /// base
        /// </summary>
        public static readonly Action BookmarkCreateAction = new Action(
            new Guid("{0D1F72A8-63DA-47ea-AE42-0900E4AC72A9}"),
            "Create bookmark"
            );

        /// <summary>
        /// base
        /// </summary>
        public static readonly Action BookmarkAddToFavouriteAction = new Action(
            new Guid("{FBC37705-A04C-40ad-A68C-CE2F0423F397}"),
            "Add to favorites"
            );

        /// <summary>
        /// base
        /// </summary>
        public static readonly Action BookmarkRemoveFromFavouriteAction = new Action(
            new Guid("{08D66144-E1C9-4065-9AA1-AA4BBA0A7BC8}"),
            "Remove from favorites"
            );

        /// <summary>
        /// base
        /// </summary>
        public static readonly Action BookmarkCreateCommentAction = new Action(
            new Guid("{A362FE79-684E-4d43-A599-65BC1F4E167F}"),
            "Add Comment"
            );

        /// <summary>
        /// base
        /// </summary>
        public static readonly Action BookmarkEditCommentAction = new Action(
            new Guid("{A18480A4-6D18-4c71-84FA-789888791F45}"),
            "Edit comment"
            );

        #endregion
    }
}