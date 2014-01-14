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
using System.IO;
using System.Web;
using System.Web.UI;
using ASC.Web.Core;
using ASC.Web.Studio.Core.HelpCenter;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Common.HelpCenter
{
    public partial class HelpCenter : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Common/HelpCenter/HelpCenter.ascx"; }
        }

        public bool IsSideBar { get; set; }
        public Guid ModuleId { get; set; }

        protected List<HelpCenterItem> HelpCenterItems { get; set; }
        protected String HelpLink { get; set; }
        protected String HelpLinkBlock { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CommonLinkUtility.GetHelpLink(true)))
            {
                return;
            }

            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/common/helpcenter/css/help-center.less"));

            string module;
            string mainLink;

            if (Page is Studio.Management)
            {
                module = "configuration.aspx";
                mainLink = CommonLinkUtility.GetAdministration(ManagementType.HelpCenter);
            }
            else
            {
                var currentModule = GetProduct();

                module = currentModule.ProductClassName + ".aspx";

                var link = currentModule.StartURL;
                mainLink = VirtualPathUtility.ToAbsolute(link + (link.LastIndexOf("/", StringComparison.Ordinal) == 0 ? "" : "/"));

                if (currentModule.ID != WebItemManager.DocumentsProductID
                && currentModule.ID != WebItemManager.MailProductID)
                {
                    mainLink += "help.aspx";
                }
            }
            const string index = "#help";
            HelpLink = mainLink + index;
            HelpLinkBlock = mainLink + index + "=";

            HelpCenterItems = HelpCenterHelper.GetHelpCenter(module, HelpLinkBlock);
        }

        private IWebItem GetProduct()
        {
            var curModule = WebItemManager.Instance[ModuleId] ?? CommonLinkUtility.GetWebItemByUrl(Context.Request.Url.AbsoluteUri);

            if (curModule is IAddon)
                return curModule;

            var product = curModule as IProduct;
            if (product != null)
                return product;

            IModule module;
            CommonLinkUtility.GetLocationByUrl(CommonLinkUtility.GetFullAbsolutePath(curModule.StartURL), out product, out module);
            return product;
        }


        public static string RenderControlToString()
        {
            return RenderControlToString(null);
        }

        public static string RenderControlToString(Guid? moduleId)
        {
            var cntrl = new HelpCenter();
            cntrl = (HelpCenter)cntrl.LoadControl(Location);
            if (moduleId.HasValue)
            {
                cntrl.ModuleId = moduleId.Value;
            }

            var page = new Page();
            page.Controls.Add(cntrl);

            var writer = new StringWriter();
            HttpContext.Current.Server.Execute(page, writer, false);
            var renderedControl = writer.ToString();
            writer.Close();

            return renderedControl;
        }
    }
}