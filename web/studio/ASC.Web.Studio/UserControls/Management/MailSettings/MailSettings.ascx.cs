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
using System.Web.UI;
using ASC.Core.Common.Logging;
using AjaxPro;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Core;
using System.Web.UI.HtmlControls;
using System.Web;

namespace ASC.Web.Studio.UserControls.Management
{

    [AjaxNamespace("MailSettingsController")]
    public partial class MailSettings : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/MailSettings/MailSettings.ascx"; } }

        protected bool _isPesonalSMTP = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());

            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/mailsettings/js/mailsettings.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/mailsettings/css/mailsettings.less"));

            if (!String.IsNullOrEmpty(CoreContext.Configuration.SmtpSettings.CredentialsDomain)
               || !String.IsNullOrEmpty(CoreContext.Configuration.SmtpSettings.CredentialsUserName))
                _isPesonalSMTP = true;
        }


        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveSmtpSettings(string host, string port, bool enableSSL, string senderAddress, string senderDisplayName,
                                             string credentialsDomain, string credentialsUserName, string credentialsUserPwd)
        {
            var portStr = string.Empty;
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
                                
                int portResult;
                if (string.IsNullOrEmpty(port) || !int.TryParse(port, out portResult))
                {
                    portResult = 25;
                }
                portStr = portResult.ToString();

                var settings = new ASC.Core.Configuration.SmtpSettings()
                {
                    CredentialsDomain = String.IsNullOrEmpty(credentialsDomain) ? credentialsDomain : credentialsDomain.Trim(),
                    CredentialsUserName = String.IsNullOrEmpty(credentialsUserName) ? credentialsUserName : credentialsUserName.Trim(),
                    CredentialsUserPassword = String.IsNullOrEmpty(credentialsUserPwd) ? credentialsUserPwd : credentialsUserPwd.Trim(),
                    Host = String.IsNullOrEmpty(host) ? host : host.Trim(),
                    Port = portResult,
                    EnableSSL = enableSSL,
                    SenderAddress = String.IsNullOrEmpty(senderAddress) ? senderAddress : senderAddress.Trim(),
                    SenderDisplayName = String.IsNullOrEmpty(senderDisplayName) ? senderDisplayName : senderDisplayName.Trim()
                };

                CoreContext.Configuration.SmtpSettings = settings;

                AdminLog.PostAction("Settings: saved portal smtp settings to {0:Json}", settings);

                return new { Status = 1, Port = portStr, Message = Resources.Resource.SuccessfullySaveSmtpSettingsMessage };
                
            }
            catch (Exception e)
            {
                return new { Status = 0, Port = portStr, Message = e.Message.HtmlEncode() };                
            }
            
        }


        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object TestSmtpSettings(string email)
        {

            AjaxResponse resp = new AjaxResponse();
            if (!email.TestEmailRegex())            
                return new { Status = 0, Message = Resources.Resource.ErrorNotCorrectEmail };                            

            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                MailMessage mail = new MailMessage();
                mail.To.Add(email);

                mail.Subject = Resources.Resource.TestSMTPEmailSubject;
                mail.Priority = MailPriority.Normal;
                mail.IsBodyHtml = false;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.From = new MailAddress(CoreContext.Configuration.SmtpSettings.SenderAddress,
                                                    CoreContext.Configuration.SmtpSettings.SenderDisplayName,
                                                    System.Text.Encoding.UTF8);


                mail.Body = Resources.Resource.TestSMTPEmailBody;

                SmtpClient client = new SmtpClient(CoreContext.Configuration.SmtpSettings.Host, CoreContext.Configuration.SmtpSettings.Port ?? 25);
                client.EnableSsl = CoreContext.Configuration.SmtpSettings.EnableSSL;

                if (client.EnableSsl)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                }

                if (String.IsNullOrEmpty(CoreContext.Configuration.SmtpSettings.CredentialsUserName))
                {
                    client.UseDefaultCredentials = true;
                }
                else
                {
                    client.Credentials = new NetworkCredential(
                        CoreContext.Configuration.SmtpSettings.CredentialsUserName,
                        CoreContext.Configuration.SmtpSettings.CredentialsUserPassword,
                        CoreContext.Configuration.SmtpSettings.CredentialsDomain);
                }
                client.Send(mail);

                return new { Status = 1, Message = Resources.Resource.SuccessfullySMTPTestMessage };         
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };  
            }
        }
    }
}