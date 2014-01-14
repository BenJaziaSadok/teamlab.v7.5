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
using System.Web.UI;
using ASC.Core;
using ASC.Web.Core;
using System.Web.UI.HtmlControls;

namespace ASC.Web.Studio.UserControls.Feed
{
    public partial class FeedList : UserControl
    {
        private static Guid User
        {
            get { return SecurityContext.CurrentAccount.ID; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(LoadControl(VirtualPathUtility.ToAbsolute("~/Masters/FeedBodyScripts.ascx")));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/UserControls/Feed/css/feed.less"));
            Page.RegisterInlineScript(@"new Feed('" + AccessRights() + "').init();");
        }

        public static string Location
        {
            get { return "~/UserControls/Feed/FeedList.ascx"; }
        }

        public static string AccessRights()
        {
            return string.Join(",", new[]
                {
                    WebItemSecurity.IsAvailableForUser(WebItemManager.CommunityProductID.ToString(), User).ToString(),
                    WebItemSecurity.IsAvailableForUser(WebItemManager.CRMProductID.ToString(), User).ToString(),
                    WebItemSecurity.IsAvailableForUser(WebItemManager.ProjectsProductID.ToString(), User).ToString(),
                    WebItemSecurity.IsAvailableForUser(WebItemManager.DocumentsProductID.ToString(), User).ToString()
                });
        }
    }
}