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
using System.ComponentModel;
using System.Web;
using System.Web.UI;

namespace ASC.Web.Studio.Controls.Common
{
    public enum HighlightStyle
    {
        Ascetic,
        Dark,
        Default,
        Far,
        Idea,
        Magula,
        Sunburst,
        VS,
        Zenburn
    }

    [ToolboxData("<{0}:CodeHighlighter runat=server></{0}:CodeHighlighter>")]
    public class CodeHighlighter : Control
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(HighlightStyle), "VS")]
        [Localizable(true)]
        public HighlightStyle HighlightStyle
        {
            get
            {
                var hs = ViewState["HighlightStyle"];
                return (hs == null ? HighlightStyle.VS : (HighlightStyle)hs);
            }
            set { ViewState["HighlightStyle"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Page.RegisterBodyScripts(ResolveUrl("~/js/third-party/highlight.pack.js"));
            Page.RegisterInlineScript("hljs.initHighlightingOnLoad();");

            var cssFileName = HighlightStyle.ToString().ToLowerInvariant();
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/app_themes/codehighlighter/" + cssFileName + ".css"));
        }

        public static string GetJavaScriptLiveHighlight()
        {
            return GetJavaScriptLiveHighlight(false);
        }

        public static string GetJavaScriptLiveHighlight(bool addScriptTags)
        {
            const string script = "jq(document).ready(function(){jq('code').each(function(){ hljs.highlightBlock(jq(this).get(0));});});";
            return
                addScriptTags
                    ? string.Format("<script language='javascript' type='text/javascript'>{0}</script>", script)
                    : script;
        }
    }
}