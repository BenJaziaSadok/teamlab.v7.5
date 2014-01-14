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
using ASC.Web.Community.News.Code.DAO;
using ASC.Web.Community.News.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility.HtmlUtility;

namespace ASC.Web.Community.News.Code.Module
{
    public class SearchHandler : BaseSearchHandlerEx
    {
        public override SearchResultItem[] Search(string text)
        {
            return FeedStorageFactory.Create()
                                     .SearchFeeds(text)
                                     .ConvertAll(f => new SearchResultItem
                                         {
                                             Name = f.Caption,
                                             Description = HtmlUtility.GetText(f.Text, 120),
                                             URL = FeedUrls.GetFeedUrl(f.Id),
                                             Date = f.Date
                                         })
                                     .ToArray();
        }

        public override ImageOptions Logo
        {
            get { return new ImageOptions { ImageFileName = "newslogo.png", PartID = NewsConst.ModuleId }; }
        }

        public override string SearchName
        {
            get { return NewsResource.SearchDefaultString; }
        }

        public override Guid ModuleID
        {
            get { return NewsConst.ModuleId; }
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