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
using ASC.Core;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.UserControls.Management.HelpBlock;
using ASC.Web.Studio.UserControls.Management.NavigationSidePanel;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio
{
    public partial class Management : MainPage
    {
        private ManagementType _managementType;

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (CoreContext.Configuration.YourDocs)
                Context.Response.Redirect(CommonLinkUtility.FilesBaseAbsolutePath);

            if (!SecurityContext.CheckPermissions(SecutiryConstants.EditPortalSettings))
            {
                Response.Redirect(VirtualPathUtility.ToAbsolute("~/"));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.Attributes.Add("autocomplete", "off");

            Master.DisabledSidePanel = false;

            SideNavigation.Controls.Add(LoadControl(ManagementNavigation.Location));

            _managementType = ManagementType.General;
            if (!String.IsNullOrEmpty(Request["type"]))
            {
                try
                {
                    _managementType = (ManagementType)Convert.ToInt32(Request["type"]);
                }
                catch
                {
                    _managementType = ManagementType.General;
                }
            }

            var standalone = CoreContext.Configuration.Standalone;

            //content
            switch (_managementType)
            {
                case ManagementType.Mail:

                    if (!(standalone || SetupInfo.IsVisibleSettings<MailSettings>()))
                        Response.Redirect("~/", true);

                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.SmtpSettings);
                    _settingsContainer.Body.Controls.Add(LoadControl(MailSettings.Location));
                    break;

                case ManagementType.Statistic:

                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.StatisticsTitle);
                    _settingsContainer.Body.Controls.Add(LoadControl(ProductQuotes.Location));
                    _settingsContainer.Body.Controls.Add(LoadControl(VisitorsChart.Location));
                    break;

                case ManagementType.Account:

                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Backup);
                    _settingsContainer.Body.Controls.Add(LoadControl(Backup.Location));
                    break;

                case ManagementType.Customization:

                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Customization);

                    //greeting settings
                    _settingsContainer.Body.Controls.Add(LoadControl(GreetingSettings.Location));

                    //naming people
                    _settingsContainer.Body.Controls.Add(LoadControl(NamingPeopleSettings.Location));

                    //color themes settings
                    _settingsContainer.Body.Controls.Add(LoadControl(ColorThemes.Location));

                    break;

                case ManagementType.AccessRights:

                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.AccessRights);
                    _settingsContainer.Body.Controls.Add(LoadControl(AccessRights.Location));
                    break;

                case ManagementType.HelpCenter:

                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.HelpCenter);
                    _settingsContainer.Body.Controls.Add(LoadControl(Help.Location));
                    break;

                case ManagementType.ProductsAndInstruments:

                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.ProductsAndInstruments);
                    _settingsContainer.Body.Controls.Add(LoadControl(ProductsAndInstruments.Location));
                    break;

                case ManagementType.AuthorizationKeys:

                    if (!standalone)
                        Response.Redirect("~/", true);

                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.ThirdPartyAuthorization);
                    _settingsContainer.Body.Controls.Add(LoadControl(AuthorizationKeys.Location));
                    _settingsContainer.Body.Controls.Add(LoadControl(SmtpSettings.Location));
                    break;

                default:

                    Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.GeneralSettings);

                    var settingsControl = LoadControl(StudioSettings.Location) as StudioSettings;
                    _settingsContainer.Body.Controls.Add(settingsControl);
                    break;
            }
        }
    }
}