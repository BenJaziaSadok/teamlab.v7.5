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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Studio.Controls.Common
{
    [ToolboxData("<{0}:InlineScript></{0}:InlineScript>")]
    public class InlineScript : WebControl
    {
        public List<Tuple<string, bool>> Scripts { get; set; }

        public InlineScript()
        {
            Scripts = new List<Tuple<string, bool>>();
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            output.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            output.RenderBeginTag(HtmlTextWriterTag.Script);

            foreach (var script in Scripts.Where(r => !r.Item2))
            {
                output.Write(script.Item1);
            }

            var inlineScripts = Scripts.Where(r => r.Item2).Select(r => r.Item1).Distinct().ToList();

            if(!inlineScripts.Any()) return;

            output.Write("jq(document).ready(function(){");

            foreach (var script in inlineScripts)
            {
                output.Write(script);
            }

            output.Write("});");

            output.RenderEndTag();
        }
    }
}