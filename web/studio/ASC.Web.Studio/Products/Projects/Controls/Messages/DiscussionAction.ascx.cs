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

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;
using System.Linq;

#endregion

namespace ASC.Web.Projects.Controls.Messages
{
    public partial class DiscussionAction : BaseUserControl
    {       
        protected bool IsMobile;

        public Project Project { get; set; }
        public Message Discussion { get; set; }
        public UserInfo Author { get; set; }
        public int ProjectFolderId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            IsMobile = Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context);

            //fix for IE 10
            var browser = HttpContext.Current.Request.Browser.Browser;

            var userAgent = Context.Request.Headers["User-Agent"];
            var regExp = new Regex("MSIE 10.0", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var regExpIe11 = new Regex("(?=.*Trident.*rv:11.0).+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (browser == "IE" && regExp.Match(userAgent).Success || regExpIe11.Match(userAgent).Success)
            {
                IsMobile = true;
            }
            if (IsMobile)
            {
                Page.RegisterBodyScripts(ResolveUrl("~/js/asc/core/decoder.js"));
                Page.RegisterInlineScript(@"jq(document).ready(function () {
                        var node = jq('<div>' + jq('[id$=discussionContent]').val() + '</div>').get(0);
                        jq('[id$=discussionContent]').val(ASC.Controls.HtmlHelper.HtmlNode2FormattedText(node));
                    });");
            }

            fckEditor.BasePath = VirtualPathUtility.ToAbsolute(CommonControlsConfigurer.FCKEditorBasePath);
            fckEditor.ToolbarSet = "BlogToolbar";
            fckEditor.EditorAreaCSS = WebSkin.BaseCSSFileAbsoluteWebPath;
            fckEditor.Visible = !IsMobile;
            var discussionParticipants = new List<Participant>();
            
            if (Discussion != null)
            {
                discussionTitle.Text = Discussion.Title;
                if (!IsMobile)
                {
                    fckEditor.Value = Discussion.Content;
                }
                else
                {
                    discussionContent.Text = Discussion.Content;
                }
                
                var recipients = Global.EngineFactory.GetMessageEngine().GetSubscribers(Discussion);
                var participantEngine = Global.EngineFactory.GetParticipantEngine();

                discussionParticipants.AddRange(recipients.Select(r => participantEngine.GetByID(new Guid(r.ID))));
                Author = CoreContext.UserManager.GetUsers(Discussion.CreateBy);
            }

            discussionParticipantRepeater.DataSource = discussionParticipants;
            discussionParticipantRepeater.DataBind();

            Author = CoreContext.UserManager.GetUsers(Page.Participant.ID);

            LoadDiscussionParticipantsSelector();

            //if (Project != null)
            //    LoadDiscussionFilesControl();
        }

        protected string GetPageTitle()
        {
            return Discussion == null ? MessageResource.CreateDiscussion : MessageResource.EditMessage;
        }

        //private void LoadDiscussionFilesControl()
        //{
        //    ProjectFolderId = (int)FileEngine2.GetRoot(Project.ID);

        //    var discussionFilesControl = 
        //        (Studio.UserControls.Common.Attachments.Attachments)LoadControl(Studio.UserControls.Common.Attachments.Attachments.Location);
        //    discussionFilesControl.EntityType = "message";
        //    discussionFilesControl.ModuleName = "projects";
        //    discussionFilesControl.ProjectId = Project.ID;
        //    discussionFilesPlaceHolder.Controls.Add(discussionFilesControl);
        //}

        private void LoadDiscussionParticipantsSelector()
        {
            var discussionParticipantsSelector = (Studio.UserControls.Users.UserSelector)LoadControl(Studio.UserControls.Users.UserSelector.Location);
            discussionParticipantsSelector.BehaviorID = "discussionParticipantsSelector";
            discussionParticipantsSelector.DisabledUsers.Add(new Guid());
            discussionParticipantsSelector.Title = MessageResource.DiscussionParticipants;
            discussionParticipantsSelector.SelectedUserListTitle = MessageResource.DiscussionParticipants;

            discussionParticipantsSelectorHolder.Controls.Add(discussionParticipantsSelector);
        }

        protected string GetDiscussionAction()
        {
            var innerHTML = new StringBuilder();
            var discussionId = Discussion == null ? -1 : Discussion.ID;
            var action = Discussion == null ? MessageResource.AddDiscussion : ProjectsCommonResource.SaveChanges;

            innerHTML.AppendFormat("<a id='discussionActionButton' class='button blue big' discussionId='{0}'>{1}</a>", 
                                    discussionId, action);
            innerHTML.AppendFormat(" <span class=\"splitter-buttons\"></span>");
            innerHTML.AppendFormat("<a id='discussionPreviewButton' class='button blue big' authorName='{0}' authorAvatarUrl='{1}' authorTitle='{2}' authorPageUrl='{3}'>{4}</a>",
                                   Author.DisplayUserName(), Author.GetBigPhotoURL(), Author.Title.HtmlEncode(), Author.GetUserProfilePageURL(), ProjectsCommonResource.Preview);
            innerHTML.AppendFormat(" <span class=\"splitter-buttons\"></span>");
            innerHTML.AppendFormat("<a id='discussionCancelButton' class='button gray big'>{0}</a>",
                                   ProjectsCommonResource.Cancel);

            return innerHTML.ToString();
        }

        protected bool CanReadDiscussion(Guid id)
        {
            return ProjectSecurity.CanRead(Discussion, id);
        }

        protected virtual string RenderRedirectUpload()
        {
            return string.Format("{0}://{1}:{2}{3}", Request.GetUrlRewriter().Scheme, Request.GetUrlRewriter().Host, Request.GetUrlRewriter().Port, VirtualPathUtility.ToAbsolute("~/") + "fckuploader.ashx?esid=discussion");
        }
    }
}
