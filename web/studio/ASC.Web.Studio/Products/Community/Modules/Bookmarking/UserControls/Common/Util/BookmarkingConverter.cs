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
using ASC.Bookmarking.Business.Permissions;
using ASC.Bookmarking.Pojo;
using ASC.Web.Core.Users;
using ASC.Web.Studio.UserControls.Common.Comments;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;

namespace ASC.Web.UserControls.Bookmarking.Common.Util
{
    public static class BookmarkingConverter
    {
        public static IList<CommentInfo> ConvertCommentList(IList<Comment> commentList)
        {
            var result = new List<CommentInfo>();
            foreach (var comment in commentList)
            {
                var parentID = Guid.Empty;
                try
                {
                    parentID = new Guid(comment.Parent);
                }
                catch
                {
                }
                if (Guid.Empty.Equals(parentID))
                {
                    var c = ConvertComment(comment, commentList);
                    result.Add(c);
                }
            }
            return result;
        }

        public static CommentInfo ConvertComment(Comment comment, IList<Comment> commentList)
        {
            var userID = comment.UserID;

            CommentInfo c = new CommentInfo
                {
                    CommentID = comment.ID.ToString(),
                    UserID = userID,
                    TimeStamp = comment.Datetime,
                    TimeStampStr = comment.Datetime.Ago(),
                    Inactive = comment.Inactive,
                    CommentBody = comment.Content,
                    UserFullName = DisplayUserSettings.GetFullUserName(userID),
                    UserAvatar = BookmarkingServiceHelper.GetHTMLUserAvatar(userID),
                    IsEditPermissions = BookmarkingPermissionsCheck.PermissionCheckEditComment(comment),
                    IsResponsePermissions = BookmarkingPermissionsCheck.PermissionCheckCreateComment(),
                    UserPost = BookmarkingServiceHelper.GetUserInfo(userID).Title
                };

            var commentsList = new List<CommentInfo>();

            var childComments = GetChildComments(comment, commentList);
            if (childComments != null)
            {
                foreach (var item in childComments)
                {
                    commentsList.Add(ConvertComment(item, commentList));
                }
            }
            c.CommentList = commentsList;
            return c;
        }

        private static IList<Comment> GetChildComments(Comment c, IList<Comment> comments)
        {
            var commentID = c.ID.ToString();
            return comments.Where(comment => commentID.Equals(comment.Parent)).ToList();
        }

        public static string GetDateAsString(DateTime date)
        {
            try
            {
                return date.ToShortTimeString() + "&nbsp;&nbsp;&nbsp;" + date.ToShortDateString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}