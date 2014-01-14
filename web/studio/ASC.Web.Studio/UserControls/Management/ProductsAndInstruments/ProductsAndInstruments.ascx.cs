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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.Core;
using ASC.Web.Core.WebZones;
using ASC.Web.Core.ModuleManagement;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using System.Web.UI.HtmlControls;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class ProductsAndInstruments : UserControl
    {
        #region Properies

        public static string Location
        {
            get { return "~/UserControls/Management/ProductsAndInstruments/ProductsAndInstruments.ascx"; }
        }

        protected List<Item> Products;
        protected List<Item> Modules;

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            InitProperties();
            RegisterClientScript();
        }

        #endregion

        #region Methods

        private void InitProperties()
        {
            Products = new List<Item>();
            Modules = new List<Item>();

            var webItems = WebItemManager.Instance.GetItems(WebZoneType.All, ItemAvailableState.All)
                .Where(item => !item.IsSubItem() && !item.CanNotBeDisabled())
                .ToList();

            foreach (var webItem in webItems)
            {
                var item = new Item
                {
                    ID = webItem.ID,
                    Name = webItem.Name,
                    IconUrl = webItem.GetIconAbsoluteURL(),
                    DisabledIconUrl = webItem.GetDisabledIconAbsoluteURL(),
                    SubItems = new List<Item>(),
                    ItemName = webItem.GetSysName()
                };

                var productInfo = WebItemSecurity.GetSecurityInfo(item.ID.ToString());
                item.Disabled = !productInfo.Enabled;

                foreach (var m in WebItemManager.Instance.GetSubItems(webItem.ID, ItemAvailableState.All))
                {
                    if ((m as Module) != null && (m as IWebItem) != null)
                    {
                        var subItem = new Item
                        {
                            ID = m.ID,
                            Name = m.Name,
                            DisplayedAlways = (m as Module).DisplayedAlways,
                            ItemName = m.GetSysName()
                        };

                        var moduleInfo = WebItemSecurity.GetSecurityInfo(subItem.ID.ToString());
                        subItem.Disabled = !moduleInfo.Enabled;

                        item.SubItems.Add(subItem);
                    }
                }

                if(webItem is IProduct)
                    Products.Add(item);
                else
                    Modules.Add(item);
            }
        }

        private void RegisterClientScript()
        {
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/productsandinstruments/js/productsandinstruments.js"));

            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/productsandinstruments/css/productsandinstruments.less"));
        }

        #endregion
    }
}