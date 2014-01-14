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
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.Web.Core;
using ASC.Web.Core.WebZones;

namespace ASC.Web.Talk.Addon
{
    [WebZoneAttribute(WebZoneType.CustomProductList)]
    public class TalkAddon : IAddon, IRenderCustomNavigation
    {
        public static Guid AddonID
        {
            get { return WebItemManager.TalkProductID; }
        }

        public string Name
        {
            get { return Resources.TalkResource.ProductName; }
        }

        public string Description
        {
            get { return Resources.TalkResource.TalkDescription; }
        }

        public Guid ID
        {
            get { return AddonID; }
        }

        public AddonContext Context { get; private set; }

        WebItemContext IWebItem.Context
        {
            get { return Context; }
        }


        public void Init()
        {
            Context = new AddonContext
                {
                    DisabledIconFileName = "product_logo_disabled.png",
                    IconFileName = "product_logo.png",
                    LargeIconFileName = "product_logolarge.png",
                    DefaultSortOrder = 60,
                };
        }

        public void Shutdown()
        {

        }

        public string StartURL
        {
            get { return BaseVirtualPath + "/default.aspx"; }
        }

        public string ProductClassName
        {
            get { return "talk"; }
        }

        public const string BaseVirtualPath = "~/addons/talk";

        public static string GetClientUrl()
        {
            return VirtualPathUtility.ToAbsolute("~/addons/talk/jabberclient.aspx");
        }

        public static string GetTalkClientURL()
        {
            return "javascript:window.ASC.Controls.JabberClient.open('" + VirtualPathUtility.ToAbsolute("~/addons/talk/jabberclient.aspx") + "')";
        }

        public static string GetMessageStr()
        {
            return Resources.TalkResource.Chat;
        }

        public Control LoadCustomNavigationControl(Page page)
        {
            return null;
        }

        public string RenderCustomNavigation(Page page)
        {
            if (CoreContext.Configuration.YourDocs) return string.Empty;

            var sb = new StringBuilder();
            using (var tw = new StringWriter(sb))
            {
                using (var hw = new HtmlTextWriter(tw))
                {
                    var ctrl = page.LoadControl(UserControls.TalkNavigationItem.Location);
                    ctrl.RenderControl(hw);
                    return sb.ToString();
                }
            }
        }
    }
}