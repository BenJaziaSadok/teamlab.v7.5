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

using System.ComponentModel;
using System.Web.UI;
using ASC.Web.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.Controls.Common
{
    [DefaultProperty("Title")]
    [ToolboxData("<{0}:FeedControl runat=server></{0}:FeedControl>")]
    public class FeedControl : Control
    {
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Title { get; set; }

        [DefaultValue("")]
        [Localizable(false)]
        public string ProductId { get; set; }

        [DefaultValue("")]
        [Localizable(false)]
        public string ContainerId { get; set; }

        [DefaultValue("")]
        [Localizable(true)]
        public string ModuleId { get; set; }

        [DefaultValue(false)]
        [Localizable(false)]
        public bool ContentOnly { get; set; }

        [DefaultValue(true)]
        [Localizable(false)]
        public bool AutoFill { get; set; }

        public FeedControl()
        {
            ContentOnly = false;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (AutoFill)
            {
                if (string.IsNullOrEmpty(ProductId))
                {
                    ProductId = CommonLinkUtility.GetProductID().ToString("D");
                }
                if (string.IsNullOrEmpty(ModuleId))
                {
                    IProduct product;
                    IModule module;
                    CommonLinkUtility.GetLocationByRequest(out product, out module);
                    if (module != null)
                    {
                        ModuleId = module.ID.ToString("D");
                    }
                }
            }

            writer.Write(Services.WhatsNew.feed.RenderRssMeta(Title, ProductId));
        }
    }
}