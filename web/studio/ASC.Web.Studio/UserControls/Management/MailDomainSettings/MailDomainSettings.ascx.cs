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
using System.Text.RegularExpressions;
using System.Web.UI;
using AjaxPro;
using ASC.Core;
using ASC.Core.Common.Logging;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.UserControls.Statistics;
using System.Web.UI.HtmlControls;
using System.Web;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("MailDomainSettingsController")]
    public partial class MailDomainSettings : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/MailDomainSettings/MailDomainSettings.ascx"; } }
        protected Tenant _currentTenant = null;
        protected StudioTrustedDomainSettings _studioTrustedDomainSettings;
        protected bool _enableInviteUsers;

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());

            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/maildomainsettings/js/maildomainsettings.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/maildomainsettings/css/maildomainsettings.less"));

            _currentTenant = CoreContext.TenantManager.GetCurrentTenant();
            _studioTrustedDomainSettings = SettingsManager.Instance.LoadSettings<StudioTrustedDomainSettings>(TenantProvider.CurrentTenantID);
            _enableInviteUsers = TenantStatisticsProvider.GetUsersCount() < TenantExtra.GetTenantQuota().ActiveUsers;

            if (!_enableInviteUsers)
                _studioTrustedDomainSettings.InviteUsersAsVisitors = true;
        }

        private bool CheckTrustedDomain(string domain)
        {
            return !string.IsNullOrEmpty(domain) && new Regex("^[a-z0-9]([a-z0-9-.]){1,98}[a-z0-9]$").IsMatch(domain);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveMailDomainSettings(TenantTrustedDomainsType type, List<string> domains, bool inviteUsersAsVisitors)
        {
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                var tenant = CoreContext.TenantManager.GetCurrentTenant();

                if (type == TenantTrustedDomainsType.Custom)
                {
                    tenant.TrustedDomains.Clear();
                    foreach (var domain in domains)
                    {
                        var d = (domain ?? "").Trim().ToLower();
                        if (!CheckTrustedDomain(d))
                            return new { Status = 0, Message = Resources.Resource.ErrorNotCorrectTrustedDomain };

                        tenant.TrustedDomains.Add(d);
                    }
                }

                if (tenant.TrustedDomains.Count == 0)
                {
                    tenant.TrustedDomainsType = TenantTrustedDomainsType.None;
                }
                else
                {
                    tenant.TrustedDomainsType = type;
                }

                var domainSettingsObj = new StudioTrustedDomainSettings { InviteUsersAsVisitors = inviteUsersAsVisitors };
                var resultStatus = SettingsManager.Instance.SaveSettings(domainSettingsObj, TenantProvider.CurrentTenantID);

                CoreContext.TenantManager.SaveTenant(tenant);

                AdminLog.PostAction("Settings: saved mail domain settings with parameters type={0}, domains={1}, inviteUsersAsVisitors={2}", type, string.Join("|", domains.ToArray()), inviteUsersAsVisitors.ToString());

                return new { Status = 1, Message = Resources.Resource.SuccessfullySaveSettingsMessage };
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }
        }
    }
}