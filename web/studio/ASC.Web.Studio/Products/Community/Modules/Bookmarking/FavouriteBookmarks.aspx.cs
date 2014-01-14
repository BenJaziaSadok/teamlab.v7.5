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
using ASC.Web.Community.Bookmarking.Util;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Resources;

namespace ASC.Web.Community.Bookmarking
{
    public partial class FavouriteBookmarks : BookmarkingBasePage
    {
        protected override void PageLoad()
        {
            ServiceHelper.DisplayMode = BookmarkingServiceHelper.BookmarkDisplayMode.Favourites;

            var c = LoadControl(BookmarkUserControlPath.BookmarkingUserControlPath);

            BookmarkingPageContent.Controls.Add(c);

            InitBreadcrumbs(BookmarkingUCResource.FavouritesNavigationItem);
            Title = HeaderStringHelper.GetPageTitle(BookmarkingUCResource.FavouritesNavigationItem);
        }

        protected override void InitBreadcrumbs(string pageTitle)
        {
            var searchText = ServiceHelper.GetSearchTag();
            if (!String.IsNullOrEmpty(searchText))
            {
                var searchResults = String.Format("{0} {1}", BookmarkingUCResource.TagBookmarks, searchText);

                Title = searchResults;
            }
        }
    }
}