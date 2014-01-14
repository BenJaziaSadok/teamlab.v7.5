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
using System.Web;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Controls;
using ASC.Web.Projects.Controls.Messages;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.Projects
{
    public partial class Messages : BasePage
    {
        protected override string CookieKeyForPagination
        {
            get
            {
                return "discussionsKeyForPagination";
            }
        }

        private int entryCountOnPage;
        protected override int EntryCountOnPage
        {
            get
            {
                if (entryCountOnPage == 0) entryCountOnPage = 10;

                return entryCountOnPage;
            }
            set { entryCountOnPage = value; }
        }

        protected Message Discussion { get; set; }

        protected bool CanCreate { get; set; }

        protected override void PageLoad()
        {
            InitScripts();

            var messageEngine = Global.EngineFactory.GetMessageEngine();

            if (RequestContext.IsInConcreteProject && !ProjectSecurity.CanReadMessages(Project))
            {
                Response.Redirect("projects.aspx?prjID=" + Project.ID, true);
            }

            CanCreate = RequestContext.CanCreateDiscussion(true);

            int discussionId;
            if (int.TryParse(UrlParameters.EntityID, out discussionId))
            {
                if (Project == null) return;

                Discussion = messageEngine.GetByID(discussionId);

                if (string.Compare(UrlParameters.ActionType, "edit", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (ProjectSecurity.CanEdit(Discussion))
                    {
                        LoadDiscussionActionControl(Project, Discussion);
                    }
                    else
                    {
                        Response.Redirect("messages.aspx", true);
                    }

                    Title = HeaderStringHelper.GetPageTitle(Discussion.Title);
                }
                else if (Discussion != null && ProjectSecurity.CanRead(Discussion.Project) && Discussion.Project.ID == Project.ID)
                {
                    LoadDiscussionDetailsControl(Project, Discussion);

                    Master.EssenceTitle = Discussion.Title;

                    Title = HeaderStringHelper.GetPageTitle(Discussion.Title);

                    Master.IsSubcribed = messageEngine.IsSubscribed(Discussion);
                }
                else
                {
                    LoadElementNotFoundControl(Project.ID);

                    Title = HeaderStringHelper.GetPageTitle(MessageResource.MessageNotFound_Header);
                }

            }
            else
            {
                if (string.Compare(UrlParameters.ActionType, "add", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (CanCreate)
                    {
                        LoadDiscussionActionControl(Project, null);

                        Title = HeaderStringHelper.GetPageTitle(MessageResource.CreateMessage);
                    }
                    else
                    {
                        Response.Redirect("messages.aspx", true);
                    }
                }
                else
                {
                    var filter = new TaskFilter();

                    if (RequestContext.IsInConcreteProject)
                        filter.ProjectIds.Add(RequestContext.GetCurrentProjectId());

                    var allDiscCount = messageEngine.GetByFilterCount(filter);

                    if (allDiscCount > 0)
                    {
                        LoadDiscussionsListControl(Project == null ? -1 : Project.ID, allDiscCount);
                    }

                    var emptyScreenControl = new Studio.Controls.Common.EmptyScreenControl
                    {
                        ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_discussions.png", ProductEntryPoint.ID),
                        Header = MessageResource.DiscussionNotFound_Header,
                        Describe = MessageResource.DiscussionNotFound_Describe,
                        ID = "emptyListDiscussion"
                    };

                    if (CanCreate)
                    {
                        emptyScreenControl.ButtonHTML = RequestContext.IsInConcreteProject
                            ? String.Format("<a href='messages.aspx?prjID={0}&action=add' class='baseLinkAction addFirstElement'>{1}</a>", Project.ID, MessageResource.StartFirstDiscussion)
                            : String.Format("<a href='messages.aspx?action=add' class='baseLinkAction addFirstElement'>{0}</a>", MessageResource.StartFirstDiscussion);
                    }

                    contentHolder.Controls.Add(emptyScreenControl);

                    Title = HeaderStringHelper.GetPageTitle(MessageResource.Messages);
                }
            }
        }

        private void InitScripts()
        {
            Page.RegisterInlineScript(@"
                    var action = jq.getURLParam('action');
                    var id = jq.getURLParam('id');
                    if (action) {
                        ASC.Projects.DiscussionAction.init();
                    }
                    if (id && action == null) {
                        ASC.Projects.DiscussionDetails.init();
                    }
                    if (id == null && action == null) {
                        ASC.Projects.Discussions.init(" + EntryCountOnPage + ",'" + CookieKeyForPagination + "', " + Global.VisiblePageCount + ");}", true);
        }

        private void LoadDiscussionDetailsControl(Project project, Message discussion)
        {
            var discussionDetails = (DiscussionDetails)LoadControl(PathProvider.GetControlVirtualPath("DiscussionDetails.ascx"));
            discussionDetails.Discussion = discussion;
            discussionDetails.Project = project;
            contentHolder.Controls.Add(discussionDetails);
        }

        private void LoadDiscussionsListControl(int projectId, int allDiscCount)
        {
            var discussionsList = (DiscussionsList)LoadControl(PathProvider.GetControlVirtualPath("DiscussionsList.ascx"));
            discussionsList.ProjectId = projectId;
            discussionsList.AllDiscCount = allDiscCount;
            contentHolder.Controls.Add(discussionsList);
        }

        private void LoadDiscussionActionControl(Project project, Message discussion)
        {
            var discussionAction = (DiscussionAction)LoadControl(PathProvider.GetControlVirtualPath("DiscussionAction.ascx"));
            discussionAction.Project = project;
            discussionAction.Discussion = discussion;
            contentHolder.Controls.Add(discussionAction);

            Master.DisabledPrjNavPanel = true;
        }

        private void LoadElementNotFoundControl(int projectId)
        {
            contentHolder.Controls.Add(new ElementNotFoundControl
            {
                Header = MessageResource.MessageNotFound_Header,
                Body = MessageResource.MessageNotFound_Body,
                RedirectURL = String.Format("messages.aspx?prjID={0}", projectId),
                RedirectTitle = MessageResource.MessageNotFound_RedirectTitle
            });
        }
    }
}
