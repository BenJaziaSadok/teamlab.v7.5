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
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio
{
    public partial class Tariffs : MainPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (CoreContext.Configuration.YourDocs)
                Context.Response.Redirect(CommonLinkUtility.FilesBaseAbsolutePath);

            if (!TenantExtra.EnableTarrifSettings)
                Response.Redirect("~/", true);

            if (CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor())
                Response.Redirect("~/", true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.DisabledSidePanel = true;

            if (TenantStatisticsProvider.IsNotPaid())
            {
                Master.TopStudioPanel.DisableProductNavigation = true;
                Master.TopStudioPanel.DisableSettings = true;
                Master.TopStudioPanel.DisableSearch = true;
                Master.TopStudioPanel.DisableVideo = true;
            }

            Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Tariffs);

            pageContainer.Controls.Add(LoadControl(TariffSettings.Location));
        }
    }
}