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
using ASC.Web.Community.Forum.Common;
using ASC.Web.Community.Forum.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.UserControls.Forum.Common;

[assembly: Product(typeof(ForumModule))]

namespace ASC.Web.Community.Forum.Common
{
    public class ForumModule : Module
    {
        public override Guid ID
        {
            get { return ForumManager.ModuleID; }
        }

        public override Guid ProjectId
        {
            get { return CommunityProduct.ID; }
        }

        public override string Name
        {
            get { return ForumResource.AddonName; }
        }

        public override string Description
        {
            get { return ForumResource.AddonDescription; }
        }

        public override string StartURL
        {
            get { return "~/products/community/modules/forum/"; }
        }
     
        public ForumModule()
        {
            Context = new ModuleContext
            {
                DefaultSortOrder = 2,
                SmallIconFileName = "forum_mini_icon.png",
                IconFileName = "forum_icon.png",
                SubscriptionManager = new ForumSubscriptionManager(),
                GetCreateContentPageAbsoluteUrl = ForumShortcutProvider.GetCreateContentPageUrl,
                SearchHandler = new ForumSearchHandler(),
            };
        }
    }
}
