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
using System.Text;
using System.Web;
using System.Web.UI;
using AjaxPro;
using ASC.Core;
using ASC.Core.Common.Logging;
using ASC.Core.Tenants;
using ASC.Security.Cryptography;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.SMS;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("StudioSettings")]
    public partial class StudioSettings : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/StudioSettings.ascx"; }
        }

        public Guid ProductID { get; set; }

        protected bool EnableDomain
        {
            get { return TenantExtra.GetTenantQuota().HasDomain; }
        }

        protected static bool EnableDnsChange
        {
            get { return !string.IsNullOrEmpty(CoreContext.TenantManager.GetCurrentTenant().MappedDomain); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());

            //transfer portal           
            _transferPortalSettings.Controls.Add(LoadControl(TransferPortal.Location));

            //timezone & language
            _timelngHolder.Controls.Add(LoadControl(TimeAndLanguage.Location));

            if (SetupInfo.IsVisibleSettings<PromoCode>() && 
                TenantExtra.GetCurrentTariff().State == ASC.Core.Billing.TariffState.Trial &&
                string.IsNullOrEmpty(CoreContext.TenantManager.GetCurrentTenant().PartnerId))
            {
                promoCodeSettings.Controls.Add(LoadControl(PromoCode.Location));
            }

            //Portal version
            if (SetupInfo.IsVisibleSettings<VersionSettings.VersionSettings>() && 1 < CoreContext.TenantManager.GetTenantVersions().Count())
                _portalVersionSettings.Controls.Add(LoadControl(VersionSettings.VersionSettings.Location));

            //main domain settings
            _mailDomainSettings.Controls.Add(LoadControl(MailDomainSettings.Location));

            //strong security password settings
            _strongPasswordSettings.Controls.Add(LoadControl(PasswordSettings.Location));

            //invitational link
            invLink.Controls.Add(LoadControl(InviteLink.Location));

            //sms settings
            var loadSms = true;
            var partnerId = CoreContext.TenantManager.GetCurrentTenant().PartnerId;
            if (!string.IsNullOrEmpty(partnerId))
            {
                var partner = CoreContext.PaymentManager.GetPartner(partnerId);

                if (partner != null && partner.Status == PartnerStatus.Approved && !partner.Removed && partner.PartnerType != PartnerType.System)
                {
                    loadSms = false;
                }
            }
            if (StudioSmsNotificationSettings.IsVisibleSettings && loadSms)
                _smsValidationSettings.Controls.Add(LoadControl(SmsValidationSettings.Location));

            //admin message settings
            _admMessSettings.Controls.Add(LoadControl(AdminMessageSettings.Location));

            //default page settings
            _defaultPageSeettings.Controls.Add(LoadControl(DefaultPageSettings.Location));
        }

        #region Check custom domain name

        /// <summary>
        /// Custom domain name shouldn't ends with tenant base domain name.
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        private static bool CheckCustomDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(TenantBaseDomain) &&
                (domain.EndsWith(TenantBaseDomain, StringComparison.InvariantCultureIgnoreCase) || domain.Equals(TenantBaseDomain.TrimStart('.'), StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }
            Uri test;
            if (Uri.TryCreate(domain.Contains(Uri.SchemeDelimiter) ? domain : Uri.UriSchemeHttp + Uri.SchemeDelimiter + domain, UriKind.Absolute, out test))
            {
                try
                {
                    CoreContext.TenantManager.CheckTenantAddress(test.Host);
                }
                catch (TenantIncorrectCharsException)
                {
                }
                return true;
            }
            return false;
        }

        #endregion

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse SaveDnsSettings(string dnsName, string alias, bool enableDns)
        {
            var resp = new AjaxResponse { rs1 = "1" };
            try
            {
                if (!EnableDomain)
                    throw new Exception(Resources.Resource.ErrorNotAllowedOption);

                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                var tenant = CoreContext.TenantManager.GetCurrentTenant();

                if (!enableDns || string.IsNullOrEmpty(dnsName))
                {
                    dnsName = null;
                }
                if (dnsName == null || CheckCustomDomain(dnsName))
                {
                    if (string.IsNullOrEmpty(alias))
                    {
                        alias = tenant.TenantAlias;
                    }

                    if (CoreContext.Configuration.Standalone)
                    {
                        tenant.MappedDomain = dnsName;
                        CoreContext.TenantManager.SaveTenant(tenant);
                        return resp;
                    }
                    else
                    {
                        if (!tenant.TenantAlias.Equals(alias))
                        {
                            CoreContext.TenantManager.CheckTenantAddress(alias);
                        }
                    }

                    if ((!string.IsNullOrEmpty(alias) && tenant.TenantAlias != alias) || tenant.MappedDomain != dnsName)
                    {
                        var portalAddress = string.Format("http://{0}.{1}", alias ?? String.Empty, SetupInfo.BaseDomain);

                        var u = CoreContext.UserManager.GetUsers(tenant.OwnerId);
                        StudioNotifyService.Instance.SendMsgDnsChange(tenant, GenerateDnsChangeConfirmUrl(u.Email, dnsName, alias, ConfirmType.DnsChange), portalAddress, dnsName);
                        resp.rs2 = string.Format(Resources.Resource.DnsChangeMsg, string.Format("<a href='mailto:{0}'>{0}</a>", u.Email));

                        AdminLog.PostAction("Settings: saved DNS settings with parameters dnsName={0}, alias={1}, enableDns={2}", dnsName, alias, enableDns);
                    }
                }
                else
                {
                    resp.rs1 = "0";
                    resp.rs2 = "<div class='errorBox'>" + Resources.Resource.ErrorNotCorrectTrustedDomain + "</div>";
                }
            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = "<div class='errorBox'>" + e.Message.HtmlEncode() + "</div>";
            }
            return resp;
        }

        private static string GenerateDnsChangeConfirmUrl(string email, string dnsName, string tenantAlias, ConfirmType confirmType)
        {
            var key = string.Join(string.Empty, new[] { email, confirmType.ToString(), dnsName, tenantAlias });
            var validationKey = EmailValidationKeyProvider.GetEmailKey(key);

            var sb = new StringBuilder();
            sb.Append(CommonLinkUtility.GetFullAbsolutePath("~/confirm.aspx"));
            sb.AppendFormat("?email={0}&key={1}&type={2}", HttpUtility.UrlEncode(email), validationKey, confirmType.ToString());
            if (!string.IsNullOrEmpty(dnsName))
            {
                sb.AppendFormat("&dns={0}", dnsName);
            }
            if (!string.IsNullOrEmpty(tenantAlias))
            {
                sb.AppendFormat("&alias={0}", tenantAlias);
            }
            return sb.ToString();
        }

        protected static string TenantBaseDomain
        {
            get
            {
                return String.IsNullOrEmpty(SetupInfo.BaseDomain)
                           ? String.Empty
                           : String.Format(".{0}", SetupInfo.BaseDomain);
            }
        }

        public static string ModifyHowToAdress(string adr)
        {
            var lang = CoreContext.TenantManager.GetCurrentTenant().Language;
            if (lang.Contains("-"))
            {
                lang = lang.Split('-')[0];
            }
            if (lang != "en") lang += "/";
            else lang = string.Empty;
            return string.Format("{0}/{1}{2}", "http://www.teamlab.com", lang, adr ?? string.Empty);
        }
    }
}