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
using System.Web.UI;

using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;

using ASC.Web.People.Classes;
using ASC.Web.People.Core;
using ASC.Web.Studio.UserControls.Common.HelpCenter;
using ASC.Web.Studio.UserControls.Common.Support;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;


namespace ASC.Web.People.UserControls
{
    public partial class SideNavigationPanel : UserControl
    {
        protected List<UserInfo> Profiles;
        protected List<MyGroup> Groups;

        protected bool HasPendingProfiles;
        protected bool EnableAddUsers;
        protected bool CurrentUserAdmin;

        public static string Location
        {
            get { return PeopleProduct.ProductPath + "UserControls/SideNavigationPanel.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitData();

            Page.RegisterBodyScripts(ResolveUrl("~/products/people/js/sideNavigationPanel.js"));

            GroupRepeater.DataSource = Groups;
            GroupRepeater.DataBind();

            var help = (HelpCenter)LoadControl(HelpCenter.Location);
            help.IsSideBar = true;
            HelpHolder.Controls.Add(help);
            SupportHolder.Controls.Add(LoadControl(Support.Location));
        }

        private void InitData()
        {
            Groups = CoreContext.UserManager.GetDepartments().Select(r => new MyGroup(r)).ToList();
            Groups.Sort((group1, group2) => String.Compare(group1.Title, group2.Title, StringComparison.Ordinal));

            Profiles = CoreContext.UserManager.GetUsers().ToList();

            HasPendingProfiles = Profiles.FindAll(u => u.ActivationStatus == EmployeeActivationStatus.Pending).Count > 0;
            EnableAddUsers =  TenantStatisticsProvider.GetUsersCount() < TenantExtra.GetTenantQuota().ActiveUsers;
            CurrentUserAdmin = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsAdmin();
        }
    }
}