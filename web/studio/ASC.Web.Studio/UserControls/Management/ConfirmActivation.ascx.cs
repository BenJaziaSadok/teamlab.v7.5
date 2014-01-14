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
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class ConfirmActivation : UserControl
    {
        public static string Location { get { return "~/UserControls/Management/ConfirmActivation.ascx"; } }

        protected UserInfo User { get; set; }

        protected ConfirmType Type
        {
            get { return (ConfirmType)ViewState["type"]; }
            set { ViewState["type"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ButtonEmailAndPasswordOK.Text = Resources.Resource.EmailAndPasswordOK;
            btChangeEmail.Text = Resources.Resource.ChangeEmail;

            var email = (Request["email"] ?? "").Trim();
            if (string.IsNullOrEmpty(email))
            {
                ShowError(Resources.Resource.ErrorNotCorrectEmail);
                return;
            }

            var type = typeof(ConfirmType).TryParseEnum(Request["type"] ?? "", ConfirmType.EmpInvite);
            Type = type;//Save to viewstate
            
            User = CoreContext.UserManager.GetUserByEmail(email);
            if (User.ID.Equals(Constants.LostUser.ID))
            {
                //Error. User doesn't exists.
                ShowError(string.Format(Resources.Resource.ErrorUserNotFoundByEmail, email));
                return;
            }
           
            Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Authorization);
            if (type == ConfirmType.EmailChange && !IsPostBack)
            {
                //If it's email confirmation then just activate
                emailChange.Visible = true;
                ActivateUser(User);
                AjaxPro.Utility.RegisterTypeForAjax(typeof(EmailOperationService));
                //RegisterRedirect();
            }
            else if (type == ConfirmType.PasswordChange)
            {
                passwordSetter.Visible = true;
            }

            RegisterScript();
        }

        private void ActivateUser(UserInfo user)
        {
            ActivateUser(user, null);
        }
        
        private void ActivateUser(UserInfo user, string newPwd)
        {
            try
            {
                //Set status to activated
                user.ActivationStatus = EmployeeActivationStatus.Activated;
                SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                CoreContext.UserManager.SaveUserInfo(user);
                if (!string.IsNullOrEmpty(newPwd))
                {
                    //set password if it's specified
                    SecurityContext.SetUserPassword(user.ID, newPwd);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                SecurityContext.Logout(); //Logout from core system
            }
            //Login user
            try
            {
                var cookiesKey = SecurityContext.AuthenticateMe(user.ID);
                CookiesManager.SetCookies(CookiesType.UserID, user.ID.ToString());
                CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);
               
            }
            catch (Exception exception)
            {
                ShowError(exception.Message);
                return;
            }
        }

        private void ShowError(string message)
        {
            ShowError(message, true);
        }

        private void ShowError(string message, bool redirect)
        {
            var confirm = Page as confirm;
            if (confirm != null)
                confirm.ErrorMessage = HttpUtility.HtmlEncode(message);

            //Logout all users. Ibo nehui
            SecurityContext.Logout();
            CookiesManager.ClearCookies(CookiesType.AuthKey);

            //Register redirect script
            if (redirect)
                RegisterRedirect();
        }

        private void RegisterRedirect()
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "redirect",
                                                    string.Format("setTimeout('location.href = \"{0}\";',10000);",
                                                                  CommonLinkUtility.GetFullAbsolutePath("~/")), true);
        }

        protected void LoginToPortal(object sender, EventArgs e)
        {
            try
            {
                var pwd = Request.Form["pwdInput"];
                var repwd = Request.Form["repwdInput"];
                //Validate Password match
                UserManagerWrapper.CheckPasswordPolicy(pwd);
                if (string.Compare(pwd, repwd) != 0)
                {
                    throw new ArgumentException(Resources.Resource.ErrorMissMatchPwd);
                }
                ActivateUser(User, pwd);
                Response.Redirect(VirtualPathUtility.ToAbsolute("~/"));
            }
            catch (ThreadAbortException)
            {
                //Thread aborted in redirect! Do nothing
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, false);
            }
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"
                    window.btChangeEmailOnClick = function() {{
                        var oldEmail = '{0}';
                        var newEmail = jq('#email1').val();
                        var newEmailConfirm = jq('#email2').val();
                        var queryString = location.search;
                        EmailOperationManager.SendEmailActivationInstructionsOnChange(oldEmail, newEmail, newEmailConfirm, queryString);
                    }}",
                User.Email
            );

            Page.RegisterInlineScript(sb.ToString());
        }
    }
}