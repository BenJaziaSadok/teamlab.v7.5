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
using ASC.Web.Community.Forum.Resources;
using ASC.Web.Studio;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum;
using ASC.Web.UserControls.Forum.Common;

namespace ASC.Web.Community.Forum
{
    public partial class EditTopic : MainPage
    {
        private TopicEditorControl _topicEditorControl;

        protected void Page_Load(object sender, EventArgs e)
        {
            ForumManager.Instance.SetCurrentPage(ForumPage.EditTopic);

            _topicEditorControl = LoadControl(ForumManager.Settings.UserControlsVirtualPath + "/TopicEditorControl.ascx") as TopicEditorControl;
            _topicEditorControl.SettingsID = ForumManager.Settings.ID;
            topicEditorHolder.Controls.Add(_topicEditorControl);
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            (Master as ForumMasterPage).CurrentPageCaption = ForumResource.EditTopicBreadCrumbs;
            Title = HeaderStringHelper.GetPageTitle(ForumResource.EditTopicBreadCrumbs);
        }
    }
}