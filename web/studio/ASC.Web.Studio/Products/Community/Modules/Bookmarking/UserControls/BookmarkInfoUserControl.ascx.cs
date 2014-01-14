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
using ASC.Web.Studio.UserControls.Common.ViewSwitcher;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Resources;

namespace ASC.Web.UserControls.Bookmarking
{
    public partial class BookmarkInfoUserControl : BookmarkInfoBase
    {

        public override void InitUserControl()
        {
            var singleBookmarkUserControl = LoadControl(BookmarkUserControlPath.SingleBookmarkUserControlPath) as BookmarkInfoBase;
            singleBookmarkUserControl.Bookmark = Bookmark;
            singleBookmarkUserControl.UserBookmark = UserBookmark;
            BookmarkInfoHolder.Controls.Add(singleBookmarkUserControl);

            var sortControl = new ViewSwitcher();
            sortControl.TabItems.Add(new ViewSwitcherTabItem
                {
                    TabName = BookmarkingUCResource.BookmarkedBy,
                    DivID = "BookmarkedByPanel",
                    IsSelected = ServiceHelper.SelectedTab == 1,
                    SkipRender = true
                });

            sortControl.TabItems.Add(new ViewSwitcherTabItem
                {
                    TabName = BookmarkingUCResource.Comments + String.Format(" ({0})", CommentsCount),
                    DivID = "BookmarkCommentsPanel",
                    IsSelected = ServiceHelper.SelectedTab == 0,
                    SkipRender = true
                });

            BookmarkInfoTabsContainer.Controls.Add(sortControl);

            //Init comments
            using (var c = LoadControl(BookmarkUserControlPath.CommentsUserControlPath) as CommentsUserControl)
            {
                c.BookmarkID = Bookmark.ID;
                c.BookmarkComments = ServiceHelper.GetBookmarkComments(Bookmark);
                c.InitComments();
                CommentsHolder.Controls.Add(c);
            }
            if (Bookmark != null)
            {
                var userBookmarks = Bookmark.UserBookmarks;
                if (userBookmarks != null && userBookmarks.Count > 0)
                {
                    //Init added by list
                    AddedByRepeater.DataSource = userBookmarks;
                    AddedByRepeater.DataBind();
                }
            }
        }

        public string GetAddedByTableItem(bool TintFlag, string UserImage, string UserPageLink, string UserBookmarkDescription, string DateAddedAsString, object userID)
        {
            return new BookmarkAddedByUserContorl().GetAddedByTableItem(TintFlag, UserImage, UserPageLink, UserBookmarkDescription, DateAddedAsString, userID);
        }
    }
}