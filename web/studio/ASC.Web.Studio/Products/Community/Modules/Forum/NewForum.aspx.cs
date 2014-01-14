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
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using ASC.Web.Studio;
using AjaxPro;
using ASC.Forum;
using System.Collections.Generic;
using ASC.Web.Studio.Utility;
using System.Text;

namespace ASC.Web.Community.Forum
{
    public partial class NewForum : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ForumManager.Instance.ValidateAccessSecurityAction(ForumAction.GetAccessForumEditor, null))
            {
                Response.Redirect(ForumManager.Instance.PreviousPage.Url);
                return;
            }

            Utility.RegisterTypeForAjax(typeof(ForumMaker));
        }

        public string GetCategoryList()
        {
            var categories = new List<ThreadCategory>();
            var threads = new List<Thread>();
            ForumDataProvider.GetThreadCategories(TenantProvider.CurrentTenantID, out categories, out threads);

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='headerPanelSmall-splitter'><b>" + Resources.ForumResource.ThreadCategory + ":</b></div>");
            sb.Append("<select id='forum_fmCategoryID' onchange=\"ForumMakerProvider.SelectCategory();\" style='width:100%;' class='comboBox'>");
            sb.Append("<option value='-1'>" + Resources.ForumResource.TypeCategoryName + "</option>");
            foreach (var categ in categories)
            {
                sb.Append("<option value='" + categ.ID + "'>" + categ.Title.HtmlEncode() + "</option>");
            }
            sb.Append("</select>");
            return sb.ToString();
        }
    }
}
