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
using System.Linq;
using System.Web;
using ASC.Blogs.Core;
using ASC.Blogs.Core.Resources;
using ASC.Core;
using ASC.Web.Community.Product;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Controls.Common;

namespace ASC.Web.Community.Blogs
{
    public class BlogsSearchHandler : BaseSearchHandlerEx
    {
        public override SearchResultItem[] Search(string text)
        {
            var posts = BasePage.GetEngine().SearchPosts(text, new PagingQuery());
            var result = new List<SearchResultItem>(posts.Count);
            result.AddRange(posts.Select(post => new SearchResultItem
                {
                    Description = BlogsResource.Blogs + ", " + DisplayUserSettings.GetFullUserName(CoreContext.UserManager.GetUsers(post.UserID), false) + ", " + post.Datetime.ToLongDateString(),
                    Name = post.Title,
                    URL = VirtualPathUtility.ToAbsolute(Constants.BaseVirtualPath) + "viewblog.aspx?blogid=" + post.ID.ToString(),
                    Date = post.Datetime
                }));

            return result.ToArray();
        }

        public override ImageOptions Logo
        {
            get { return new ImageOptions {ImageFileName = "blog_add.png", PartID = Constants.ModuleID}; }
        }

        public override string SearchName
        {
            get { return BlogsResource.SearchDefaultString; }
        }

        public override Guid ProductID
        {
            get { return CommunityProduct.ID; }
        }

        public override Guid ModuleID
        {
            get { return Constants.ModuleID; }
        }

        public override IItemControl Control
        {
            get { return new ResultsView(); }
        }
    }
}