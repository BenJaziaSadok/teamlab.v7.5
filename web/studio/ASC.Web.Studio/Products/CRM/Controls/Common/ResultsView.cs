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
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Utility.HtmlUtility;

namespace ASC.Web.CRM.Controls.Common
{
    public sealed class ResultsView : ItemSearchControl
    {

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "tableBase");
            writer.AddAttribute("cellspacing", "0");
            writer.AddAttribute("cellpadding", "8");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            writer.RenderBeginTag(HtmlTextWriterTag.Tbody);

            foreach (var searchItemResult in Items.GetRange(0, (MaxCount < Items.Count) ? MaxCount : Items.Count))
            {
                var relativeInfo = searchItemResult.Additional["relativeInfo"].ToString();

                if (String.IsNullOrEmpty(relativeInfo))
                    relativeInfo = searchItemResult.Description.HtmlEncode();
                else
                    relativeInfo = String.Format("<span class='describe-text'>{0}</span> {1}", CRMCommonResource.RelativeTo, relativeInfo.HtmlEncode());

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "search-result-item");
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "borderBase left-column gray-text");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.AddAttribute(HtmlTextWriterAttribute.Title, searchItemResult.Additional["typeInfo"].ToString());
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(searchItemResult.Additional["typeInfo"].ToString());
                writer.RenderEndTag();
                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "borderBase center-column");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);

                writer.AddAttribute(HtmlTextWriterAttribute.Href, searchItemResult.URL);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "link bold");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(HtmlUtility.SearchTextHighlight(Text, searchItemResult.Name.HtmlEncode(), false));
                writer.RenderEndTag();

                if (!String.IsNullOrEmpty(relativeInfo))
                {
                    writer.WriteBreak();
                    writer.Write(relativeInfo);
                }

                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "borderBase right-column gray-text");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                if (searchItemResult.Date.HasValue)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Title, searchItemResult.Date.Value.ToShortDateString());
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.Write(searchItemResult.Date.Value.ToShortDateString());
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();

                writer.RenderEndTag();
            }

            writer.RenderEndTag();
            writer.RenderEndTag();
        }
    }
}