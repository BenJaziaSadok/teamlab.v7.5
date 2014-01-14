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
using ASC.Common.Data;
using ASC.Common.Data.Sql.Expressions;
using ASC.Core;
using ASC.Core.Caching;
using ASC.Core.Users;
using ASC.Web.Community.Controls;
using ASC.Web.Community.Product;
using ASC.Web.Core;
using ASC.Web.Studio;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.Utility;


namespace ASC.Web.Community
{
    public partial class _Default : MainPage
    {
        private static ICache showEmptyScreen = new AspCache();
        private const String NavigationPanelCookieName = "asc_minimized_np";

        private bool ShowEmptyScreen()
        {
            if (CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor())
            {
                return false;
            }
            if (showEmptyScreen.Get("communityScreen" + TenantProvider.CurrentTenantID) != null)
            {
                return false;
            }

            var q = new SqlExp(string.Format(@"select exists(select 1 from blogs_posts where tenant = {0}) or " +
                "exists(select 1 from bookmarking_bookmark where tenant = {0}) or exists(select 1 from events_feed where tenant = {0}) or " +
                "exists(select 1 from forum_category where tenantid = {0})", CoreContext.TenantManager.GetCurrentTenant().TenantId));

            using (var db = new DbManager("community"))
            {
                var hasacitvity = db.ExecuteScalar<bool>(q);
                if (hasacitvity)
                {
                    showEmptyScreen.Insert("communityScreen" + TenantProvider.CurrentTenantID, new object(), TimeSpan.FromMinutes(30));
                }
                return !hasacitvity;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ShowEmptyScreen())
            {
                var dashboardEmptyScreen = (DashboardEmptyScreen)Page.LoadControl(DashboardEmptyScreen.Location);
                dashboardEmptyScreen.ProductStartUrl = GetStartUrl();
                emptyModuleCommunity.Controls.Add(dashboardEmptyScreen);
                ImportUsers.Controls.Add(new ImportUsersWebControl());
                return;
            }

            Response.Redirect(GetStartUrl());
        }

        protected Boolean isMinimized()
        {
            return !String.IsNullOrEmpty(CookiesManager.GetCookies(CookiesType.MinimizedNavpanel));
        }


        protected string RenderGreetingTitle()
        {
            return CoreContext.TenantManager.GetCurrentTenant().Name.HtmlEncode();
        }

        private string GetStartUrl()
        {
            var enabledList = new Dictionary<int, string>();
            var modules = WebItemManager.Instance.GetSubItems(CommunityProduct.ID);

            foreach (var m in modules)
            {
                switch (m.GetSysName())
                {
                    case "community-blogs":
                        enabledList.Add(1, "modules/blogs/");
                        break;
                    case "community-news":
                        enabledList.Add(2, "modules/news/");
                        break;
                    case "community-forum":
                        enabledList.Add(3, "modules/forum/");
                        break;
                    case "community-bookmarking":
                        enabledList.Add(4, "modules/bookmarking/");
                        break;
                    case "community-wiki":
                        enabledList.Add(5, "modules/wiki/");
                        break;
                }
            }
            if (enabledList.Count > 0)
            {
                var keys = enabledList.Keys.ToList();
                keys.Sort();
                return enabledList[keys[0]];
            }
            return "modules/birthdays/";
        }

    }
}
