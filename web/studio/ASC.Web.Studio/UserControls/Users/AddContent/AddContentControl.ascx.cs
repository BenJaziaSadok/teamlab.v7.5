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
using System.Web;
using ASC.Data.Storage;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;
using System.Web.UI.HtmlControls;

namespace ASC.Web.Studio.UserControls.Users
{
    public partial class AddContentControl : System.Web.UI.UserControl
    {
      public sealed class ContentTypes
      {
          public string Link { get; set; }
          public string Icon { get; set; }
          public string Label { get; set; }
        }

        public static string Location
        {
          get { return "~/UserControls/Users/AddContent/AddContentControl.ascx"; }
        }

        public List<ContentTypes> Types { get; set; }

        public AddContentControl()
        {
            Types = new List<ContentTypes>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/users/addcontent/css/addcontentcontrol_style.less"));

            AddContentContainer.Options.IsPopup = true;

            ContentTypesRepeater.DataSource = Types;
            ContentTypesRepeater.DataBind();
        }
    }
}
