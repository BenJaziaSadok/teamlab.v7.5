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
using ASC.Web.CRM.Resources;
using ASC.SocialMedia.Facebook;

namespace ASC.Web.CRM.Controls.SocialMedia
{
    public partial class FacebookUserInfoView : System.Web.UI.UserControl
    {
        public FacebookUserInfo UserInfo { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {            
            _ctrlChbAddImage.Text = CRMSocialMediaResource.RelateAvatar;
            _ctrlBtRelate.Value = CRMSocialMediaResource.Relate;

            _ctrlImageUserAvatar.ImageUrl = UserInfo.SmallImageUrl.Replace("small", "large");
            _ctrlUserName.InnerText = UserInfo.UserName;            

            _ctrlHiddenContactID.Value = "";
            _ctrlHiddenFacebookUserID.Value = UserInfo.UserID;
            _ctrlHiddenUserAvatarUrl.Value = UserInfo.SmallImageUrl.Replace("small", "large");
        }
    }
}