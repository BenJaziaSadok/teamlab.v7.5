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
using System.Globalization;
using System.Text;
using System.Threading;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Core.Security;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Management;
using log4net;
using System.Web;

namespace ASC.Web.Studio.UserControls.FirstTime
{
    [AjaxNamespace("EmailAndPasswordController")]
    public partial class EmailAndPassword : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/FirstTime/EmailAndPassword.ascx"; } }

        protected Tenant _curTenant;

        protected bool IsVisibleTestData { get; set; }

        protected bool IsVisiblePromocode {
            get { return (string.IsNullOrEmpty(ASC.Core.CoreContext.TenantManager.GetCurrentTenant().PartnerId) && !CoreContext.Configuration.Standalone); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            
            InitScript();

            _curTenant = CoreContext.TenantManager.GetCurrentTenant();
            _dateandtimeHolder.Controls.Add(LoadControl(TimeAndLanguage.Location));

            IsVisibleTestData = SetupInfo.PortalTestDataEnable;
        }

        private void InitScript()
        {
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/firsttime/js/manager.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/firsttime/css/EmailAndPassword.less"));

            var script = new StringBuilder();

            script.AppendFormat(@"ASC.Controls.EmailAndPasswordManager.init('{0}','{1}','{2}','{3}','{4}');",
                Resources.Resource.EmailAndPasswordTypeChangeIt.ReplaceSingleQuote(),
                Resources.Resource.EmailAndPasswordOK.ReplaceSingleQuote(),
                Resources.Resource.EmailAndPasswordWrongPassword.ReplaceSingleQuote(),
                Resources.Resource.EmailAndPasswordEmptyPassword.ReplaceSingleQuote(),
                Resources.Resource.EmailAndPasswordIncorrectEmail.ReplaceSingleQuote()
            );

            Page.RegisterInlineScript(script.ToString());
        }

        [AjaxMethod]
        [SecurityPassthrough]
        public object SaveData(string email, string pwd, string lng, bool populateDemoData, string promocode)
        {
            try
            {
                var tenant = CoreContext.TenantManager.GetCurrentTenant();
                var settings = SettingsManager.Instance.LoadSettings<WizardSettings>(tenant.TenantId);
                if (settings.Completed)
                {
                    return new { Status = 0, Message = "Wizard passed." };
                }

                if (tenant.OwnerId == Guid.Empty)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(6)); // wait cache interval
                    tenant = CoreContext.TenantManager.GetTenant(tenant.TenantId);
                    if (tenant.OwnerId == Guid.Empty)
                    {
                        LogManager.GetLogger("ASC.Web.FirstTime").Error(tenant.TenantId + ": owner id is empty.");
                    }
                }
                var currentUser = CoreContext.UserManager.GetUsers(CoreContext.TenantManager.GetCurrentTenant().OwnerId);

                if (!currentUser.IsOwner())
                {
                    return new { Status = 0, Message = Resources.Resource.EmailAndPasswordNotOwner };
                }
                if (!UserManagerWrapper.ValidateEmail(email))
                {
                    return new { Status = 0, Message = Resources.Resource.EmailAndPasswordIncorrectEmail };
                }

                SecurityContext.AuthenticateMe(currentUser.ID);
                UserManagerWrapper.SetUserPassword(currentUser.ID, pwd);

                email = email.Trim();
                if (currentUser.Email != email)
                {
                    currentUser.Email = email;
                    currentUser.ActivationStatus = EmployeeActivationStatus.NotActivated;
                }
                CoreContext.UserManager.SaveUserInfo(currentUser);

                var cookie = SecurityContext.AuthenticateMe(currentUser.ID);
                CookiesManager.SetCookies(CookiesType.AuthKey, cookie);

                if (!string.IsNullOrWhiteSpace(promocode))
                {
                    try
                    {
                        CoreContext.PaymentManager.ActivateKey(promocode);
                    }
                    catch (Exception err)
                    {
                        LogManager.GetLogger("ASC.Web.FirstTime").ErrorFormat("Incorrect Promo: {0}\r\n{1}", promocode, err);
                        return new { Status = 0, Message = Resources.Resource.EmailAndPasswordIncorrectPromocode };
                    }
                }

                settings.Completed = true;
                SettingsManager.Instance.SaveSettings(settings, tenant.TenantId);

                TrySetLanguage(tenant, lng);
                FirstTimeTenantSettings.SetDefaultTenantSettings();
                FirstTimeTenantSettings.SendInstallInfo(currentUser);
                if (populateDemoData)
                {
                    FirstTimeTenantSettings.SetTenantData(lng);
                }

                return new { Status = 1, Message = Resources.Resource.EmailAndPasswordSaved };
            }
            catch (Exception ex)
            {
                return new { Status = 0, Message = ex.Message };
            }
        }

        public string GetEmail()
        {
            var currentUser = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            return currentUser.Email;
        }

        private void TrySetLanguage(Tenant tenant, string lng)
        {
            if (!string.IsNullOrEmpty(lng))
            {
                try
                {
                    var culture = CultureInfo.GetCultureInfo(lng);
                    tenant.Language = culture.Name;
                }
                catch (Exception err)
                {
                    LogManager.GetLogger("ASC.Web.FirstTime").Error(err);
                }
            }
        }
    }
}