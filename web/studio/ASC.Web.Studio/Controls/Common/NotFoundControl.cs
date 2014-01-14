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

namespace ASC.Web.Studio.Controls.Common
{
    public class NotFoundControl : LiteralControl
    {
        public string LinkFormattedText { get; set; }

        public string LinkURL { get; set; }

        public bool HasLink { get; set; }

        public NotFoundControl()
        {
            Text = Resources.Resource.SearchNotFoundMessage;
        }

        protected override void Render(HtmlTextWriter output)
        {
            var sb = new StringBuilder("<div class=\"noContentBlock\">");

            sb.Append(Text);
            if (HasLink && !String.IsNullOrEmpty(LinkURL) && !String.IsNullOrEmpty(LinkFormattedText))
                sb.AppendFormat("&nbsp;" + LinkFormattedText, LinkURL);

            sb.Append("</div>");
            output.WriteLine(sb.ToString());
        }
    }
}