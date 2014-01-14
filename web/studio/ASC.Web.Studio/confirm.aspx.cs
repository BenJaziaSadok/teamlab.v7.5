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
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.Utility;
using Resources;

namespace ASC.Web.Studio
{
    //  emp-invite - confirm ivite by email
    //  portal-suspend - confirm portal suspending - Tenant.SetStatus(TenantStatus.Suspended)
    //  portal-continue - confirm portal continuation  - Tenant.SetStatus(TenantStatus.Active)
    //  portal-remove - confirm portal deletation - Tenant.SetStatus(TenantStatus.RemovePending)
    //  DnsChange - change Portal Address and/or Custom domain name
    public enum ConfirmType
    {
        EmpInvite,
        LinkInvite,
        PortalSuspend,
        PortalContinue,
        PortalRemove,
        DnsChange,
        PortalOwnerChange,
        Activation,
        EmailChange,
        EmailActivation,
        PasswordChange,
        ProfileRemove,
        PhoneActivation,
        PhoneAuth,
        Auth
    }

    public partial class confirm : MainPage
    {
        protected TenantInfoSettings _tenantInfoSettings;

        public string ErrorMessage { get; set; }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (CoreContext.Configuration.YourDocs)
                Context.Response.Redirect(CommonLinkUtility.FilesBaseAbsolutePath);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = HeaderStringHelper.GetPageTitle(Resource.AccountControlPageTitle);

            Master.DisabledSidePanel = true;
            Master.TopStudioPanel.DisableProductNavigation = true;
            Master.TopStudioPanel.DisableUserInfo = true;
            Master.TopStudioPanel.DisableSearch = true;
            Master.TopStudioPanel.DisableSettings = true;
            Master.TopStudioPanel.DisableVideo = true;

            _tenantInfoSettings = SettingsManager.Instance.LoadSettings<TenantInfoSettings>(TenantProvider.CurrentTenantID);

            var email = Request["email"] ?? "";
            var key = Request["key"] ?? "";
            var emplType = Request["emplType"] ?? "";
            var type = typeof(ConfirmType).TryParseEnum(Request["type"] ?? "", ConfirmType.EmpInvite);

            var validInterval = SetupInfo.ValidEamilKeyInterval;
            var authInterval = TimeSpan.FromHours(1);
            EmailValidationKeyProvider.ValidationResult checkKeyResult;

            UserInfo user = null;
            var tenant = CoreContext.TenantManager.GetCurrentTenant();
            if (tenant.Status != TenantStatus.Active && type != ConfirmType.PortalContinue)
            {
                Response.Redirect(SetupInfo.NoTenantRedirectURL, true);
            }

