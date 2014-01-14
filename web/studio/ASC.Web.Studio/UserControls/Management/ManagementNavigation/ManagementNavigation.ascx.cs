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
using ASC.Web.Studio.UserControls.Common.HelpCenter;
using ASC.Web.Studio.UserControls.Common.Support;

namespace ASC.Web.Studio.UserControls.Management.NavigationSidePanel
{
    public partial class ManagementNavigation : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/ManagementNavigation/ManagementNavigation.ascx"; }
        }

        protected string CurrentPage
        {
            get { return (HttpContext.Current.Request["type"] ?? "common").ToLower(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var help = (HelpCenter)LoadControl(HelpCenter.Location);
            help.IsSideBar = true;
            HelpHolder.Controls.Add(help);
            SupportHolder.Controls.Add(LoadControl(Support.Location));
        }
    }
}