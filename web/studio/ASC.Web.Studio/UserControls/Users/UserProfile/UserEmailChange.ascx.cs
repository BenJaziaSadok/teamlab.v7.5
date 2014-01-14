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
using System.Web.UI;
using ASC.Core.Users;
using ASC.Web.Studio.Core;
using System.Web;

namespace ASC.Web.Studio.UserControls.Users.UserProfile
{
    public partial class UserEmailChange : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Users/UserProfile/UserEmailChange.ascx"; }
        }

        public UserInfo UserInfo { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _emailChangerContainer.Options.IsPopup = true;
            AjaxPro.Utility.RegisterTypeForAjax(typeof(EmailOperationService));

            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/users/userprofile/css/userprofilecontrol_style.less"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/skins/default/toastr.css"));

            Page.RegisterBodyScripts(ResolveUrl("~/js/third-party/jquery/toastr.js"));
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/users/userprofile/js/userprofilecontrol.js"));
        }
    }
}