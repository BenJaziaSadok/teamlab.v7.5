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
using ASC.SocialMedia;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web.UI;

namespace ASC.Web.UserControls.SocialMedia.UserControls
{
    public partial class ListActivityMessageView : BaseUserControl
    {
        public List<Message> MessageList { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (MessageList != null)
            {
                _ctrlRptrUserActivity.DataSource = MessageList;
                _ctrlRptrUserActivity.DataBind();
            }
        }

        protected void _ctrlRptrUserActivity_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Message msg = (Message)e.Item.DataItem;
            ((Image)e.Item.FindControl("_ctrlImgSocialMediaIcon")).ImageUrl = Page.ClientScript.GetWebResourceUrl(typeof(BaseUserControl), String.Format("ASC.Web.UserControls.SocialMedia.images.{0}.png", msg.Source.ToString().ToLower()));

        }
    }
}