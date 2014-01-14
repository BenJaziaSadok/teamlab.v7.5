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
using ASC.Web.Studio.UserControls.Statistics;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Web.Core.Security;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using System.Net.Mail;
using ASC.Core.Users;

namespace ASC.Web.Studio.UserControls
{
    [AjaxNamespace("AuthCommunicationsController")]
    public partial class AuthCommunications : System.Web.UI.UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Users/AuthCommunications.ascx"; }
        }

        public bool MaxHighAdmMess { get; set; }

        protected bool ShowSeparator { get; private set; }

        protected bool EnabledJoin
        {
            get
            {
                var t = CoreContext.TenantManager.GetCurrentTenant();
                return (t.TrustedDomainsType == TenantTrustedDomainsType.Custom && t.TrustedDomains.Count > 0) ||
                       t.TrustedDomainsType == TenantTrustedDomainsType.All;
            }
        }

        protected bool EnableAdmMess
        {
            get
            {
                var setting = SettingsManager.Instance.LoadSettings<StudioAdminMessageSettings>(TenantProvider.CurrentTenantID);
                return setting.Enable || TenantStatisticsProvider.IsNotPaid();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (MaxHighAdmMess)
            {
                _joinBlock.Visible = false;
                _sendAdmin.Visible = true;
            }
            else
            {
                _joinBlock.Visible = EnabledJoin;

                _sendAdmin.Visible = EnableAdmMess;
            }
            if (EnabledJoin || EnableAdmMess)
                AjaxPro.Utility.RegisterTypeForAjax(GetType());

            ShowSeparator = _joinBlock.Visible && _sendAdmin.Visible;
        }

        [SecurityPassthrough]
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SendAdmMail(string email, string message)
        {
            try
            {
                if (email == null || email.Trim().Length == 0)
                {
                    throw new ArgumentException("Email is empty.", "email");
                }
                if (message == null || message.Trim().Length == 0)
                {
                    throw new ArgumentException("Message is empty.", "message");
                }
                StudioNotifyService.Instance.SendMsgToAdminFromNotAuthUser(email, message);
                return new { Status = 1, Message = Resources.Resource.AdminMessageSent };
            }
            catch (Exception ex)
            {
                return new { Status = 0, Message = ex.Message.HtmlEncode() };
            }
        }

        [SecurityPassthrough]
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse SendJoinInviteMail(string email)
        {

            email = (email ?? "").Trim();
            var resp = new AjaxResponse { rs1 = "0" };

            try
            {
                if (String.IsNullOrEmpty(email))
                {
                    resp.rs2 = Resources.Resource.ErrorNotCorrectEmail;
                    return resp;
                }

                if (!email.TestEmailRegex())
                    resp.rs2 = Resources.Resource.ErrorNotCorrectEmail;

                var user = CoreContext.UserManager.GetUserByEmail(email);
                if (!user.ID.Equals(ASC.Core.Users.Constants.LostUser.ID))
                {
                    resp.rs1 = "0";
                    resp.rs2 = CustomNamingPeople.Substitute<Resources.Resource>("ErrorEmailAlreadyExists").HtmlEncode();
                    return resp;
                }

                var tenant = CoreContext.TenantManager.GetCurrentTenant();
                var trustedDomainSettings = SettingsManager.Instance.LoadSettings<StudioTrustedDomainSettings>(TenantProvider.CurrentTenantID);
                var emplType = trustedDomainSettings.InviteUsersAsVisitors ? EmployeeType.Visitor : EmployeeType.User;
                var enableInviteUsers = TenantStatisticsProvider.GetUsersCount() < TenantExtra.GetTenantQuota().ActiveUsers;

                if (!enableInviteUsers)
                    emplType = EmployeeType.Visitor;

                if (tenant.TrustedDomainsType == TenantTrustedDomainsType.Custom)
                {
                    var address = new MailAddress(email);
                    foreach (var d in tenant.TrustedDomains)
                    {
                        if (address.Address.EndsWith("@" + d, StringComparison.InvariantCultureIgnoreCase))
                        {
                            StudioNotifyService.Instance.InviteUsers(email, "", true, emplType);
                            resp.rs1 = "1";
                            resp.rs2 = Resources.Resource.FinishInviteJoinEmailMessage;
                            return resp;
                        }
                    }
                }
                else if (tenant.TrustedDomainsType == TenantTrustedDomainsType.All)
                {
                    StudioNotifyService.Instance.InviteUsers(email, "", true, emplType);
                    resp.rs1 = "1";
                    resp.rs2 = Resources.Resource.FinishInviteJoinEmailMessage;
                    return resp;
                }

                resp.rs2 = Resources.Resource.ErrorNotCorrectEmail;
            }
            catch (FormatException)
            {
                resp.rs2 = Resources.Resource.ErrorNotCorrectEmail;
            }
            catch (Exception e)
            {
                resp.rs2 = HttpUtility.HtmlEncode(e.Message);
            }

            return resp;
        }

        public static string RenderTrustedDominTitle()
        {
            var tenant = CoreContext.TenantManager.GetCurrentTenant();
            if (tenant.TrustedDomainsType == TenantTrustedDomainsType.Custom)
            {
                var domains = String.Empty;
                var i = 0;
                foreach (var d in tenant.TrustedDomains)
                {
                    if (i != 0)
                        domains += ", ";

                    domains += d;
                    i++;
                }
                return String.Format(Resources.Resource.TrustedDomainsInviteTitle, domains);
            }
            else if (tenant.TrustedDomainsType == TenantTrustedDomainsType.All)
                return Resources.Resource.SignInFromAnyDomainInviteTitle;

            return "";
        }
    }
}