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
using System.Web.UI;
using ASC.Web.Files.Classes;
using ASC.Web.Studio.UserControls.Common;

namespace ASC.Web.Files.Controls
{
    public partial class AccessRights : UserControl
    {
        public bool IsPopup = true;

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("AccessRights/AccessRights.ascx"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/products/files/Controls/AccessRights/accessrights.js"));

            var sharingSetting = (SharingSettings) LoadControl(SharingSettings.Location);
            sharingSetting.EnableShareMessage = true;
            sharingSetting.IsPopup = IsPopup;
            _sharingContainer.Controls.Add(sharingSetting);

            confirmUnsubscribeDialog.Options.IsPopup = true;
        }
    }
}