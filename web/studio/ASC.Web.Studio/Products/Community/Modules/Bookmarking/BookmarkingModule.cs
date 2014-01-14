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
using System.Web;
using ASC.Bookmarking.Business.Permissions;
using ASC.Bookmarking.Business.Subscriptions;
using ASC.Web.Community.Bookmarking;
using ASC.Web.Community.Bookmarking.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.UserControls.Bookmarking.Common;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;
using ASC.Web.UserControls.Bookmarking.Common.Search;

[assembly: Product(typeof(BookmarkingModule))]

namespace ASC.Web.Community.Bookmarking
{
    public class BookmarkingModule : Module
    {
        public override Guid ID
        {
            get { return BookmarkingSettings.ModuleId; }
        }

        public override Guid ProjectId
        {
            get { return CommunityProduct.ID; }
        }

        public override string Name
        {
            get { return BookmarkingResource.AddonName; }
        }

        public override string Description
        {
            get { return BookmarkingResource.AddonDescriptionResourceKey; }
        }

        public override string StartURL
        {
            get { return "~/products/community/modules/bookmarking/"; }
        }

        public BookmarkingModule()
        {
            Context = new ModuleContext
                {
                    DefaultSortOrder = 4,
                    SmallIconFileName = "bookmarking_mini_icon.png",
                    IconFileName = "bookmarking_icon.png",
                    SubscriptionManager = new BookmarkingSubscriptionManager(),
                    SearchHandler = new BookmarkingSearchHandler(),
                    GetCreateContentPageAbsoluteUrl = () => BookmarkingPermissionsCheck.PermissionCheckCreateBookmark() ? VirtualPathUtility.ToAbsolute("~/products/community/modules/bookmarking/" + BookmarkingServiceHelper.GetCreateBookmarkPageUrl()) : null,
                };
        }
    }
}