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
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using System.Web.UI.HtmlControls;
using System.Web;
using System.Text;

namespace ASC.Web.Studio.UserControls.Statistics
{
    public partial class ProductQuotes : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Statistics/ProductQuotes/ProductQuotes.ascx"; }
        }

        public long CurrentSize { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/statistics/productquotes/css/productquotes_style.less"));

            var data = new List<object>();
            foreach (var item in WebItemManager.Instance.GetItems(Web.Core.WebZones.WebZoneType.All, ItemAvailableState.All))
            {
                if (item.Context == null || item.Context.SpaceUsageStatManager == null)
                    continue;

                data.Add(new Product {Id = item.ID, Name = item.Name, Icon = item.GetIconAbsoluteURL()});
            }

            _itemsRepeater.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler(_itemsRepeater_ItemDataBound);
            _itemsRepeater.DataSource = data;
            _itemsRepeater.DataBind();

            RegisterScript();
        }

        private void _itemsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            var product = e.Item.DataItem as Product;
            var webItem = WebItemManager.Instance[product.Id];

            var data = new List<object>();
            var items = webItem.Context.SpaceUsageStatManager.GetStatData();

            foreach (var it in items)
            {
                data.Add(new {Name = it.Name, Icon = it.ImgUrl, Size = FileSizeComment.FilesSizeToString(it.SpaceUsage), Url = it.Url});
            }

            if (items.Count == 0)
            {
                e.Item.FindControl("_emptyUsageSpace").Visible = true;
                e.Item.FindControl("_showMorePanel").Visible = false;

            }
            else
            {
                var repeater = (Repeater) e.Item.FindControl("_usageSpaceRepeater");
                repeater.DataSource = data;
                repeater.DataBind();


                e.Item.FindControl("_showMorePanel").Visible = (items.Count > 10);
                e.Item.FindControl("_emptyUsageSpace").Visible = false;
            }
        }

        protected String RenderCreatedDate()
        {
            return String.Format("{0}", CoreContext.TenantManager.GetCurrentTenant().CreatedDateTime.ToShortDateString());
        }

        protected int RenderUsersTotal()
        {
            return TenantStatisticsProvider.GetUsersCount();
        }

        protected String GetMaxTotalSpace()
        {
            return FileSizeComment.FilesSizeToString(TenantExtra.GetTenantQuota().MaxTotalSize);
        }

        protected String RenderUsedSpace()
        {
            var used = TenantStatisticsProvider.GetUsedSize();
            return FileSizeComment.FilesSizeToString(used);
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.Append(@"
                    jq('.moreBox a.topTitleLink').click(function () {
                        jq(jq(this).parent().prev()).find('tr').show();
                        jq(this).parent().hide();
                    });"
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        protected sealed class Product
        {
            public Guid Id { get; set; }
            public String Name { get; set; }
            public String Icon { get; set; }
            public long Size { get; set; }
        }
    }
}