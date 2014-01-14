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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ASC.SocialMedia.Facebook;

namespace ASC.Web.CRM.Controls.SocialMedia
{
    public partial class ListFacebookUserInfoView : System.Web.UI.UserControl
    {
        public List<FacebookUserInfo> UserInfoCollection { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserInfoCollection != null && UserInfoCollection.Count > 0)
            {
                BindRepeater();
                ShowRepeater();
            }
            else
                ShowNotFoundText();

        }

        private void BindRepeater()
        {
            _ctrlRptrUsers.DataSource = UserInfoCollection;
            _ctrlRptrUsers.DataBind();

        }

        private void ShowRepeater()
        {
            _ctrlDivNotFound.Visible = false;
            _ctrlRptrUsers.Visible = true;
        }

        private void ShowNotFoundText()
        {
            _ctrlDivNotFound.Visible = true;
            _ctrlRptrUsers.Visible = false;
        }

        protected void _ctrlRptrUsers_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            string userID = ((FacebookUserInfo)e.Item.DataItem).UserID;
            string _ctrlRelateAccountScript = String.Format("ASC.CRM.SocialMedia.ShowAccountRelationPanel('{0}','{1}'); return false;", userID, "facebook");
            ((HtmlAnchor)e.Item.FindControl("_ctrlRelateContactWithAccount")).Attributes.Add("onclick", _ctrlRelateAccountScript);
        }
    }
}