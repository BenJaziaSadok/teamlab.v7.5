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

namespace ASC.Web.Studio.UserControls.Common.TabsNavigator
{
    public class TabsNavigatorItem : Control
    {
        public string OnClickText { get; set; }

        public string TabName { get; set; }

        public string TabAnchorName { get; set; }

        public string TabHref { get; set; }

        public bool IsSelected { get; set; }

        public string DivID = Guid.NewGuid().ToString();

        public bool SkipRender { get; set; }

        public string GetTabLink(bool isLast)
        {
            var tabCssName = String.IsNullOrEmpty(TabAnchorName)
                                 ? (IsSelected ? "tabsNavigationLink selectedTab" : "tabsNavigationLink")
                                 : String.Format("{0} tabsNavigationLink_{1}",
                                                 IsSelected ? "tabsNavigationLink selectedTab" : "tabsNavigationLink", TabAnchorName);

            var href = String.IsNullOrEmpty(TabHref) || IsSelected ? "" : String.Format(" href='{0}'", TabHref);

            var javascriptText = String.IsNullOrEmpty(TabHref)
                                     ? String.Format(" onclick=\"{0} ASC.Controls.TabsNavigator.toggleTabs(this.id, '{1}');\"",
                                                     String.IsNullOrEmpty(OnClickText) ? String.Empty : OnClickText + ";",
                                                     TabAnchorName)
                                     : String.Empty;

            var sb = new StringBuilder();
            sb.AppendFormat("<a id='{0}_tab' class='{1}'{2} {3}>{4}</a>",
                            DivID, tabCssName, href, javascriptText, TabName);
            if (!isLast)
            {
                sb.AppendFormat("<span class=\"splitter\"></span>");
            }

            return sb.ToString();
        }

        public void RenderTabContent(HtmlTextWriter writer)
        {
            writer.Write("<div id='{0}'{1}>", DivID, IsSelected ? string.Empty : " class='display-none'");
            RenderControl(writer);
            writer.Write("</div>");
        }
    }
}