            if (type == ConfirmType.DnsChange)
            {
                var dnsChangeKey = string.Join(string.Empty, new[] { email, type.ToString(), Request["dns"], Request["alias"] });
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(dnsChangeKey, key, validInterval);
            }
            else if (type == ConfirmType.PortalContinue)
            {
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString(), key);
            }
            else if ((type == ConfirmType.EmpInvite || type == ConfirmType.Activation) && !String.IsNullOrEmpty(emplType))
            {
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString() + emplType, key, validInterval);
            }
            else if (type == ConfirmType.PasswordChange)
            {
                //Check activation signature
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString(), key, validInterval);
            }
            else if (type == ConfirmType.PortalOwnerChange && !String.IsNullOrEmpty(Request["uid"]))
            {
                var uid = Guid.Empty;
                try
                {
                    uid = new Guid(Request["uid"]);
                }
                catch
                {
                }
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString() + uid.ToString(), key, validInterval);
            }
            else if (type == ConfirmType.ProfileRemove && !(String.IsNullOrEmpty(Request["email"]) || String.IsNullOrEmpty(Request["key"])))
            {
                user = CoreContext.UserManager.GetUserByEmail(email);

                if (user.ID.Equals(Constants.LostUser.ID))
                {
                    ShowError(Resource.ErrorUserNotFound);
                    return;
                }

                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString(), key, validInterval);
            }
            else if (type == ConfirmType.EmpInvite && String.IsNullOrEmpty(email))
            {
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString(), key, TimeSpan.FromDays(3));
            }
            else if (type == ConfirmType.PhoneAuth || type == ConfirmType.PhoneActivation)
            {
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString(), key, authInterval);
            }
            else if (type == ConfirmType.Auth)
            {
                if (SecurityContext.IsAuthenticated)
                    Response.Redirect(CommonLinkUtility.GetDefault());

                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString(), key, authInterval);
                if (checkKeyResult == EmailValidationKeyProvider.ValidationResult.Ok)
                {
                    user = CoreContext.UserManager.GetUserByEmail(email);
                    var authCookie = SecurityContext.AuthenticateMe(user.ID);
                    CookiesManager.SetCookies(CookiesType.AuthKey, authCookie);
                    Response.Redirect(CommonLinkUtility.GetDefault());
                    return;
                }
            }else
            {
                checkKeyResult = EmailValidationKeyProvider.ValidateEmailKey(email + type.ToString() + emplType, key, validInterval);
            }

            if (type == ConfirmType.PhoneActivation && SecurityContext.IsAuthenticated)
            {
                Master.TopStudioPanel.DisableUserInfo = false;
            }

            if ((!email.TestEmailRegex() || checkKeyResult != EmailValidationKeyProvider.ValidationResult.Ok) && type != ConfirmType.LinkInvite)
            {
                ShowError(Resource.ErrorConfirmURLError);
                return;
            }

            if (!email.TestEmailRegex() && type != ConfirmType.LinkInvite)
            {
                ShowError(Resource.ErrorNotCorrectEmail);
                return;
            }

            if (checkKeyResult == EmailValidationKeyProvider.ValidationResult.Invalid)
            {
                //If check failed
                ShowError(Resource.ErrorInvalidActivationLink);
                return;
            }
            if (checkKeyResult == EmailValidationKeyProvider.ValidationResult.Expired)
            {
                //If link expired
                ShowError(Resource.ErrorExpiredActivationLink);
                return;
            }

            switch (type)
            {
                    //Invite
                case ConfirmType.EmpInvite:
                case ConfirmType.LinkInvite:
                case ConfirmType.Activation:
                    _confirmHolder2.Controls.Add(LoadControl(ConfirmInviteActivation.Location));
                    _contentWithControl.Visible = false;
                    break;

                case ConfirmType.EmailChange:
                case ConfirmType.PasswordChange:
                    _confirmHolder.Controls.Add(LoadControl(ConfirmActivation.Location));
                    break;

                case ConfirmType.EmailActivation:
                    ProcessEmailActivation(email);
                    break;

                case ConfirmType.PortalRemove:
                case ConfirmType.PortalSuspend:
                case ConfirmType.PortalContinue:
                case ConfirmType.DnsChange:
                    _confirmHolder.Controls.Add(LoadControl(ConfirmPortalActivity.Location));
                    break;

                case ConfirmType.PortalOwnerChange:
                    _confirmHolder.Controls.Add(LoadControl(ConfirmPortalOwner.Location));
                    break;

                case ConfirmType.ProfileRemove:
                    var control = (ProfileOperation)LoadControl(ProfileOperation.Location);
                    control.Key = key;
                    control.Email = email;
                    control.User = user;
                    _confirmHolder.Controls.Add(control);
                    break;

                case ConfirmType.PhoneActivation:
                case ConfirmType.PhoneAuth:
                    var confirmMobileActivation = (ConfirmMobileActivation)LoadControl(ConfirmMobileActivation.Location);
                    confirmMobileActivation.Activation = type == ConfirmType.PhoneActivation;
                    confirmMobileActivation.User = CoreContext.UserManager.GetUserByEmail(email);
                    _confirmHolder.Controls.Add(confirmMobileActivation);
                    break;
            }
        }

        private void ProcessEmailActivation(string email)
        {
            var user = CoreContext.UserManager.GetUserByEmail(email);

            if (user.ID.Equals(Constants.LostUser.ID))
            {
                ShowError(Resource.ErrorConfirmURLError);
            }
            else if (user.ActivationStatus == EmployeeActivationStatus.Activated)
            {
                Response.Redirect("~/");
            }
            else
            {
                try
                {
                    SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                    user.ActivationStatus = EmployeeActivationStatus.Activated;
                    CoreContext.UserManager.SaveUserInfo(user);
                }
                finally
                {
                    SecurityContext.Logout();
                    CookiesManager.ClearCookies(CookiesType.AuthKey);
                }

                var redirectUrl = String.Format("~/auth.aspx?confirmed-email={0}", email);
                Response.Redirect(redirectUrl, true);
            }
        }

        private void ShowError(string error)
        {
            if (SecurityContext.IsAuthenticated)
            {
                ErrorMessage = error;
                _confirmHolder.Visible = false;
            }
            else
            {
                Response.Redirect(string.Format("~/auth.aspx?m={0}", HttpUtility.UrlEncode(error)));
            }
        }
    }
}