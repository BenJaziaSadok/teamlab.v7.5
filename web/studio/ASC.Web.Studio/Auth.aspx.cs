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
using ASC.Web.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio
{
    public partial class Auth : MainPage
    {
        protected string LogoPath = SettingsManager.Instance.LoadSettings<TenantInfoSettings>(TenantProvider.CurrentTenantID).GetAbsoluteCompanyLogoPath();
        protected bool withHelpBlock { get; set; }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (!SecurityContext.IsAuthenticated) return;

            if (IsLogout)
            {
                ProcessLogout();
                Response.Redirect("~/auth.aspx");
            }
            else
            {
                Response.Redirect("~/");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.DisabledSidePanel = true;
            withHelpBlock = false;
            if (CoreContext.Configuration.YourDocsDemo)
            {
                if (AccountLinkControl.IsNotEmpty)
                    HolderLoginWithThirdParty.Controls.Add(LoadControl(LoginWithThirdParty.Location));
            }
            else
            {
                var authControl = (Authorize) LoadControl(Authorize.Location);
                authControl.IsLogout = IsLogout;
                AuthorizeHolder.Controls.Add(authControl);

                if (!CoreContext.Configuration.YourDocs)
                    CommunitationsHolder.Controls.Add(LoadControl(AuthCommunications.Location));
                    withHelpBlock = true;
            }
        }

        public static void ProcessLogout()
        {
            //logout
            CookiesManager.ClearCookies(CookiesType.AuthKey);
            SecurityContext.Logout();
        }

        private bool IsLogout
        {
            get
            {
                var logoutParam = Request["t"];
                if (String.IsNullOrEmpty(logoutParam))
                    return false;

                return logoutParam.ToLower() == "logout";
            }
        }
    }
}