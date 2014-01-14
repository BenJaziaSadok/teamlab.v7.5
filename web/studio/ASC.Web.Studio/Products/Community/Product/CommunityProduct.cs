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
using ASC.Web.Community.Resources;
using ASC.Web.Core;

namespace ASC.Web.Community.Product
{
    public class CommunityProduct : Core.Product
    {
        private static ProductContext ctx;


        public static Guid ID
        {
            get { return WebItemManager.CommunityProductID; }
        }

        public override Guid ProductID
        {
            get { return ID; }
        }

        public override string Name
        {
            get { return CommunityResource.ProductName; }
        }

        public override string ExtendedDescription
        {
            get { return string.Format(CommunityResource.ProductDescriptionExt, "<span style='display:none'>", "</span>"); }
        }

        public override string Description
        {
            get { return CommunityResource.ProductDescription; }
        }

        public override string StartURL
        {
            get { return "~/products/community"; }
        }

        public override string ProductClassName
        {
            get { return "community"; }
        }

        public override ProductContext Context
        {
            get { return ctx; }
        }


        public override void Init()
        {
            ctx = new ProductContext
            {
                MasterPageFile = "~/products/community/community.master",
                DisabledIconFileName = "product_disabled_logo.png",
                IconFileName = "product_logo.png",
                LargeIconFileName = "product_logolarge.png",
                DefaultSortOrder = 40,

                SubscriptionManager = new CommunitySubscriptionManager(),

                SpaceUsageStatManager = new CommunitySpaceUsageStatManager(),

                AdminOpportunities = () => CommunityResource.ProductAdminOpportunities.Split('|').ToList(),
                UserOpportunities = () => CommunityResource.ProductUserOpportunities.Split('|').ToList(),
            };
        }
    }
}
