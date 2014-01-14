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

using ASC.Web.Community.Bookmarking.Util;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;

namespace ASC.Web.Community.Bookmarking
{
    public partial class UserBookmarks : BookmarkingBasePage
    {
        protected override void PageLoad()
        {
            ServiceHelper.DisplayMode = BookmarkingServiceHelper.BookmarkDisplayMode.BookmarksCreatedByUser;

            var c = LoadControl(BookmarkUserControlPath.BookmarkingUserControlPath);

            BookmarkingPageContent.Controls.Add(c);

            var pageTitle = ServiceHelper.GetUserNameByRequestParam();

            InitBreadcrumbs(pageTitle);
            Title = HeaderStringHelper.GetPageTitle(pageTitle);
        }

        protected override void InitBreadcrumbs(string pageTitle)
        {
            Title = pageTitle;
        }
    }
}