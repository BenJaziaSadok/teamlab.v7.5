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
using System.ServiceModel.Security;
using System.Web;
using System.Web.UI;
using ASC.Core.Common.Logging;
using ASC.Core.Users;
using AjaxPro;
using ASC.Core;
using ASC.Web.Studio.Core.Notify;
using Resources;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("AjaxPro.ChangeMobileNumber")]
    public partial class ChangeMobileNumber : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/SmsControls/ChangeMobileNumber.ascx"; }
        }

        public UserInfo User;

        protected void Page_Load(object sender, EventArgs e)
        {
            _changePhoneContainer.Options.IsPopup = true;

            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/smscontrols/js/changemobile.js"));

            AjaxPro.Utility.RegisterTypeForAjax(GetType());
        }

        [AjaxMethod]
        public string SendNotificationToChange(string userId)
        {
            var user = CoreContext.UserManager.GetUsers(
                string.IsNullOrEmpty(userId)
                    ? SecurityContext.CurrentAccount.ID
                    : new Guid(userId));

            var canChange =
                user.IsMe()
                || SecurityContext.CheckPermissions(new UserSecurityProvider(user.ID), ASC.Core.Users.Constants.Action_EditUser);

            if (!canChange)
                throw new SecurityAccessDeniedException(Resource.ErrorAccessDenied);

            user.MobilePhoneActivationStatus = MobilePhoneActivationStatus.NotActivated;
            CoreContext.UserManager.SaveUserInfo(user);

            if (user.IsMe())
            {
                return StudioNotifyService.GenerateConfirmUrl(user.Email, ConfirmType.PhoneActivation);
            }

            AdminLog.PostAction("UserProfile: erase phone number for user with id {0}", user.ID);
            StudioNotifyService.Instance.SendMsgMobilePhoneChange(user);
            return string.Empty;
        }
    }
}