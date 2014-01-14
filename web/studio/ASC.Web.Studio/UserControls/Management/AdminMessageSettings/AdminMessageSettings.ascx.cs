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
using ASC.Core.Common.Logging;
using AjaxPro;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using System.Web.UI.HtmlControls;
using System.Web;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("AdmMessController")]
    public partial class AdminMessageSettings : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/AdminMessageSettings/AdminMessageSettings.ascx"; } }
        protected StudioAdminMessageSettings _studioAdmMessNotifSettings;

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/Management/AdminMessageSettings/js/admmess.js"));

            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/AdminMessageSettings/css/admmess.less"));

            _studioAdmMessNotifSettings = SettingsManager.Instance.LoadSettings<StudioAdminMessageSettings>(TenantProvider.CurrentTenantID);
       
        }

        [AjaxMethod]
        public object SaveSettings(bool turnOn)
        {
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                var passwordSettingsObj = new StudioAdminMessageSettings { Enable = turnOn};
                var resultStatus = SettingsManager.Instance.SaveSettings(passwordSettingsObj, TenantProvider.CurrentTenantID);
                
                AdminLog.PostAction("Settings: saved admin message settings to \"{0}\"", turnOn);

                return new
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
    }
}