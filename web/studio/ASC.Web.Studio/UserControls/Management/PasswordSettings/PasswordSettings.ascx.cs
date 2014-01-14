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
using AjaxPro;
using ASC.Core.Common.Logging;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using System.Web;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("PasswordSettingsController")]
    public partial class PasswordSettings : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/PasswordSettings/PasswordSettings.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());

            Page.RegisterBodyScripts(ResolveUrl("~/js/third-party/jquery/jquery.ui.slider.js"));
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/PasswordSettings/js/PasswordSettings.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/passwordsettings/css/passwordsettings.less"));
        }

        [AjaxMethod]
        public object SavePasswordSettings(string objData)
        {
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var passwordSettingsObj = jsSerializer.Deserialize<StudioPasswordSettings>(objData);
                SettingsManager.Instance.SaveSettings(passwordSettingsObj, TenantProvider.CurrentTenantID);

                AdminLog.PostAction("Settings: saved password strength settings to {0}", objData);

                return
                    new
                        {
                            Status = 1,
                            Message = Resources.Resource.SuccessfullySaveSettingsMessage
                        };
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }
        }

        [AjaxMethod]
        public string LoadPasswordSettings()
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            var passwordSettingsObj = SettingsManager.Instance.LoadSettings<StudioPasswordSettings>(TenantProvider.CurrentTenantID);

            return serializer.Serialize(passwordSettingsObj);
        }
    }
}