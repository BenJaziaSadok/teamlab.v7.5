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
using ASC.Web.Community.Product;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Resources;

namespace ASC.Web.UserControls.Bookmarking.Common.Search
{
    public class BookmarkingSearchHandler : BaseSearchHandlerEx
    {
        public override ImageOptions Logo
        {
            get { return new ImageOptions { ImageFileName = "bookmarking_mini_icon.png", PartID = ModuleID }; }
        }

        public override string SearchName
        {
            get { return BookmarkingUCResource.BookmarkingSearch; }
        }

        public override SearchResultItem[] Search(string text)
        {
            return BookmarkingServiceHelper.GetCurrentInstanse().SearchBookmarksBySearchString(text);
        }

        public override Guid ModuleID
        {
            get { return BookmarkingSettings.ModuleId; }
        }

        public override Guid ProductID
        {
            get { return CommunityProduct.ID; }
        }

        public override IItemControl Control
        {
            get { return new ResultsView(); }
        }
    }
}