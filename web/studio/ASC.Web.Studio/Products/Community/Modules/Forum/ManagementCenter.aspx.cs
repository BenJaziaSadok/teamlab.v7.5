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
using ASC.Web.Studio;
using ASC.Web.UserControls.Forum.Common;

namespace ASC.Web.Community.Forum
{

    public partial class ManagementCenter : MainPage
    {      
        protected void Page_Load(object sender, EventArgs e)
        {
            ForumManager.Instance.SetCurrentPage(ForumPage.ManagementCenter);
            Control managementControl = LoadControl(ForumManager.BaseVirtualPath + "/UserControls/ForumEditor.ascx");                   
            controlPanel.Controls.Add(managementControl);
        }
    }
}