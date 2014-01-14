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
using ASC.Web.UserControls.Bookmarking.Resources;

namespace ASC.Web.UserControls.Bookmarking
{
	public partial class BookmarkingRemoverFromFavouritePopup : System.Web.UI.UserControl
	{

		public const string Location = "~/Products/Community/Modules/Bookmarking/UserControls/BookmarkingRemoverFromFavouritePopup.ascx";

		protected void Page_Load(object sender, EventArgs e)
		{
			BookmarkingRemoveFromFavouriteContainer.Options.IsPopup = true;

			BookmarkingRemoveFromFavouriteLink.ButtonText = BookmarkingUCResource.RemoveButton;
			BookmarkingRemoveFromFavouriteLink.AjaxRequestText = BookmarkingUCResource.RemovingBookmarkFromFavouriteIsInProgressLabel;
		}
	}
}