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
using ASC.Web.UserControls.Forum;
using ASC.Web.UserControls.Forum.Common;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.Forum
{
    public partial class NewPost : MainPage
    {
        private NewPostControl _newPostControl;

        protected void Page_Load(object sender, EventArgs e)
        {
            ForumManager.Instance.SetCurrentPage(ForumPage.NewPost);

            _newPostControl = LoadControl(ForumManager.Settings.UserControlsVirtualPath + "/NewPostControl.ascx") as NewPostControl;
            _newPostControl.SettingsID = ForumManager.Settings.ID;
            _newPostHolder.Controls.Add(_newPostControl);
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            var caption =
                (Master as ForumMasterPage).CurrentPageCaption =
                _newPostControl.PostType == NewPostType.Post
                    ? ForumResource.NewPostButton
                    : _newPostControl.PostType == NewPostType.Topic
                          ? ForumResource.NewTopicButton :
                          ForumResource.NewPollButton;

            (Master as ForumMasterPage).CurrentPageCaption = caption;
            Title = HeaderStringHelper.GetPageTitle(caption);
        }
    }
}