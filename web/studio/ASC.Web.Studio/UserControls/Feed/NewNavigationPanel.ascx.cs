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
using System.Web.UI;
using ASC.Core;
using ASC.Web.Core;
using ASC.Web.Studio.UserControls.Common.Support;

namespace ASC.Web.Studio.UserControls.Feed
{
    public partial class NewNavigationPanel : UserControl
    {
        private static Guid User
        {
            get { return SecurityContext.CurrentAccount.ID; }
        }

        protected bool IsProductAvailable(string product)
        {
            switch (product)
            {
                case "community":
                    return WebItemSecurity.IsAvailableForUser(WebItemManager.CommunityProductID.ToString(), User);
                case "crm":
                    return WebItemSecurity.IsAvailableForUser(WebItemManager.CRMProductID.ToString(), User);
                case "projects":
                    return WebItemSecurity.IsAvailableForUser(WebItemManager.ProjectsProductID.ToString(), User);
                case "documents":
                    return WebItemSecurity.IsAvailableForUser(WebItemManager.DocumentsProductID.ToString(), User);
                default:
                    return false;
            }
        }

        public static string Location
        {
            get { return "~/UserControls/Feed/NewNavigationPanel.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SupportHolder.Controls.Add(LoadControl(Support.Location));
        }
    }
}