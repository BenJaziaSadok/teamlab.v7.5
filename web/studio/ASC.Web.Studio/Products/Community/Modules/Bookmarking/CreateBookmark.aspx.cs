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

using System.Web;
using ASC.Bookmarking.Business.Permissions;
using ASC.Web.Community.Bookmarking.Util;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Bookmarking;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Resources;

namespace ASC.Web.Community.Bookmarking
{
    public partial class CreateBookmark : BookmarkingBasePage
    {
        protected override void PageLoad()
        {
            if (!BookmarkingPermissionsCheck.PermissionCheckCreateBookmark())
            {
                Response.Redirect(BookmarkingRequestConstants.BookmarkingPageName);
            }

            ServiceHelper.DisplayMode = BookmarkingServiceHelper.BookmarkDisplayMode.CreateBookmark;

            var c = LoadControl(BookmarkUserControlPath.CreateBookmarkUserControlPath) as CreateBookmarkUserControl;
            c.IsNewBookmark = true;
            BookmarkingPageContent.Controls.Add(c);

            var url = Request.QueryString[BookmarkingRequestConstants.UrlGetRequest];
            var s = string.Empty;
            if (!string.IsNullOrEmpty(url))
            {
                s = string.Format(" getBookmarkUrlInput().val(\"{0}\"); getBookmarkByUrlButtonClick(); ", url);
            }

            var script = string.Format("showAddBookmarkPanel(); {0}", s);

            Page.RegisterInlineScript(script);

            InitBreadcrumbs(BookmarkingUCResource.AddBookmarkLink);
            Title = HeaderStringHelper.GetPageTitle(BookmarkingUCResource.AddBookmarkLink);
        }

        protected override void InitBreadcrumbs(string pageTitle)
        {
            Title = pageTitle;
        }
    }
}