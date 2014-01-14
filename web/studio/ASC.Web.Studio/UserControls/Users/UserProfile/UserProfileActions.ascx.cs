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
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using System.Web;

namespace ASC.Web.Studio.UserControls.Users
{
    public partial class UserProfileActions : System.Web.UI.UserControl
    {
        public ProfileHelper profileHelper { get; set; }
        public bool MyStaff { get; set; }
        public AllowedActions Actions { get; set; }

        public string profileEditLink { get; set; }


        public static string Location
        {
            get { return "~/UserControls/Users/UserProfile/UserProfileActions.ascx"; }
        }

        protected bool UserHasAvatar
        {
            get { return !UserPhotoManager.GetPhotoAbsoluteWebPath(profileHelper.userProfile.Id).Contains("default/images/"); }
        }

        protected bool HasActions
        {
            get
            {
                return Actions.AllowEdit || Actions.AllowAddOrDelete;
            }
        }
        public bool IsAdmin
        {
            get { return CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsAdmin(); }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (HasActions && Actions.AllowAddOrDelete)
            {
                _phConfirmationDeleteUser.Controls.Add(LoadControl(ConfirmationDeleteUser.Location));
            }

            if (Request.Url.AbsolutePath.IndexOf("my.aspx") > 0)
            {
                profileEditLink = "/my.aspx?action=edit";
            }
            else
            {
                profileEditLink = "profileaction.aspx?action=edit&user=" + HttpUtility.UrlEncode(profileHelper.UserInfo.UserName);
            }
        }
    }
}