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
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Profile;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using ASC.Web.Studio.Utility;
using Resources;
using System.Text.RegularExpressions;
using System.Configuration;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class ConfirmInviteActivation : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/ConfirmInviteActivation/ConfirmInviteActivation.ascx"; }
        }

        protected string _errorMessage;

        protected TenantInfoSettings _tenantInfoSettings;
        protected string _userName;
        protected string _userPost;
        protected string _userAvatar;

        protected string _email
        {
            get { return (Request["email"] ?? String.Empty).Trim(); }
        }

        protected string _firstName
        {
            get { return (Request["firstname"] ?? String.Empty).Trim(); }
        }

        protected string _lastName
        {
            get { return (Request["lastname"] ?? String.Empty).Trim(); }
        }

        protected string _pwd
        {
            get { return (Request["pwd"] ?? "").Trim(); }
        }

        protected string _rePwd
        {
            get { return (Request["repwd"] ?? "").Trim(); }
        }

        protected ConfirmType _type
        {
            get { return typeof (ConfirmType).TryParseEnum(Request["type"] ?? "", ConfirmType.EmpInvite); }
        }

        protected EmployeeType _employeeType
        {
            get { return typeof(EmployeeType).TryParseEnum(Request["emplType"] ?? "", EmployeeType.User); }
        }

        protected string GetEmailAddress()
        {
            if (!String.IsNullOrEmpty(Request["emailInput"]))
                return Request["emailInput"].Trim();

            if (!String.IsNullOrEmpty(Request["email"]))
                return Request["email"].Trim();

            return String.Empty;
        }

        private string GetEmailAddress(LoginProfile account)
        {
            var value = GetEmailAddress();
            return String.IsNullOrEmpty(value) ? account.EMail : value;
        }

        protected string GetFirstName()
        {
            var value = string.Empty;
            if (!string.IsNullOrEmpty(Request["firstname"])) value = Request["firstname"].Trim();
            if (!string.IsNullOrEmpty(Request["firstnameInput"])) value = Request["firstnameInput"].Trim();
            return HtmlUtil.GetText(value);
        }

        private string GetFirstName(LoginProfile account)
        {
            var value = GetFirstName();
            return String.IsNullOrEmpty(value) ? account.FirstName : value;
        }

        protected string GetLastName()
        {
            var value = string.Empty;
            if (!string.IsNullOrEmpty(Request["lastname"])) value = Request["lastname"].Trim();
            if (!string.IsNullOrEmpty(Request["lastnameInput"])) value = Request["lastnameInput"].Trim();
            return HtmlUtil.GetText(value);
        }

        private string GetLastName(LoginProfile account)
        {
            var value = GetLastName();
            return String.IsNullOrEmpty(value) ? account.LastName : value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/confirminviteactivation/js/confirm_invite_activation.js"));

            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/confirminviteactivation/css/confirm_invite_activation.less"));

            _tenantInfoSettings = SettingsManager.Instance.LoadSettings<TenantInfoSettings>(TenantProvider.CurrentTenantID);

            var uid = Guid.Empty;
            try
            {
                uid = new Guid(Request["uid"]);
            }
            catch
            {
            }

            var email = GetEmailAddress();

            if (_type != ConfirmType.Activation && AccountLinkControl.IsNotEmpty)
            {
                var thrd = (AccountLinkControl) LoadControl(AccountLinkControl.Location);
                thrd.InviteView = true;
                thrd.ClientCallback = "loginJoinCallback";
                thrdParty.Visible = true;
                thrdParty.Controls.Add(thrd);
            }

            Page.Title = HeaderStringHelper.GetPageTitle(Resource.Authorization);

            UserInfo user;
            try
            {
                SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);

                user = CoreContext.UserManager.GetUserByEmail(email);
                var usr = CoreContext.UserManager.GetUsers(uid);
                if (usr.ID.Equals(ASC.Core.Users.Constants.LostUser.ID) || usr.ID.Equals(ASC.Core.Configuration.Constants.Guest.ID))
                    usr = CoreContext.UserManager.GetUsers(CoreContext.TenantManager.GetCurrentTenant().OwnerId);

                _userAvatar = usr.GetMediumPhotoURL();
                _userName = usr.DisplayUserName(true);
                _userPost = (usr.Title ?? "").HtmlEncode();
            }
            finally
            {
                SecurityContext.Logout();
            }

            if (_type == ConfirmType.LinkInvite || _type == ConfirmType.EmpInvite)
            {
                if (TenantStatisticsProvider.GetUsersCount() >= TenantExtra.GetTenantQuota().ActiveUsers && _employeeType == EmployeeType.User)
                {
                    ShowError(UserControlsCommonResource.TariffUserLimitReason);
                    return;
                }

                if (!user.ID.Equals(ASC.Core.Users.Constants.LostUser.ID))
                {
                    ShowError(CustomNamingPeople.Substitute<Resource>("ErrorEmailAlreadyExists"));
                    return;
                }
            }

            else if (_type == ConfirmType.Activation)
            {
                if (user.IsActive)
                {
                    ShowError(Resource.ErrorConfirmURLError);
                    return;
                }

                if (user.ID.Equals(ASC.Core.Users.Constants.LostUser.ID) || user.Status == EmployeeStatus.Terminated)
                {
                    ShowError(string.Format(Resource.ErrorUserNotFoundByEmail, email));
                    return;
                }
            }

            if (!IsPostBack)
                return;

            var firstName = GetFirstName();
            var lastName = GetLastName();
            var pwd = (Request["pwdInput"] ?? "").Trim();
            var repwd = (Request["repwdInput"] ?? "").Trim();
            LoginProfile thirdPartyProfile;

            //thirdPartyLogin confirmInvite
            if (Request["__EVENTTARGET"] == "thirdPartyLogin")
            {
                var valueRequest = Request["__EVENTARGUMENT"];
                thirdPartyProfile = new LoginProfile(valueRequest);

                if (!string.IsNullOrEmpty(thirdPartyProfile.AuthorizationError))
                {
                    // ignore cancellation
                    if (thirdPartyProfile.AuthorizationError != "Canceled at provider")
                        ShowError(HttpUtility.HtmlEncode(thirdPartyProfile.AuthorizationError));
                    return;
                }

                if (string.IsNullOrEmpty(thirdPartyProfile.EMail))
                {
                    ShowError(HttpUtility.HtmlEncode(Resource.ErrorNotCorrectEmail));
                    return;
                }
            }

            if (Request["__EVENTTARGET"] == "confirmInvite")
            {
                if (String.IsNullOrEmpty(email))
                {
                    _errorMessage = Resource.ErrorEmptyUserEmail;
                    return;
                }

                if (!email.TestEmailRegex())
                {
                    _errorMessage = Resource.ErrorNotCorrectEmail;
                    return;
                }

                if (String.IsNullOrEmpty(firstName))
                {
                    _errorMessage = Resource.ErrorEmptyUserFirstName;
                    return;
                }

                if (String.IsNullOrEmpty(lastName))
                {
                    _errorMessage = Resource.ErrorEmptyUserLastName;
                    return;
                }

                var checkPassResult = CheckPassword(pwd, repwd);
                if (!String.IsNullOrEmpty(checkPassResult))
                {
                    _errorMessage = checkPassResult;
                    return;
                }
            }
            var userID = Guid.Empty;
            try
            {
                SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                if (_type == ConfirmType.EmpInvite || _type == ConfirmType.LinkInvite)
                {
                    if (TenantStatisticsProvider.GetUsersCount() >= TenantExtra.GetTenantQuota().ActiveUsers && _employeeType == EmployeeType.User)
                    {
                        ShowError(UserControlsCommonResource.TariffUserLimitReason);
                        return;
                    }

                    UserInfo newUser;
                    if (Request["__EVENTTARGET"] == "confirmInvite")
                    {
                        var fromInviteLink = _type == ConfirmType.LinkInvite;
                        newUser = CreateNewUser(firstName, lastName, email, pwd, _employeeType, fromInviteLink);
                        userID = newUser.ID;
                    }

                    if (Request["__EVENTTARGET"] == "thirdPartyLogin")
                    {
                        if (!String.IsNullOrEmpty(CheckPassword(pwd, repwd)))
                            pwd = UserManagerWrapper.GeneratePassword();
                        var valueRequest = Request["__EVENTARGUMENT"];
                        thirdPartyProfile = new LoginProfile(valueRequest);
                        newUser = CreateNewUser(GetFirstName(thirdPartyProfile), GetLastName(thirdPartyProfile), GetEmailAddress(thirdPartyProfile), pwd, _employeeType, false);
                        userID = newUser.ID;
                        if (!String.IsNullOrEmpty(thirdPartyProfile.Avatar))
                            SaveContactImage(userID, thirdPartyProfile.Avatar);

                        var linker = new AccountLinker("webstudio");
                        linker.AddLink(userID.ToString(), thirdPartyProfile);
                    }
                }
                else if (_type == ConfirmType.Activation)
                {
                    user.ActivationStatus = EmployeeActivationStatus.Activated;
                    user.FirstName = firstName;
                    user.LastName = lastName;
                    CoreContext.UserManager.SaveUserInfo(user);
                    SecurityContext.SetUserPassword(user.ID, pwd);

                    userID = user.ID;

                    //notify
                    if (user.IsVisitor()) { 
                        StudioNotifyService.Instance.GuestInfoAddedAfterInvite(user, pwd);
                    }
                    else
                    {
                        StudioNotifyService.Instance.UserInfoAddedAfterInvite(user, pwd);
                    }
                }
            }
            catch (Exception exception)
            {
                _errorMessage = HttpUtility.HtmlEncode(exception.Message);
                return;
            }
            finally
            {
                SecurityContext.Logout();
            }

            try
            {
                var cookiesKey = SecurityContext.AuthenticateMe(userID.ToString(), pwd);
                CookiesManager.SetCookies(CookiesType.UserID, userID.ToString());
                CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);
                StudioNotifyService.Instance.UserHasJoin();
            }
            catch (Exception exception)
            {
                (Page as confirm).ErrorMessage = HttpUtility.HtmlEncode(exception.Message);
                return;
            }

            user = CoreContext.UserManager.GetUsers(userID);

            UserHelpTourHelper.IsNewUser = true;
            Response.Redirect(user.IsVisitor() ? "~/" : "~/welcome.aspx");
        }

        private static void SaveContactImage(Guid userID, string url)
        {
            using (var memstream = new MemoryStream())
            {
                var req = WebRequest.Create(url);
                var response = req.GetResponse();
                var stream = response.GetResponseStream();

                var buffer = new byte[512];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    memstream.Write(buffer, 0, bytesRead);
                var bytes = memstream.ToArray();

                UserPhotoManager.SaveOrUpdatePhoto(userID, bytes);
            }
        }

        private void ShowError(string message)
        {
            (Page as confirm).ErrorMessage = message.Trim();
            (Page as confirm)._confirmHolder2.Visible = false;
            (Page as confirm)._confirmHolder.Visible = false;
            (Page as confirm)._contentWithControl.Visible = true;

            if (SecurityContext.IsAuthenticated == false)
                (Page as confirm).ErrorMessage +=
                    (message.EndsWith(".") ? "" : ".")
                    + " "
                    + String.Format(Resource.ForSignInFollowMessage,
                                    string.Format("<a href=\"{0}\">",
                                                  VirtualPathUtility.ToAbsolute("~/auth.aspx")),
                                    "</a>");

            _confirmHolder.Visible = false;
        }

        private static string CheckPassword(string pwd, string repwd)
        {
            if (String.IsNullOrEmpty(pwd))
                return Resource.ErrorPasswordEmpty;

            try
            {
                UserManagerWrapper.CheckPasswordPolicy(pwd);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            if (!String.Equals(pwd, repwd))
                return Resource.ErrorMissMatchPwd;

            return String.Empty;
        }

        private static UserInfo CreateNewUser(string firstName, string lastName, string email, string pwd, EmployeeType employeeType, bool fromInviteLink)
        {
            var isVisitor = employeeType == EmployeeType.Visitor;

            string secretEmailPattern = ConfigurationManager.AppSettings["web.autotest.secret-email"];
            if (!string.IsNullOrEmpty(secretEmailPattern) && Regex.IsMatch(email, secretEmailPattern, RegexOptions.Compiled))
            {
                fromInviteLink = false;
            }

            var newUser = UserManagerWrapper.AddUser(new UserInfo
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    WorkFromDate = TenantUtil.DateTimeNow()
                }, pwd, true, true, isVisitor, fromInviteLink);

            return newUser;
        }
    }
}