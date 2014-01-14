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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Studio.Controls.Common
{
    public class EmptyScreenControl : WebControl
    {
        public string ImgSrc { get; set; }

        public string Header { get; set; }

        public string HeaderDescribe { get; set; }

        public string Describe { get; set; }

        public string ButtonHTML { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            var html = new StringBuilder();

            html.AppendFormat("<div id='{0}' class='noContentBlock emptyScrCtrl {1}' >", ID, !String.IsNullOrEmpty(CssClass)? CssClass : String.Empty);
            if (!String.IsNullOrEmpty(ImgSrc))
            {
                html.AppendFormat("<table><tr><td><div style=\"background-image: url('{0}');\" class=\"emptyScrImage\" ></div></td>", ImgSrc)
                    .Append("<td><div class='emptyScrTd' >");
            }
            if (!String.IsNullOrEmpty(Header))
            {
                html.AppendFormat("<div class='header-base-big' >{0}</div>", Header);
            }
            if (!String.IsNullOrEmpty(HeaderDescribe))
            {
                html.AppendFormat("<div class='emptyScrHeadDscr' >{0}</div>", HeaderDescribe);
            }
            if (!String.IsNullOrEmpty(Describe))
            {
                html.AppendFormat("<div class='emptyScrDscr' >{0}</div>", Describe);
            }
            if (!String.IsNullOrEmpty(ButtonHTML))
            {
                html.AppendFormat("<div class='emptyScrBttnPnl' >{0}</div>", ButtonHTML);
            }
            if (!String.IsNullOrEmpty(ImgSrc))
            {
                html.Append("</div></td></tr></table>");
            }

            html.Append("</div>");

            writer.WriteLine(html.ToString());
        }
    }
}