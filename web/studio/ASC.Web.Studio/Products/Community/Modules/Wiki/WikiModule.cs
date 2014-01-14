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
using ASC.Web.Community.Wiki;
using ASC.Web.Community.Wiki.Common;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.UserControls.Wiki;
using ASC.Web.UserControls.Wiki.Resources;

[assembly: Product(typeof(WikiModule))]

namespace ASC.Web.Community.Wiki
{
    public class WikiModule : Module
    {
        public override Guid ID
        {
            get { return WikiManager.ModuleId; }
        }

        public override Guid ProjectId
        {
            get { return CommunityProduct.ID; }
        }

        public override string Name
        {
            get { return WikiResource.ModuleName; }
        }

        public override string Description
        {
            get { return WikiResource.ModuleDescription; }
        }

        public override string StartURL
        {
            get { return "~/products/community/modules/wiki/"; }
        }

        public static string GetCreateContentPageUrl()
        {
            return VirtualPathUtility.ToAbsolute(WikiManager.BaseVirtualPath + "/default.aspx") + "?action=new";
        }


        public WikiModule()
        {
            Context = new ModuleContext
            {
                DefaultSortOrder = 5,
                SmallIconFileName = "WikiLogo16.png",
                IconFileName = "WikiLogo32.png",
                SubscriptionManager = new WikiSubscriptionManager(),
                GetCreateContentPageAbsoluteUrl = GetCreateContentPageUrl,
                SearchHandler = new WikiSearchHandler(),
            };
        }
    }
}
