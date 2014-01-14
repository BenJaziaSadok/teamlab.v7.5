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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Core.Common.Logging;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using System.Web.UI.HtmlControls;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("TimeAndLanguageSettingsController")]
    public partial class TimeAndLanguage : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/TimeAndLanguage/TimeAndLanguage.ascx"; } }

        protected Tenant _currentTenant;

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/TimeAndLanguage/js/TimeAndLanguage.js"));

            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/TimeAndLanguage/css/TimeAndLanguage.less"));

            _currentTenant = CoreContext.TenantManager.GetCurrentTenant();
        }

        protected string RenderLanguageSelector()
        {
            var sb = new StringBuilder();
            sb.Append("<select id=\"studio_lng\" class=\"comboBox\">");
            foreach (var ci in SetupInfo.EnabledCultures)
            {
                sb.AppendFormat("<option " + (String.Equals(_currentTenant.GetCulture().Name, ci.Name) ? "selected" : "") + " value=\"{0}\">{1}</option>", ci.Name, ci.DisplayName);
            }
            sb.Append("</select>");

            return sb.ToString();
        }

        protected string RenderTimeZoneSelector()
        {
            var sb = new StringBuilder("<select id=\"studio_timezone\" class=\"comboBox\">");
            foreach (var timeZone in TimeZoneInfo.GetSystemTimeZones().OrderBy(z => z.BaseUtcOffset))
            {
                sb.AppendFormat("<option " + (timeZone.Equals(_currentTenant.TimeZone) ? "selected" : string.Empty) + " value=\"{0}\">{1}</option>", timeZone.Id, timeZone.DisplayName);
            }
            sb.Append("</select>");
            return sb.ToString();
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveLanguageTimeSettings(string lng, string timeZoneID)
        {
            var resp = new AjaxResponse();
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                var tenant = CoreContext.TenantManager.GetCurrentTenant();
                var culture = CultureInfo.GetCultureInfo(lng);

                var changelng = false;
                if (SetupInfo.EnabledCultures.Find(c => String.Equals(c.Name, culture.Name, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    if (!String.Equals(tenant.Language, culture.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        tenant.Language = culture.Name;
                        changelng = true;
                    }
                }

                var oldTimeZone = tenant.TimeZone;
                tenant.TimeZone = new List<TimeZoneInfo>(TimeZoneInfo.GetSystemTimeZones()).Find(tz => String.Equals(tz.Id, timeZoneID));

                CoreContext.TenantManager.SaveTenant(tenant);
                
                if (!tenant.TimeZone.Id.Equals(oldTimeZone.Id) || changelng)
                {
                    AdminLog.PostAction("Settings: saved language and time zone settings with parameters language={0},time={1}", lng, timeZoneID);
                }

                if (changelng)
                {
                    return new { Status = 1, Message = String.Empty };
                }
                else
                {
                    return new { Status = 2, Message = Resources.Resource.SuccessfullySaveSettingsMessage };
                }
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }
        }       
    }
}