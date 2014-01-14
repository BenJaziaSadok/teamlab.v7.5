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
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.UserControls.Bookmarking.Common
{
    [ToolboxData("<{0}:ActionButton runat=server></{0}:ActionButton>")]
    public class ActionButton : WebControl
    {
        public string ButtonCssClass { get; set; }

        public string ButtonID { get; set; }

        public string ButtonStyle { get; set; }

        public string ButtonText { get; set; }

        public string AjaxRequestText { get; set; }

        public string OnClickJavascript { get; set; }

        public bool EnableRedirectAfterAjax { get; set; }

        public bool DisableInputs { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(ButtonID))
            {
                ButtonID = Guid.NewGuid().ToString();
            }

            sb.AppendFormat("<div style='display: none;' id='{0}AjaxRequestPanel'>", ButtonID);

            sb.AppendFormat("<div class='text-medium-describe'>{0}</div>", AjaxRequestText);

            sb.AppendFormat("<img src='{0}' />", WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif", BookmarkingSettings.ModuleId));

            sb.AppendFormat("</div>");

            sb.AppendFormat(@"<a class='{0}' id='{1}' href='javascript:void(0);' onclick='actionButtonClick(this.id, {5}); {2}' style='{3}'>{4}</a>",
                            ButtonCssClass, ButtonID, OnClickJavascript, ButtonStyle, ButtonText, EnableRedirectAfterAjax.ToString().ToLower());

            writer.Write(sb);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/products/community/modules/bookmarking/UserControls/Common/ActionButton/js/actionbutton.js"));
        }
    }
}