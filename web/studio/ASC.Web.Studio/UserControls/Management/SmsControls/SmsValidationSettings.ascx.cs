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
using System.Web.UI;
using ASC.Core;
using ASC.Core.Common.Logging;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.SMS;
using AjaxPro;
using Resources;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("AjaxPro.SmsValidationSettingsController")]
    public partial class SmsValidationSettings : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/SmsControls/SmsValidationSettings.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/Management/SmsControls/js/SmsValidation.js"));

            SmsBuyHolder.Controls.Add(LoadControl(SmsBuy.Location));
        }

        [AjaxMethod]
        public object SaveSettings(bool smsEnable)
        {
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                if (smsEnable && StudioSmsNotificationSettings.SentSms >= StudioSmsNotificationSettings.PaidSms)
                    throw new Exception(Resource.SmsNotPaidError);

                StudioSmsNotificationSettings.Enable = smsEnable;

                AdminLog.PostAction("Settings: saved sms validation settings to {0}", smsEnable);

                return new
                    {
                        Status = 1,
                        Message = Resource.SuccessfullySaveSettingsMessage
                    };
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }
        }
    }
}