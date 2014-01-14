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
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using System.Web.UI.HtmlControls;
using System.Web;

namespace ASC.Web.Studio.UserControls.FirstTime
{
    public partial class StepContainer : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/FirstTime/StepContainer.ascx"; } }

        public Control ChildControl { get; set; }
        public string SaveButtonEvent { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
            
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/firsttime/js/view.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/firsttime/css/stepcontainer.less"));

            SaveButtonEvent = "ASC.Controls.FirstTimeView.SaveRequiredStep();";
            ChildControl = LoadControl(EmailAndPassword.Location);
            if (ChildControl != null)
            {
                content1.Controls.Add(ChildControl);
            }
        }
    }
}