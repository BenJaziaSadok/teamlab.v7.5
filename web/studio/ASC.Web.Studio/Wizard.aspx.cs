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
using ASC.Web.Studio.UserControls.FirstTime;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using ASC.Core;
using ASC.Web.Core.Utility.Settings;

namespace ASC.Web.Studio
{
    public partial class Wizard : MainPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (CoreContext.Configuration.YourDocs)
                Context.Response.Redirect(CommonLinkUtility.FilesBaseAbsolutePath);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            Page.RegisterBodyScripts(ResolveUrl("~/js/third-party/head.load.min.js"));
            Page.RegisterBodyScripts(ResolveUrl("~/js/asc/core/asc.listscript.js"));

            var settings = SettingsManager.Instance.LoadSettings<WizardSettings>(TenantProvider.CurrentTenantID);
            if (settings.Completed)
            {
                Response.Redirect("~/");
            }

            Title = Resources.Resource.WizardPageTitle;

            Master.DisabledSidePanel = true;
            Master.TopStudioPanel.DisableProductNavigation = true;
            Master.TopStudioPanel.DisableUserInfo = true;
            Master.TopStudioPanel.DisableSearch = true;
            Master.TopStudioPanel.DisableSettings = true;
            Master.TopStudioPanel.DisableTariff = true;
            Master.TopStudioPanel.DisableTariffNotify = true;
            Master.TopStudioPanel.DisableVideo = true;

            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            content.Controls.Add(LoadControl(StepContainer.Location));
        }
    }
}