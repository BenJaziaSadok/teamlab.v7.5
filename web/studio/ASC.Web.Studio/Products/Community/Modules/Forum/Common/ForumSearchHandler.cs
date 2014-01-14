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
using System.Linq;
using System.Web;
using ASC.Forum;
using ASC.Web.Community.Product;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.Forum
{
    public class ForumSearchHandler : BaseSearchHandlerEx
    {
        public override SearchResultItem[] Search(string text)
        {
            int topicCount;
            var findTopicList = ForumDataProvider.SearchTopicsByText(TenantProvider.CurrentTenantID, text, 1, -1, out topicCount);

            return findTopicList.Select(topic => new SearchResultItem
                {
                    Name = topic.Title,
                    Description = String.Format(Resources.ForumResource.FindTopicDescription, topic.ThreadTitle),
                    URL = VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/posts.aspx") + "?t=" + topic.ID,
                    Date = topic.CreateDate
                }).ToArray();
        }

        public override ImageOptions Logo
        {
            get { return new ImageOptions { ImageFileName = "forum_mini_icon.png", PartID = ForumManager.ModuleID }; }
        }

        public override string SearchName
        {
            get { return Resources.ForumResource.SearchDefaultString; }
        }

        public override Guid ModuleID
        {
            get { return ForumManager.ModuleID; }
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