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
using System.Linq;
using System.Threading;
using System.Web.UI;
using AjaxPro;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using System.Web.UI.HtmlControls;
using System.Web;

namespace ASC.Web.Studio.UserControls.Management.VersionSettings
{
    [AjaxNamespace("VersionSettingsController")]
    public partial class VersionSettings : System.Web.UI.UserControl
    {
        public const string Location = "~/UserControls/Management/VersionSettings/VersionSettings.ascx";

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());

            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/versionsettings/css/versionsettings.less"));
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/Management/VersionSettings/js/script.js"));
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SwitchVersion(string version)
        {
            try
            {
                int tenantVersion = int.Parse(version);

                if (!CoreContext.TenantManager.GetTenantVersions().Any(x => x.Id == tenantVersion)) throw new ArgumentException(Resources.Resource.SettingsBadPortalVersion);

                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
                var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
                try
                {
                    CoreContext.TenantManager.SetTenantVersion(tenant, tenantVersion);
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
                catch (ArgumentException e)
                {
                    throw new ArgumentException(Resources.Resource.SettingsAlreadyCurrentPortalVersion,e);
                }
                return new { Status = 1 };
            }
            catch (Exception e)
            {
                return new { Status = 0, e.Message };

            }
        }

        protected string GetLocalizedName(string name)
        {
            try
            {
                var localizedName = Resources.Resource.ResourceManager.GetString(("version_"+name.Replace(".","")).ToLowerInvariant());
                if (string.IsNullOrEmpty(localizedName))
                {
                    localizedName = name;
                }
                return localizedName;
            }
            catch (Exception)
            {
                return name;
            }
        }
    }
}