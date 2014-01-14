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
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using AjaxPro;
using ASC.Web.Studio.Core.Notify;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.Studio.UserControls.Users
{
    [AjaxNamespace("InviteResender")]
    public partial class ResendInvitesControl : UserControl
    {
        public static string Location
        {
            get { return "~/usercontrols/users/resendinvitescontrol/resendinvitescontrol.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/users/resendinvitescontrol/js/resendinvitescontrol.js"));

            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            _invitesResenderContainer.Options.IsPopup = true;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object Resend()
        {
            try
            {
                foreach (var user in new List<UserInfo>(CoreContext.UserManager.GetUsers())
                    .FindAll(u => u.ActivationStatus == EmployeeActivationStatus.Pending))
                {
                    if (user.IsVisitor())
                    {
                        StudioNotifyService.Instance.GuestInfoActivation(user);
                    }
                    else
                    {
                        StudioNotifyService.Instance.UserInfoActivation(user);
                    }
                }

                return new { status = 1, message = Resources.Resource.SuccessResendInvitesText };
            }
            catch (Exception e)
            {
                return new { status = 0, message = e.Message.HtmlEncode() };
            }
        }

        public static string GetHrefAction()
        {
            return "javascript:InvitesResender.Show();";
        }
    }
}