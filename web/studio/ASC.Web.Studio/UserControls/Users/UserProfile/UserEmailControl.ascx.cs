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
using ASC.Core.Users;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.UserControls.Users.UserProfile
{
    public partial class UserEmailControl : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Users/UserProfile/UserEmailControl.ascx"; } }

        /// <summary>
        /// The user represented by the control
        /// </summary>
        public UserInfo User { get; set; }

        /// <summary>
        /// The user who is viewing the page
        /// </summary>
        public UserInfo Viewer { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(typeof(EmailOperationService));
        }
    }
}