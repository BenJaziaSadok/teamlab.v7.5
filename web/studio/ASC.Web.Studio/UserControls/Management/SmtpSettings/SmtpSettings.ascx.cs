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
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Studio.Core;
using SmtpSettingsModel = ASC.Core.Configuration.SmtpSettings;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxPro.AjaxNamespace("SmtpSettings")]
    public partial class SmtpSettings : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/SmtpSettings/SmtpSettings.ascx"; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType(), Page);
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/smtpsettings/js/smtpsettings.js"));
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "smtpsettings_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebPath.GetPath("usercontrols/management/smtpsettings/css/smtpsettings.css") + "\">", false);
        }

        [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)]
        public void Save(SmtpSettingsModel settings)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            CoreContext.Configuration.SmtpSettings = settings;
        }

        [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)]
        public void Test(SmtpSettingsModel settings)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            var smtpClient = new SmtpClient(settings.Host, settings.Port ?? 25)
            {
                EnableSsl = settings.EnableSSL
            };

            if (!string.IsNullOrEmpty(settings.CredentialsUserName) && !string.IsNullOrEmpty(settings.CredentialsUserPassword))
                smtpClient.Credentials = new NetworkCredential(settings.CredentialsUserName, settings.CredentialsUserPassword);

            smtpClient.Send(
                new MailMessage(new MailAddress(settings.SenderAddress, settings.SenderDisplayName),
                                new MailAddress("DCC6B687CD2C40A3845D95615A6DC013@test.test")));
        }
    }
}