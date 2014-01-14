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
using ASC.Forum;
using ASC.Web.Community.Forum.Resources;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum.Common;

namespace ASC.Web.Community.Forum
{
    public partial class Default : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ForumManager.Instance.SetCurrentPage(ForumPage.Default);

            List<ThreadCategory> categories;
            List<Thread> threads;

            ForumDataProvider.GetThreadCategories(TenantProvider.CurrentTenantID, true, out categories, out threads);

            if (0 < categories.Count)
            {
                var categoryListControl = LoadControl(ForumManager.Settings.UserControlsVirtualPath + "/ThreadCategoryListControl.ascx") as UserControls.Forum.ThreadCategoryListControl;
                categoryListControl.Categories = categories;
                categoryListControl.Threads = threads;
                categoryListControl.SettingsID = ForumManager.Settings.ID;
                forumListHolder.Controls.Add(categoryListControl);

                (Master as ForumMasterPage).CurrentPageCaption = ForumResource.ForumsBreadCrumbs;
            }
            else
            {
                _headerHolder.Visible = false;

                var emptyScreenControl = new EmptyScreenControl
                    {
                        ImgSrc = WebImageSupplier.GetAbsoluteWebPath("forums_icon.png", ForumManager.Settings.ModuleID),
                        Header = ForumResource.EmptyScreenForumCaption,
                        Describe = ForumResource.EmptyScreenForumText,
                        ButtonHTML = ForumManager.Instance.ValidateAccessSecurityAction(ForumAction.GetAccessForumEditor, null) ? String.Format("<a class='link underline blue plus' href='NewForum.aspx'>{0}</a>", ForumResource.EmptyScreenForumLink) : String.Empty
                    };
                forumListHolder.Controls.Add(emptyScreenControl);
            }

            Title = HeaderStringHelper.GetPageTitle((Master as ForumMasterPage).CurrentPageCaption ?? ForumResource.AddonName);
        }
    }
}