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
using System.Collections.Generic;
using System.Web;
using ASC.Web.Community.Product;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Utility.HtmlUtility;
using ASC.Web.UserControls.Wiki;
using ASC.Web.UserControls.Wiki.Handlers;
using ASC.Web.UserControls.Wiki.Resources;
using ASC.Web.UserControls.Wiki.UC;

namespace ASC.Web.Community.Wiki.Common
{
    public class WikiSearchHandler : BaseSearchHandlerEx
    {
        public override SearchResultItem[] Search(string text)
        {
            var list = new List<SearchResultItem>();
            var defPageHref = VirtualPathUtility.ToAbsolute(WikiManager.ViewVirtualPath);

            foreach (var page in new WikiEngine().SearchPagesByContent(text))
            {
                var pageName = page.PageName;
                if (string.IsNullOrEmpty(pageName))
                {
                    pageName = WikiResource.MainWikiCaption;
                }

                list.Add(new SearchResultItem
                    {
                        Name = pageName,
                        Description = HtmlUtility.GetText(
                            EditPage.ConvertWikiToHtml(page.PageName, page.Body, defPageHref,
                                                       WikiSection.Section.ImageHangler.UrlFormat, TenantProvider.CurrentTenantID), 120),
                        URL = ActionHelper.GetViewPagePath(defPageHref, page.PageName),
                        Date = page.Date
                    });
            }
            return list.ToArray();
        }

        public override ImageOptions Logo
        {
            get { return new ImageOptions { ImageFileName = "WikiLogo16.png", PartID = WikiManager.ModuleId }; }
        }

        public override string SearchName
        {
            get { return WikiManager.SearchDefaultString; }
        }

        public override Guid ModuleID
        {
            get { return WikiManager.ModuleId; }
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