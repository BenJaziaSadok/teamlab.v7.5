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
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.Core.Users;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Profile;
using ASC.Web.Core;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Common
{
    public partial class LoginWithThirdParty : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Common/LoginWithThirdParty.ascx"; }
        }

        public bool FromEditor;

        private string _loginMessage;

        protected string LoginMessage
        {
            get
            {
                return string.IsNullOrEmpty(_loginMessage)
                           ? string.Empty
                           : "<div class=\"errorBox\">" + _loginMessage.HtmlEncode() + "</div>";
            }
            set { _loginMessage = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var accountLink = (AccountLinkControl) LoadControl(AccountLinkControl.Location);
            accountLink.ClientCallback = "loginJoinCallback";
            accountLink.SettingsView = false;
            ThirdPartyList.Controls.Add(accountLink);

            _loginMessage = Request["m"];

            if (!IsPostBack || SecurityContext.IsAuthenticated) return;

            try
            {
                if (string.IsNullOrEmpty(Request["__EVENTARGUMENT"]) || Request["__EVENTTARGET"] != "thirdPartyLogin")
                {
                    LoginMessage = "<div class=\"errorBox\">" + HttpUtility.HtmlEncode(Resources.Resource.InvalidUsernameOrPassword) + "</div>";
                    return;
                }

                var valueRequest = Request["__EVENTARGUMENT"];

                var thirdPartyProfile = new LoginProfile(valueRequest);

                if (!string.IsNullOrEmpty(thirdPartyProfile.AuthorizationError))
                {
                    // ignore cancellation
                    if (thirdPartyProfile.AuthorizationError != "Canceled at provider")
                    {
                        _loginMessage = thirdPartyProfile.AuthorizationError;
                    }
                    return;
                }

                if (string.IsNullOrEmpty(thirdPartyProfile.EMail))
                {
                    _loginMessage = Resources.Resource.ErrorNotCorrectEmail;
                    return;
                }

                var cookiesKey = string.Empty;
                var accounts = accountLink.GetLinker().GetLinkedObjectsByHashId(thirdPartyProfile.HashId);

                foreach (var account in accounts.Select(x =>
                                                            {
                                                                try
                                                                {
                                                                    return new Guid(x);
                                                                }
                                                                catch
                                                                {
                                                                    return Guid.Empty;
                                                                }
                                                            }))
                {
                    if (account == Guid.Empty || !CoreContext.UserManager.UserExists(account)) continue;

                    var coreAcc = CoreContext.UserManager.GetUsers(account);
                    cookiesKey = SecurityContext.AuthenticateMe(coreAcc.ID);
                }

                if (string.IsNullOrEmpty(cookiesKey))
                {
                    var emailAcc = CoreContext.UserManager.GetUserByEmail(thirdPartyProfile.EMail);
                    if (CoreContext.UserManager.UserExists(emailAcc.ID))
                    {
                        cookiesKey = SecurityContext.AuthenticateMe(emailAcc.ID);
                    }
                }

                if (CoreContext.Configuration.YourDocsDemo && string.IsNullOrEmpty(cookiesKey))
                {
                    cookiesKey = JoinByThirdPartyAccount(thirdPartyProfile);
                }

                CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);
            }
            catch (System.Security.SecurityException)
            {
                Auth.ProcessLogout();
                _loginMessage = Resources.Resource.InvalidUsernameOrPassword;
                return;
            }
            catch (Exception exception)
            {
                Auth.ProcessLogout();
                _loginMessage = exception.Message;
                return;
            }

            var refererURL = (string) Session["refererURL"];

            if (String.IsNullOrEmpty(refererURL))
                Response.Redirect("~/");
            else
            {
                Session["refererURL"] = null;
                Response.Redirect(refererURL);
            }
        }

        private static string JoinByThirdPartyAccount(LoginProfile thirdPartyProfile)
        {
            var userInfo = new UserInfo
                               {
                                   Status = EmployeeStatus.Active,
                                   FirstName = string.IsNullOrEmpty(thirdPartyProfile.FirstName) ? Resources.UserControlsCommonResource.UnknownFirstName : thirdPartyProfile.FirstName,
                                   LastName = string.IsNullOrEmpty(thirdPartyProfile.LastName) ? Resources.UserControlsCommonResource.UnknownLastName : thirdPartyProfile.LastName,
                                   Email = thirdPartyProfile.EMail,
                                   Title = string.Empty,
                                   Location = string.Empty,
                                   WorkFromDate = ASC.Core.Tenants.TenantUtil.DateTimeNow(),
                               };

            var pwd = UserManagerWrapper.GeneratePassword();

            UserInfo newUserInfo;
            try
            {
                SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                SecurityContext.DemandPermissions(Constants.Action_AddRemoveUser);
                newUserInfo = UserManagerWrapper.AddUser(userInfo, pwd);
            }
            finally
            {
                SecurityContext.Logout();
            }

            var linker = new AccountLinker("webstudio");
            linker.AddLink(newUserInfo.ID.ToString(), thirdPartyProfile);

            return SecurityContext.AuthenticateMe(newUserInfo.ID);
        }
    }
}