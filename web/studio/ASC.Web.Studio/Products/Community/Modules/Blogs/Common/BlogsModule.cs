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
using ASC.Blogs.Core.Resources;
using ASC.Blogs.Core.Security;
using ASC.Core;
using ASC.Web.Community.Blogs.Common;
using ASC.Web.Community.Product;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;

[assembly: Product(typeof(BlogsModule))]

namespace ASC.Web.Community.Blogs.Common
{
    public class BlogsModule : Module
    {
        public override Guid ID
        {
            get { return new Guid("6A598C74-91AE-437d-A5F4-AD339BD11BB2"); }
        }

        public override Guid ProjectId
        {
            get { return CommunityProduct.ID; }
        }

        public override string Name
        {
            get { return BlogsResource.AddonName; }
        }

        public override string Description
        {
            get { return BlogsResource.AddonDescriptionResourceKey; }
        }

        public override string StartURL
        {
            get { return "~/products/community/modules/blogs/"; }
        }        

        public BlogsModule()
        {
            Context = new ModuleContext
            {
                DefaultSortOrder = 1,
                SmallIconFileName = "blog_add.png",
                IconFileName = "blogiconwg.png",
                SubscriptionManager = new BlogsSubscriptionManager(),
                GetCreateContentPageAbsoluteUrl = () => CanEdit() ? VirtualPathUtility.ToAbsolute(ASC.Blogs.Core.Constants.BaseVirtualPath + "addblog.aspx") : null,
                SearchHandler = new BlogsSearchHandler(),
            };
        }

        private static bool CanEdit()
        {
            return CommunitySecurity.CheckPermissions(new PersonalBlogSecObject(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID)), ASC.Blogs.Core.Constants.Action_AddPost);
        }
    }
}
