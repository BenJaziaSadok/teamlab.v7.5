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
using ASC.SocialMedia.Twitter;
using ASC.Web.CRM.Resources;

namespace ASC.Web.CRM.Controls.SocialMedia
{
    public partial class TwitterUserInfoView : System.Web.UI.UserControl
    {
        public TwitterUserInfo UserInfo { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _ctrlChbRelateToAccount.Text = CRMSocialMediaResource.RelateAccountToContact;
            _ctrlChbAddImage.Text = CRMSocialMediaResource.RelateAvatar;
            _ctrlBtRelate.Value = CRMSocialMediaResource.Relate;

            _ctrlImageUserAvatar.ImageUrl = UserInfo.SmallImageUrl.Replace("_normal", "");
            _ctrlUserName.InnerText = UserInfo.UserName;
            _ctrlUserDescription.InnerText = UserInfo.Description;

            _ctrlHiddenContactID.Value = "";
            _ctrlHiddenTwitterUserID.Value = UserInfo.UserID.ToString();
            _ctrlHiddenTwitterUserScreenName.Value = UserInfo.ScreenName;
            _ctrlHiddenUserAvatarUrl.Value = UserInfo.SmallImageUrl.Replace("_normal", "");
        }
    }
}