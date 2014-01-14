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
using System.Web;
using ASC.Core;
using ASC.Core.Common.Logging;
using AjaxPro;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using ASC.Web.Core;
using System.Collections.Generic;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("DefaultPageController")]
    public partial class DefaultPageSettings : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/DefaultPageSettings/DefaultPageSettings.ascx"; } }

        protected List<DefaultStartPageWrapper> DefaultPages { get; set; }
        protected Guid DefaultProductID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/defaultpagesettings/js/defaultpage.js"));

            DefaultPages = new List<DefaultStartPageWrapper>();

            var defaultPageSettings = SettingsManager.Instance.LoadSettings<StudioDefaultPageSettings>(TenantProvider.CurrentTenantID);
            DefaultProductID = defaultPageSettings.DefaultProductID;

            var products = WebItemManager.Instance.GetItemsAll<IProduct>();
            foreach (var p in products)
            {
                var productInfo = WebItemSecurity.GetSecurityInfo(p.ID.ToString());
                if(productInfo.Enabled)
                    DefaultPages.Add( new DefaultStartPageWrapper
                                          {
                                              ProductID = p.ID,
                                              DisplayName = p.Name,
                                              ProductName = p.GetSysName(),
                                              IsSelected = DefaultProductID.Equals(p.ID)
                                          });

            }

            DefaultPages.Add(new DefaultStartPageWrapper
            {
                ProductID = defaultPageSettings.FeedModuleID,
                DisplayName = Resources.UserControlsCommonResource.FeedTitle,
                ProductName = "feed",
                IsSelected = DefaultProductID.Equals(defaultPageSettings.FeedModuleID)
            });

            DefaultPages.Add(new DefaultStartPageWrapper
            {
                ProductID = Guid.Empty,
                DisplayName = Resources.Resource.DefaultPageSettingsChoiseOfProducts,
                ProductName = string.Empty,
                IsSelected = DefaultProductID.Equals(Guid.Empty)
            });
        }

        [AjaxMethod]
        public object SaveSettings(string defaultProductID)
        {
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                var defaultPageSettingsObj = new StudioDefaultPageSettings
                                                 {
                                                     DefaultProductID = new Guid(defaultProductID)
                                                 };
                var resultStatus = SettingsManager.Instance.SaveSettings(defaultPageSettingsObj, TenantProvider.CurrentTenantID);

                AdminLog.PostAction("Settings: default product ID \"{0}\"", defaultProductID);

                return new
                {
                    Status = 1,
                    Message = Resources.Resource.SuccessfullySaveSettingsMessage
                };
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }
        }
    }

    public class DefaultStartPageWrapper
    {
        public Guid ProductID { get; set; }
        public string DisplayName { get; set; }
        public string ProductName { get; set; }
        public bool IsSelected { get; set; }
    }
}