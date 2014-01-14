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

#region Import

using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

#endregion

namespace ASC.Web.Projects.Controls
{
    public class ElementNotFoundControl : WebControl
    {
        #region Property

        public String Header { get; set; }
        public String Body { get; set; }
        public String RedirectURL { get; set; }
        public String RedirectTitle { get; set; }

        #endregion

        protected override void Render(HtmlTextWriter writer)
        {

            StringBuilder innerHTML = new StringBuilder("<div>");

            innerHTML.AppendFormat(@"<div class='pm-elementNotFoundControl-header'>{0}</div>", Header);
            innerHTML.AppendFormat(@"<div>{0}</div>", Body);
            innerHTML.AppendFormat(@"<div style='margin-top: 20px;'><a href='{0}'>{1}</a></div>", RedirectURL, RedirectTitle);

            innerHTML.Append("</div>");

            writer.WriteLine(innerHTML.ToString());
            
        }
    }
}
