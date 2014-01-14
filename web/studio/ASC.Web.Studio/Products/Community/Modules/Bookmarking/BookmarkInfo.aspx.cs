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

using System.Collections.Generic;
using ASC.Bookmarking.Pojo;
using ASC.Web.Community.Bookmarking.Util;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Bookmarking;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.Community.Bookmarking.UserControls;
using ASC.Web.UserControls.Bookmarking.Resources;

namespace ASC.Web.Community.Bookmarking
{
    public partial class BookmarkInfo : BookmarkingBasePage
    {
        protected string BookmarkTitle { get; set; }

        protected override void PageLoad()
        {
            ServiceHelper.DisplayMode = BookmarkingServiceHelper.BookmarkDisplayMode.SelectedBookmark;

            var c = LoadControl(BookmarkUserControlPath.BookmarkInfoUserControlPath) as BookmarkInfoUserControl;
            InitBookmarkInfoUserControl(c);

            var pageTitle = BookmarkingUCResource.BookmarksNavigationItem;

            var bookmarks = new List<Bookmark> { c.Bookmark };

            var bookmarkingUserControl = LoadControl(BookmarkUserControlPath.BookmarkingUserControlPath) as BookmarkingUserControl;
            bookmarkingUserControl.Bookmarks = bookmarks;

            var b = LoadControl(BookmarkUserControlPath.BookmarkHeaderPageControlPath) as BookmarkHeaderPageControl;
            b.Title = ServiceHelper.BookmarkToAdd.Name;
            BookmarkingPageContent.Controls.Add(b);

            BookmarkingPageContent.Controls.Add(bookmarkingUserControl);
            BookmarkingPageContent.Controls.Add(c);


            InitBreadcrumbs(pageTitle);
            Title = HeaderStringHelper.GetPageTitle(pageTitle);
        }

        #region Init Bookmark

        private void InitBookmarkInfoUserControl(BookmarkInfoUserControl c)
        {
            var b = ServiceHelper.GetBookmarkWithUserBookmarks();

            if (b == null)
            {
                var url = Request.QueryString[BookmarkingRequestConstants.UrlGetRequest];

                b = ServiceHelper.GetBookmarkWithUserBookmarks(url, false) ?? ServiceHelper.GetBookmarkWithUserBookmarks(url, true);

                if (b == null)
                {

                    var redirectUrl = BookmarkingRequestConstants.CreateBookmarkPageName;
                    if (!string.IsNullOrEmpty(url))
                    {
                        url = BookmarkingServiceHelper.UpdateBookmarkInfoUrl(url);
                        redirectUrl += string.Format("?{0}={1}", BookmarkingRequestConstants.UrlGetRequest, url);
                    }

                    Response.Redirect(redirectUrl);
                }
            }
            c.Bookmark = b;
            c.UserBookmark = ServiceHelper.GetCurrentUserBookmark(b.UserBookmarks);
        }

        #endregion

        protected override void InitBreadcrumbs(string pageTitle)
        {
            //Get text from the search input
            var bookmarkName = string.Empty;
            if (ServiceHelper.BookmarkToAdd != null)
            {
                bookmarkName = ServiceHelper.BookmarkToAdd.Name;
            }
            BookmarkTitle = bookmarkName;
        }
    }
}