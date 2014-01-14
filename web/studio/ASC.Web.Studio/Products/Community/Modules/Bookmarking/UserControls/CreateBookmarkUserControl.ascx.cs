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
using AjaxPro;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Resources;

namespace ASC.Web.UserControls.Bookmarking
{
    public partial class CreateBookmarkUserControl : System.Web.UI.UserControl
    {
        public bool IsNewBookmark { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof(BookmarkingUserControl));
            Utility.RegisterTypeForAjax(typeof(SingleBookmarkUserControl));
            Utility.RegisterTypeForAjax(typeof(CommentsUserControl));

            InitActionButtons();
        }

        #region Init Action Buttons

        private void InitActionButtons()
        {
            SaveBookmarkButton.ButtonText = BookmarkingUCResource.Save;
            SaveBookmarkButton.AjaxRequestText = BookmarkingUCResource.BookmarkCreationIsInProgressLabel;

            SaveBookmarkButtonCopy.ButtonText = BookmarkingUCResource.AddToFavourite;
            SaveBookmarkButtonCopy.AjaxRequestText = BookmarkingUCResource.BookmarkCreationIsInProgressLabel;

            AddToFavouritesBookmarkButton.ButtonText = BookmarkingUCResource.AddToFavourite;
            AddToFavouritesBookmarkButton.AjaxRequestText = BookmarkingUCResource.BookmarkCreationIsInProgressLabel;

            CheckBookmarkUrlLinkButton.ButtonText = BookmarkingUCResource.CheckBookmarkUrlButton;
            CheckBookmarkUrlLinkButton.AjaxRequestText = BookmarkingUCResource.CheckingUrlIsInProgressLabel;
        }

        #endregion

        public string NavigateToMainPage
        {
            get { return BookmarkingServiceHelper.BookmarkDisplayMode.CreateBookmark.Equals(BookmarkingServiceHelper.GetCurrentInstanse().DisplayMode).ToString().ToLower(); }
        }

        public bool CreateBookmarkMode
        {
            get { return BookmarkingServiceHelper.BookmarkDisplayMode.CreateBookmark.Equals(BookmarkingServiceHelper.GetCurrentInstanse().DisplayMode); }
        }

        public bool IsEditMode
        {
            get
            {
                var serviceHelper = BookmarkingServiceHelper.GetCurrentInstanse();
                return BookmarkingServiceHelper.BookmarkDisplayMode.SelectedBookmark.Equals(serviceHelper.DisplayMode) && serviceHelper.IsCurrentUserBookmark();
            }
        }
    }
}