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

#region Import

using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ASC.CRM.Core;
using ASC.Core.Common.Logging;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.CRM.Classes;
using ASC.Web.Studio.Utility;
using AjaxPro;
using ASC.Common.Threading.Progress;
using System.Text;
using ASC.Web.CRM.Resources;

#endregion

namespace ASC.Web.CRM.Controls.Settings
{
    [AjaxNamespace("AjaxPro.CommonSettingsView")]
    public partial class CommonSettingsView : BaseUserControl
    {
        #region Property

        public static string Location { get { return PathProvider.GetFileStaticRelativePath("Settings/CommonSettingsView.ascx"); } }

        protected List<CurrencyInfo> AllCurrencyRates { get; set; }

        protected bool MobileVer = false;
        
        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            MobileVer = Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);
            _sendTestMailContainer.Options.IsPopup = true;
            Utility.RegisterTypeForAjax(typeof(CommonSettingsView));
            AllCurrencyRates = CurrencyProvider.GetAll().Where(n => n.IsConvertable).ToList();

            SMTPServerSetting settings = Global.TenantSettings.SMTPServerSetting;
            Page.JsonPublisher(settings, "SMTPSettings");
            RegisterScript();
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"ASC.CRM.SettingsPage.initSMTPSettings('{0}');",
                String.Format(CRMSettingResource.pattern_TestMailSMTPMainBody, ASC.Core.CoreContext.TenantManager.GetTenant(TenantProvider.CurrentTenantID).TenantDomain).HtmlEncode().ReplaceSingleQuote()
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        [AjaxMethod]
        public IProgressItem StartExportData()
        {
            if (!CRMSecurity.IsAdmin)
                throw new Exception();

            AdminLog.PostAction("CRM Settings: started crm data export");

            return ExportToCSV.Start();
        }
        
        [AjaxMethod]
        public void SaveChangeSettings(String defaultCurrency)
        {
            if (!CRMSecurity.IsAdmin)
                throw new Exception();

            var tenantSettings = Global.TenantSettings;

            tenantSettings.DefaultCurrency = CurrencyProvider.Get(defaultCurrency);

            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);

            AdminLog.PostAction("CRM Settings: saved default currency settings to \"{0:Json}\"", defaultCurrency);

        }

        [AjaxMethod]
        public void SaveSMTPSettings(string host, int port, bool authentication, string hostLogin, string hostPassword, string senderDisplayName, string senderEmailAddress, bool enableSSL)
        {
            var crmSettings = Global.TenantSettings;

            crmSettings.SMTPServerSetting = new SMTPServerSetting
            {
                Host = host,
                Port = port,
                RequiredHostAuthentication = authentication,
                HostLogin = hostLogin,
                HostPassword = hostPassword,
                SenderDisplayName = senderDisplayName,
                SenderEmailAddress = senderEmailAddress,
                EnableSSL = enableSSL
            };

            SettingsManager.Instance.SaveSettings(crmSettings, TenantProvider.CurrentTenantID);

            AdminLog.PostAction("CRM Settings: saved crm smtp settings to {0}", crmSettings);
        }

        [AjaxMethod]
        public string SendTestMailSMTP(string toEmail, string mailSubj, string mailBody)
        {
            MailSender.StartSendTestMail(toEmail, mailSubj, mailBody);
            return "";
        }

        [AjaxMethod]
        public IProgressItem GetStatus()
        {
            return ExportToCSV.GetStatus();
        }

        [AjaxMethod]
        public IProgressItem Cancel()
        {
            ExportToCSV.Cancel();

            return GetStatus();
        }

        #endregion

    }
}