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

using ASC.Core;
using ASC.Core.Caching;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.SMS;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using ASC.Web.Studio.Utility;
using Resources;
using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Web;
using System.Web.UI;

namespace ASC.Web.Studio.UserControls.Common
{
    public partial class Authorize : UserControl
    {
        protected string LoginMessage;
        private string _errorMessage;
        private ICache cache = new AspCache();

        protected string ErrorMessage
        {
            get
            {
                return string.IsNullOrEmpty(_errorMessage)
                           ? string.Empty
                           : "<div class='errorBox'>" + _errorMessage.HtmlEncode() + "</div>";
            }
            set { _errorMessage = value; }
        }

        protected string Login;
        protected string Password;
        protected string HashId;

        protected string ConfirmedEmail;

        public bool IsLogout { get; set; }

        public static string Location
        {
            get { return "~/UserControls/Common/Authorize/Authorize.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/common/authorize/css/authorize.less"));

            Login = "";
            Password = "";
            HashId = "";

            //Account link control
            AccountLinkControl accountLink = null;
            if (SetupInfo.ThirdPartyAuthEnabled && AccountLinkControl.IsNotEmpty)
            {
                accountLink = (AccountLinkControl)LoadControl(AccountLinkControl.Location);
                accountLink.Visible = true;
                accountLink.ClientCallback = "authCallback";
                accountLink.SettingsView = false;
                signInPlaceholder.Controls.Add(accountLink);
            }

            //top panel
            var master = Page.Master as BaseTemplate;
            if (master != null)
            {
                master.TopStudioPanel.DisableProductNavigation = true;
                master.TopStudioPanel.DisableSearch = true;
                master.TopStudioPanel.DisableVideo = true;
            }

            Page.Title = HeaderStringHelper.GetPageTitle(Resource.Authorization);

            pwdReminderHolder.Controls.Add(LoadControl(PwdTool.Location));

            var msg = Request["m"];
            if (!string.IsNullOrEmpty(msg))
            {
                ErrorMessage = msg;
            }

            if (IsPostBack && !SecurityContext.IsAuthenticated)
            {
                var tryByHash = false;
                var smsLoginUrl = string.Empty;
                try
                {
                    if (!string.IsNullOrEmpty(Request["__EVENTARGUMENT"]) && Request["__EVENTTARGET"] == "signInLogin" && accountLink != null)
                    {
                        HashId = Request["__EVENTARGUMENT"];
                    }

                    if (!string.IsNullOrEmpty(Request["login"]))
                    {
                        Login = Request["login"].Trim();
                    }
                    else if (string.IsNullOrEmpty(HashId))
                    {
                        throw new InvalidCredentialException("login");
                    }

                    if (!string.IsNullOrEmpty(Request["pwd"]))
                    {
                        Password = Request["pwd"];
                    }
                    else if (string.IsNullOrEmpty(HashId))
                    {
                        throw new InvalidCredentialException("password");
                    }

                    
                    var counter = (int)(cache.Get("loginsec/" + Login) ?? 0);
                    if (++counter%5 == 0)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                    }
                    cache.Insert("loginsec/" + Login, counter, DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)));

                    smsLoginUrl = SmsLoginUrl(accountLink);
                    if (string.IsNullOrEmpty(smsLoginUrl))
                    {
                        if (string.IsNullOrEmpty(HashId))
                        {
                            var cookiesKey = SecurityContext.AuthenticateMe(Login, Password);
                            CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);
                        }
                        else
                        {
                            Guid userId;
                            tryByHash = TryByHashId(accountLink, HashId, out userId);
                            var cookiesKey = SecurityContext.AuthenticateMe(userId);
                            CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);
                        }
                    }
                }
                catch (InvalidCredentialException)
                {
                    Auth.ProcessLogout();
                    ErrorMessage = tryByHash ? Resource.LoginWithAccountNotFound : Resource.InvalidUsernameOrPassword;
                    return;
                }
                catch (System.Security.SecurityException)
                {
                    Auth.ProcessLogout();
                    ErrorMessage = Resource.ErrorDisabledProfile;
                    return;
                }
                catch (Exception ex)
                {
                    Auth.ProcessLogout();
                    ErrorMessage = ex.Message;
                    return;
                }

                if (!string.IsNullOrEmpty(smsLoginUrl))
                {
                    Response.Redirect(smsLoginUrl);
                }

                var refererURL = (string)Session["refererURL"];
                if (string.IsNullOrEmpty(refererURL))
                {
                    Response.Redirect("~/");
                }
                else
                {
                    Session["refererURL"] = null;
                    Response.Redirect(refererURL);
                }
            }

            ProcessConfirmedEmailCondition();
        }

        private static bool TryByHashId(AccountLinkControl accountLinkControl, string hashId, out Guid userId)
        {
            userId = Guid.Empty;
            if (accountLinkControl == null || string.IsNullOrEmpty(hashId))
            {
                return false;
            }

            var accountsStrId = accountLinkControl.GetLinker().GetLinkedObjectsByHashId(hashId);
            userId = accountsStrId
                .Select(x =>
                            {
                                try
                                {
                                    return new Guid(x);
                                }
                                catch
                                {
                                    return Guid.Empty;
                                }
                            })
                .Where(x => x != Guid.Empty)
                .FirstOrDefault(x => CoreContext.UserManager.UserExists(x));

            return true;
        }

        private string SmsLoginUrl(AccountLinkControl accountLinkControl)
        {
            if (!StudioSmsNotificationSettings.IsVisibleSettings
                || !StudioSmsNotificationSettings.Enable)
                return string.Empty;

            UserInfo user;

            if (string.IsNullOrEmpty(HashId))
            {
                user = CoreContext.UserManager.GetUsers(TenantProvider.CurrentTenantID, Login, Hasher.Base64Hash(Password, HashAlg.SHA256));
            }
            else
            {
                Guid userId;
                TryByHashId(accountLinkControl, HashId, out userId);
                user = CoreContext.UserManager.GetUsers(userId);
            }

            if (user == null)
                return string.Empty;

            var confirmType =
                string.IsNullOrEmpty(user.MobilePhone) ||
                user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated
                    ? ConfirmType.PhoneActivation
                    : ConfirmType.PhoneAuth;

            return StudioNotifyService.GenerateConfirmUrl(user.Email, confirmType);
        }

        private void ProcessConfirmedEmailCondition()
        {
            if (IsPostBack) return;

            var confirmedEmail = Request.QueryString["confirmed-email"];

            if (String.IsNullOrEmpty(confirmedEmail) || !confirmedEmail.TestEmailRegex()) return;

            Login = confirmedEmail;
            LoginMessage = String.Format("<div class=\"confirmBox\">{0} {1}</div>", Resource.MessageEmailConfirmed, Resource.MessageAuthorize);
        }
    }
}