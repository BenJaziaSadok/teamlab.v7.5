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
using ASC.Web.Studio.Core;
using AjaxPro;
using ASC.Web.Core.Security;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("MySettings")]
    public partial class PwdTool : System.Web.UI.UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/PwdTool.ascx"; }
        }

        public Guid UserID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _pwdRemainderContainer.Options.IsPopup = true;
            _pwdRemainderContainer.Options.InfoMessageText = "";
            _pwdRemainderContainer.Options.InfoType = InfoType.Info;

            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
        }

        [SecurityPassthroughAttribute]
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse RemindPwd(string email)
        {
            var responce = new AjaxResponse { rs1 = "0" };

            if (!email.TestEmailRegex())
            {
                responce.rs2 = "<div>" + Resources.Resource.ErrorNotCorrectEmail + "</div>";
                return responce;
            }

            try
            {
                UserManagerWrapper.SendUserPassword(email);

                responce.rs1 = "1";
                responce.rs2 = String.Format(Resources.Resource.MessageYourPasswordSuccessfullySendedToEmail, email);
            }
            catch (Exception exc)
            {
                responce.rs2 = "<div>" + HttpUtility.HtmlEncode(exc.Message) + "</div>";
            }

            return responce;
        }
    }
}