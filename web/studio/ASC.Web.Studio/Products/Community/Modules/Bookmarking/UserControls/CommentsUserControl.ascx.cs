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
using AjaxPro;
using ASC.Bookmarking.Business.Permissions;
using ASC.Bookmarking.Pojo;
using ASC.Core;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.UserControls.Common.Comments;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Common.Util;

namespace ASC.Web.UserControls.Bookmarking
{
    [AjaxNamespace("CommentsUserControl")]
    public partial class CommentsUserControl : System.Web.UI.UserControl
    {
        #region Fields

        private readonly BookmarkingServiceHelper _serviceHelper = BookmarkingServiceHelper.GetCurrentInstanse();

        public long BookmarkID { get; set; }

        #region Bookmark Comments

        private IList<Comment> _bookmarkComments;

        public IList<Comment> BookmarkComments
        {
            get { return _bookmarkComments ?? new List<Comment>(); }
            set { _bookmarkComments = value; }
        }

        #endregion

        public CommentsList Comments
        {
            get
            {
                CommentList = _serviceHelper.Comments;
                return _serviceHelper.Comments;
            }
            set { _serviceHelper.Comments = CommentList; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            InitComments();
        }

        public void InitComments()
        {
            ConfigureComments(CommentList);
            CommentList.Items = BookmarkingConverter.ConvertCommentList(BookmarkComments);
            Comments = CommentList;
        }

        private void ConfigureComments(CommentsList commentList)
        {
            CommonControlsConfigurer.CommentsConfigure(commentList);

            // add comments permission check		
            commentList.IsShowAddCommentBtn = BookmarkingPermissionsCheck.PermissionCheckCreateComment();
            commentList.CommentsCountTitle = BookmarkComments.Count.ToString();
            commentList.Simple = false;
            commentList.BehaviorID = "commentsObj";
            commentList.JavaScriptAddCommentFunctionName = "CommentsUserControl.AddComment";
            commentList.JavaScriptLoadBBcodeCommentFunctionName = "CommentsUserControl.LoadCommentBBCode";
            commentList.JavaScriptPreviewCommentFunctionName = "CommentsUserControl.GetPreview";
            commentList.JavaScriptRemoveCommentFunctionName = "CommentsUserControl.RemoveComment";
            commentList.JavaScriptUpdateCommentFunctionName = "CommentsUserControl.UpdateComment";
            commentList.FckDomainName = "bookmarking_comments";
            commentList.TotalCount = BookmarkComments.Count;
            commentList.ShowCaption = false;
            commentList.ObjectID = BookmarkID.ToString();
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse AddComment(string parrentCommentID, long bookmarkID, string text, string pid)
        {
            var resp = new AjaxResponse { rs1 = parrentCommentID };

            var comment = new Comment
                {
                    Content = text,
                    Datetime = ASC.Core.Tenants.TenantUtil.DateTimeNow(),
                    UserID = SecurityContext.CurrentAccount.ID
                };

            var parentID = Guid.Empty;
            try
            {
                if (!string.IsNullOrEmpty(parrentCommentID))
                {
                    parentID = new Guid(parrentCommentID);
                }
            }
            catch
            {
                parentID = Guid.Empty;
            }
            comment.Parent = parentID.ToString();
            comment.BookmarkID = bookmarkID;
            comment.ID = Guid.NewGuid();

            _serviceHelper.AddComment(comment);

            resp.rs2 = GetOneCommentHtmlWithContainer(comment);

            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string LoadCommentBBCode(string commentID)
        {
            if (string.IsNullOrEmpty(commentID))
                return string.Empty;

            var comment = _serviceHelper.GetCommentById(commentID);
            return comment.Content;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string GetPreview(string text, string commentID)
        {
            return GetHTMLComment(text, commentID);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string RemoveComment(string commentID, string pid)
        {
            var comment = _serviceHelper.GetCommentById(commentID);
            if (comment != null && BookmarkingPermissionsCheck.PermissionCheckEditComment(comment))
            {
                _serviceHelper.RemoveComment(commentID);
                return commentID;
            }
            return null;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse UpdateComment(string commentID, string text, string pid)
        {
            var resp = new AjaxResponse();
            var comment = _serviceHelper.GetCommentById(commentID);
            if (comment != null)
            {
                if (BookmarkingPermissionsCheck.PermissionCheckEditComment(comment))
                {
                    _serviceHelper.UpdateComment(commentID, text);
                    resp.rs1 = commentID;
                    resp.rs2 = text + CodeHighlighter.GetJavaScriptLiveHighlight(true);
                }
            }
            return resp;
        }

        private string GetHTMLComment(string text, string commentID)
        {
            var comment = new Comment
                {
                    Content = text,
                    Datetime = ASC.Core.Tenants.TenantUtil.DateTimeNow(),
                    UserID = SecurityContext.CurrentAccount.ID
                };

            if (!String.IsNullOrEmpty(commentID))
            {
                comment = _serviceHelper.GetCommentById(commentID);

                comment.Content = text;
                comment.Parent = string.Empty;
            }

            var defComment = new CommentsList();

            ConfigureComments(defComment);

            var ci = BookmarkingConverter.ConvertComment(comment, BookmarkingServiceHelper.GetCurrentInstanse().BookmarkToAdd.Comments);
            ci.IsEditPermissions = false;
            ci.IsResponsePermissions = false;

            var isRoot = string.IsNullOrEmpty(comment.Parent) || comment.Parent.Equals(Guid.Empty.ToString(), StringComparison.CurrentCultureIgnoreCase);

            return CommentsHelper.GetOneCommentHtmlWithContainer(defComment, ci, isRoot, false);
        }

        private string GetOneCommentHtmlWithContainer(Comment comment)
        {
            return CommentsHelper.GetOneCommentHtmlWithContainer(
                Comments,
                BookmarkingConverter.ConvertComment(comment, BookmarkingServiceHelper.GetCurrentInstanse().BookmarkToAdd.Comments),
                comment.Parent.Equals(Guid.Empty.ToString(), StringComparison.CurrentCultureIgnoreCase),
                false);
        }
    }
}