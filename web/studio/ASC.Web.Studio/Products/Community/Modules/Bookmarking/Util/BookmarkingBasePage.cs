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
using System.Web;
using ASC.Web.Studio;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.Community.Product;
using ASC.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Resources;

namespace ASC.Web.Community.Bookmarking.Util
{
    public abstract class BookmarkingBasePage : MainPage
    {
        protected BookmarkingServiceHelper ServiceHelper;

        /// <summary>
        /// Page_Load of the Page Controller pattern.
        /// See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnpatterns/html/ImpPageController.asp
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            BookmarkingBusinessConstants.CommunityProductID = CommunityProduct.ID;

            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/products/community/modules/bookmarking/app_themes/default/css/bookmarkingstyle.css"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/products/community/modules/bookmarking/js/bookmarking.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/js/asc/plugins/tagsautocompletebox.js"));

            ServiceHelper = BookmarkingServiceHelper.GetCurrentInstanse();

            PageLoad();
        }

        protected abstract void PageLoad();

        protected virtual void InitBreadcrumbs(string pageTitle)
        {
            var searchText = ServiceHelper.GetSearchText();
            if (!String.IsNullOrEmpty(searchText))
            {
                ServiceHelper.DisplayMode = BookmarkingServiceHelper.BookmarkDisplayMode.SearchBookmarks;
                return;
            }

            searchText = ServiceHelper.GetSearchTag();
            if (!String.IsNullOrEmpty(searchText))
            {
                ServiceHelper.DisplayMode = BookmarkingServiceHelper.BookmarkDisplayMode.SearchByTag;
                var searchResults = String.Format("{0} {1}", BookmarkingUCResource.TagBookmarks, searchText);

                Title = searchResults;
            }
        }
    }
}