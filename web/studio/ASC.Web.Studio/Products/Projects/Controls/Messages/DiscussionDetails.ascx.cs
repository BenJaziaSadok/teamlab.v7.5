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
using System.Globalization;
using System.Linq;
using System.Web;
using ASC.Web.Studio.Utility.HtmlUtility;
using AjaxPro;
using ASC.Core;
using ASC.Web.Core.Mobile;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Core.Users;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.UserControls.Common.Comments;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Projects.Controls.Messages
{
    [AjaxNamespace("AjaxPro.DiscussionDetails")]
    public partial class DiscussionDetails : BaseUserControl
    {
        public Message Discussion { get; set; }

        public Project Project { get; set; }

        public bool CanReadFiles { get; set; }

        public bool CanEditFiles { get; set; }

        public bool CanEdit { get; set; }

        public bool IsMobile { get; set; }

        public int FilesCount { get; private set; }

        public int ProjectFolderId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof(DiscussionDetails), Page);

            _hintPopup.Options.IsPopup = true;

            LoadCommentsControl();

            BindDiscussionParticipants();
            CanEdit = ProjectSecurity.CanEdit(Discussion);

            if (CanEdit)
                LoadDiscussionParticipantsSelector();

            CanReadFiles = ProjectSecurity.CanReadFiles(Discussion.Project);
            CanEditFiles = ProjectSecurity.IsInTeam(Project);
            IsMobile = MobileDetector.IsRequestMatchesMobile(Context);

            FilesCount = GetDiscussionFilesCount();

            if (CanReadFiles && (CanEditFiles || FilesCount > 0))
                LoadDiscussionFilesControl();
        }

        #region LoadControls

        private void BindDiscussionParticipants()
        {
            var participants = Global.EngineFactory.GetMessageEngine().GetSubscribers(Discussion).Select(r => new ParticipiantWrapper(r.ID, Discussion));

            discussionParticipantRepeater.DataSource = participants;
            discussionParticipantRepeater.DataBind();
        }

        private void LoadDiscussionParticipantsSelector()
        {
            var discussionParticipantsSelector = (Studio.UserControls.Users.UserSelector)LoadControl(Studio.UserControls.Users.UserSelector.Location);
            discussionParticipantsSelector.BehaviorID = "discussionParticipantsSelector";
            discussionParticipantsSelector.DisabledUsers.Add(new Guid());
            discussionParticipantsSelector.Title = MessageResource.DiscussionParticipants;
            discussionParticipantsSelector.SelectedUserListTitle = MessageResource.DiscussionParticipants;

            discussionParticipantsSelectorHolder.Controls.Add(discussionParticipantsSelector);
        }

        private void LoadDiscussionFilesControl()
        {
            ProjectFolderId = (int)FileEngine2.GetRoot(Project.ID);

            var discussionFilesControl = (Studio.UserControls.Common.Attachments.Attachments)LoadControl(Studio.UserControls.Common.Attachments.Attachments.Location);
            discussionFilesControl.EmptyScreenVisible = false;
            discussionFilesControl.EntityType = "message";
            discussionFilesControl.ModuleName = "projects";
            discussionFilesControl.CanAddFile = CanEditFiles;
            discussionFilesPlaceHolder.Controls.Add(discussionFilesControl);
        }

        #endregion

        #region Protected Fields

        protected string GetDiscussionTitle()
        {
            return Discussion.Title.HtmlEncode();
        }

        protected string GetUpdateDiscussionUrl()
        {
            return string.Format("messages.aspx?prjID={0}&id={1}&action=edit", Project.ID, Discussion.ID);
        }

        protected string GetNewDiscussionUrl()
        {
            return string.Format("messages.aspx?prjID={0}&action=add", Project.ID);
        }

        protected string GetDiscussionAuthorAvatar()
        {
            return Global.EngineFactory.GetParticipantEngine().GetByID(Discussion.CreateBy).UserInfo.GetBigPhotoURL();
        }

        protected string GetDiscussionAuthorUrl()
        {
            return CoreContext.UserManager.GetUsers(Discussion.CreateBy).GetUserProfilePageURL();
        }

        protected string GetDiscussionAuthor()
        {
            return CoreContext.UserManager.GetUsers(Discussion.CreateBy).DisplayUserName();
        }

        protected string GetDiscussionDate()
        {
            return Discussion.CreateOn.ToString("d");
        }

        protected string GetDiscussionTime()
        {
            return Discussion.CreateOn.ToString("t");
        }

        protected string GetDiscussionProjectUrl()
        {
            return string.Format("tasks.aspx?prjID={0}", Discussion.Project.ID);
        }

        protected string GetDiscussionProject()
        {
            return Discussion.Project.Title.HtmlEncode();
        }

        protected int GetDiscussionFilesCount()
        {
            var files = FileEngine2.GetMessageFiles(Discussion);
            return files.Count();
        }

        protected string GetDiscussionText()
        {
            return HtmlUtility.GetFull(Discussion.Content, ProductEntryPoint.ID);
        }

        #endregion

        #region Tabs

        protected string GetTabTitle(int count, string defaultTitle)
        {
            return count > 0 ? string.Format("{0} ({1})", defaultTitle, count) : defaultTitle;
        }

        #endregion

        #region ParticipiantWrapper

        protected class ParticipiantWrapper
        {
            public string ID { get; set; }
            public string Link { get; set; }
            public string FullUserName { get; set; }
            public string Title { get; set; }
            public string Department { get; set; }
            public bool CanRead { get; set; }

            public ParticipiantWrapper(string id, Message message)
            {
                ID = id;

                var participant = Global.EngineFactory.GetParticipantEngine().GetByID(new Guid(id));

                Link = participant.UserInfo.RenderProfileLink(ProductEntryPoint.ID);
                FullUserName = DisplayUserSettings.GetFullUserName(participant.UserInfo);
                Title = HttpUtility.HtmlEncode(participant.UserInfo.Title);
                Department = HttpUtility.HtmlEncode(participant.UserInfo.Department);
                CanRead = ProjectSecurity.CanRead(message, new Guid(id));
            }
        }

        #endregion

        #region Comments

        private void LoadCommentsControl()
        {
            discussionComments.Items = BuilderCommentInfo();
            ConfigureComments(discussionComments, Discussion);
        }

        private static void ConfigureComments(CommentsList commentList, Message messageToUpdate)
        {
            CommonControlsConfigurer.CommentsConfigure(commentList);

            var countMessageToUpdate = messageToUpdate != null ? Global.EngineFactory.GetCommentEngine().Count(messageToUpdate) : 0;

            commentList.IsShowAddCommentBtn = ProjectSecurity.CanCreateComment();
            commentList.CommentsCountTitle = countMessageToUpdate != 0 ? countMessageToUpdate.ToString(CultureInfo.InvariantCulture) : "0";
            commentList.ObjectID = messageToUpdate != null
                                       ? messageToUpdate.ID.ToString(CultureInfo.InvariantCulture) : "";

            commentList.Simple = false;
            commentList.BehaviorID = "commentsObj";
            commentList.JavaScriptAddCommentFunctionName = "AjaxPro.DiscussionDetails.AddComment";
            commentList.JavaScriptLoadBBcodeCommentFunctionName = "AjaxPro.DiscussionDetails.LoadCommentBBCode";
            commentList.JavaScriptPreviewCommentFunctionName = "AjaxPro.DiscussionDetails.GetPreview";
            commentList.JavaScriptRemoveCommentFunctionName = "AjaxPro.DiscussionDetails.RemoveComment";
            commentList.JavaScriptUpdateCommentFunctionName = "AjaxPro.DiscussionDetails.UpdateComment";
            commentList.OnRemovedCommentJS = "ASC.Projects.DiscussionDetails.removeComment";
            commentList.FckDomainName = "projects_comments";
            commentList.TotalCount = countMessageToUpdate;
        }

        [AjaxMethod]
        public AjaxResponse AddComment(string parrentCommentID, string messageID, string text, string pid)
        {
            ProjectSecurity.DemandCreateComment();

            var resp = new AjaxResponse();

            var comment = new Comment
                {
                    Content = text,
                    TargetUniqID = ProjectEntity.BuildUniqId<Message>(Convert.ToInt32(messageID))
                };

            resp.rs1 = parrentCommentID;

            if (!String.IsNullOrEmpty(parrentCommentID))
                comment.Parent = new Guid(parrentCommentID);

            var messageEngine = Global.EngineFactory.GetMessageEngine();
            Discussion = messageEngine.GetByID(Convert.ToInt32(messageID));

            if (Discussion == null) return new AjaxResponse { status = "error", message = "Access denied." };

            messageEngine.SaveOrUpdateComment(Discussion, comment);
            resp.rs2 = GetHTMLComment(comment);
            return resp;
        }

        private string GetHTMLComment(Comment comment)
        {
            var creator = Global.EngineFactory.GetParticipantEngine().GetByID(comment.CreateBy);
            var commentInfo = new CommentInfo
                {
                    TimeStamp = comment.CreateOn,
                    TimeStampStr = comment.CreateOn.Ago(),
                    CommentBody = comment.Content,
                    CommentID = comment.ID.ToString(),
                    UserID = comment.CreateBy,
                    UserFullName = creator.UserInfo.DisplayUserName(),
                    Inactive = comment.Inactive,
                    IsEditPermissions = ProjectSecurity.CanEditComment(Discussion != null ? Discussion.Project : null, comment),
                    IsResponsePermissions = ProjectSecurity.CanCreateComment(),
                    IsRead = true,
                    UserAvatar = Global.GetHTMLUserAvatar(creator.UserInfo),
                    UserPost = creator.UserInfo.Title
                };

            if (discussionComments == null)
            {
                discussionComments = new CommentsList();
                ConfigureComments(discussionComments, null);
            }

            return CommentsHelper.GetOneCommentHtmlWithContainer(
                discussionComments,
                commentInfo,
                comment.Parent == Guid.Empty,
                false);
        }

        [AjaxMethod]
        public AjaxResponse UpdateComment(string commentID, string text, string pid)
        {
            var messageEngine = Global.EngineFactory.GetMessageEngine();
            var resp = new AjaxResponse { rs1 = commentID };

            var comment = Global.EngineFactory.GetCommentEngine().GetByID(new Guid(commentID));

            comment.Content = text;

            var targetID = Convert.ToInt32(comment.TargetUniqID.Split('_')[1]);
            var target = messageEngine.GetByID(targetID);

            if (target == null) return new AjaxResponse { status = "error", message = "Access denied." };

            var targetProject = target.Project;

            ProjectSecurity.DemandEditComment(targetProject, comment);

            messageEngine.SaveOrUpdateComment(target, comment);

            resp.rs2 = text + CodeHighlighter.GetJavaScriptLiveHighlight(true);

            return resp;
        }

        [AjaxMethod]
        public string RemoveComment(string commentID, string pid)
        {
            var commentEngine = Global.EngineFactory.GetCommentEngine();
            var comment = commentEngine.GetByID(new Guid(commentID));

            comment.Inactive = true;

            var targetID = Convert.ToInt32(comment.TargetUniqID.Split('_')[1]);
            var target = Global.EngineFactory.GetMessageEngine().GetByID(targetID);
            var targetProject = target.Project;

            ProjectSecurity.DemandEditComment(targetProject, comment);
            ProjectSecurity.DemandRead(target);

            commentEngine.SaveOrUpdate(comment);

            return commentID;
        }

        [AjaxMethod]
        public string GetPreview(string text, string commentID)
        {
            ProjectSecurity.DemandAuthentication();

            return GetHTMLComment(text, commentID);
        }

        [AjaxMethod]
        public string LoadCommentBBCode(string commentId)
        {
            ProjectSecurity.DemandAuthentication();

            var comment = Global.EngineFactory.GetCommentEngine().GetByID(new Guid(commentId));
            return comment != null ? comment.Content : String.Empty;
        }


        private string GetHTMLComment(Comment comment, bool isPreview)
        {
            var creator = Global.EngineFactory.GetParticipantEngine().GetByID(comment.CreateBy);
            var commentInfo = new CommentInfo
                {
                    CommentID = comment.ID.ToString(),
                    UserID = comment.CreateBy,
                    TimeStamp = comment.CreateOn,
                    TimeStampStr = comment.CreateOn.Ago(),
                    UserPost = creator.UserInfo.Title,
                    IsRead = true,
                    Inactive = comment.Inactive,
                    CommentBody = comment.Content,
                    UserFullName = DisplayUserSettings.GetFullUserName(creator.UserInfo),
                    UserAvatar = Global.GetHTMLUserAvatar(creator.UserInfo)
                };

            var defComment = new CommentsList();
            ConfigureComments(defComment, null);

            if (!isPreview)
            {
                commentInfo.IsEditPermissions = ProjectSecurity.CanEditComment(Discussion.Project, comment);
                commentInfo.IsResponsePermissions = ProjectSecurity.CanCreateComment();
                commentInfo.IsRead = true;
            }

            return CommentsHelper.GetOneCommentHtmlWithContainer(
                defComment,
                commentInfo,
                comment.Parent == Guid.Empty,
                false);
        }

        private string GetHTMLComment(string text, string commentId)
        {
            Comment comment;
            if (!String.IsNullOrEmpty(commentId))
            {
                comment = Global.EngineFactory.GetCommentEngine().GetByID(new Guid(commentId));
                comment.Content = text;
            }
            else
            {
                comment = new Comment
                    {
                        Content = text,
                        CreateOn = TenantUtil.DateTimeNow(),
                        CreateBy = SecurityContext.CurrentAccount.ID
                    };
            }
            return GetHTMLComment(comment, true);
        }

        private IList<CommentInfo> BuilderCommentInfo()
        {
            var comments = Global.EngineFactory.GetCommentEngine().GetComments(Discussion);
            comments.Sort((x, y) => DateTime.Compare(x.CreateOn, y.CreateOn));

            return (from comment in comments where comment.Parent == Guid.Empty select GetCommentInfo(comments, comment)).ToList();
        }

        private CommentInfo GetCommentInfo(IEnumerable<Comment> allComments, Comment parent)
        {
            var creator = Global.EngineFactory.GetParticipantEngine().GetByID(parent.CreateBy).UserInfo;
            var commentInfo = new CommentInfo
                {
                    TimeStampStr = parent.CreateOn.Ago(),
                    IsRead = true,
                    Inactive = parent.Inactive,
                    IsResponsePermissions = ProjectSecurity.CanCreateComment(),
                    IsEditPermissions = ProjectSecurity.CanEditComment(Discussion.Project, parent),
                    CommentID = parent.ID.ToString(),
                    CommentBody = parent.Content,
                    UserID = parent.CreateBy,
                    UserFullName = creator.DisplayUserName(),
                    UserPost = creator.Title,
                    UserAvatar = Global.GetHTMLUserAvatar(creator),
                    CommentList = new List<CommentInfo>(),
                };

            if (allComments != null)
            {
                foreach (var comment in allComments.Where(comment => comment.Parent == parent.ID))
                {
                    commentInfo.CommentList.Add(GetCommentInfo(allComments, comment));
                }
            }
            return commentInfo;
        }

        #endregion
    }
}