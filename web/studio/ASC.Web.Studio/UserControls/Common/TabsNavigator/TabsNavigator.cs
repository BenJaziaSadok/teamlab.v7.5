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
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Linq;

namespace ASC.Web.Studio.UserControls.Common.TabsNavigator
{
    [
        ToolboxData("<{0}:TabsNavigator runat=\"server\"/>"),
        ParseChildren(ChildrenAsProperties = true), PersistChildren(true)
    ]
    public class TabsNavigator : Control
    {
        #region Properties

        public string BlockID { get; set; }


        private List<TabsNavigatorItem> _tabItems;

        [Description("List of tabs."),
         Category("Data"), PersistenceMode(PersistenceMode.InnerProperty),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)
        ]
        public List<TabsNavigatorItem> TabItems
        {
            get { return _tabItems ?? (_tabItems = new List<TabsNavigatorItem>()); }
            set { _tabItems = value; }
        }

        protected bool HasTabItems
        {
            get { return TabItems.Count > 0; }
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            foreach (var tab in TabItems.Where(tab => tab.Visible))
            {
                Controls.Add(tab);
            }
            base.OnInit(e);
            InitScripts();
        }

        protected void InitScripts()
        {
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/common/tabsnavigator/js/tabsnavigator.js"));
        }

        protected override void Render(HtmlTextWriter writer)
        {
            var sb = new StringBuilder();

            sb.Append("<div class=\"clearFix\">");
            sb.Append("  <div id=\"" + (String.IsNullOrEmpty(BlockID) ? ClientID : BlockID) + "\" class=\"tabsNavigationLinkBox\">");

            if (HasTabItems)
            {
                var visibleTabItems = TabItems.Where(tab => tab.Visible).ToList();
                var visibleTabsCount = visibleTabItems.Count;

                for (var i = 0; i < visibleTabsCount; i++)
                {
                    sb.Append(visibleTabItems[i].GetTabLink(i == visibleTabsCount - 1));
                }
            }

            sb.Append("  </div>");
            sb.Append("</div>");

            writer.Write(sb.ToString());

            foreach (var tab in TabItems.Where(tab => tab.Visible && string.IsNullOrEmpty(tab.TabHref) && !tab.SkipRender))
            {
                tab.RenderTabContent(writer);
            }
        }
    }
}