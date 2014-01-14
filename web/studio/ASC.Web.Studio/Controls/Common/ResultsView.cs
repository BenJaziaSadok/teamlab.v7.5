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
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Studio.Utility.HtmlUtility;

namespace ASC.Web.Studio.Controls.Common
{
    public class ResultsView : ItemSearchControl
    {
        protected override void RenderContents(HtmlTextWriter writer)
        {
            foreach (var srGroup in Items.GetRange(0, (MaxCount < Items.Count) ? MaxCount : Items.Count))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "clearFix item");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, String.IsNullOrEmpty(srGroup.Description) ? "widebody" : "body");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, srGroup.URL);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "link bold");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(HtmlUtility.SearchTextHighlight(Text, srGroup.Name.HtmlEncode(), false));
                writer.RenderEndTag();

                if (!String.IsNullOrEmpty(srGroup.Description))
                {
                    writer.WriteBreak();

                    if (!string.IsNullOrEmpty(SpanClass))
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, SpanClass);
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write(CheckEmptyValue(srGroup.Description.HtmlEncode()));
                    writer.RenderEndTag();

                }
                writer.RenderEndTag();

                if (srGroup.Date.HasValue)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "date");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Style, "height:100%");
                    writer.RenderBeginTag(HtmlTextWriterTag.Table);
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);

                    writer.Write(srGroup.Date.Value.ToShortDateString());

                    writer.RenderEndTag();
                    writer.RenderEndTag();
                    writer.RenderEndTag();

                    writer.RenderEndTag();
                }
                writer.RenderEndTag();
            }
        }
    }
}