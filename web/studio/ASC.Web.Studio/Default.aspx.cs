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
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Core;
using AjaxPro;

namespace ASC.Web.Studio
{
    [AjaxNamespace("StudioDefault")]
    public partial class _Default : MainPage
    {
        public bool ShowWelcomePopupForCollaborator { get; set; }

        protected Product _showDocs;

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (CoreContext.Configuration.YourDocs)
                Context.Response.Redirect(CommonLinkUtility.FilesBaseAbsolutePath);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/skins/page_default.less"));

            var defaultPageSettings = SettingsManager.Instance.LoadSettings<StudioDefaultPageSettings>(TenantProvider.CurrentTenantID);
            if (defaultPageSettings != null && defaultPageSettings.DefaultProductID != Guid.Empty)
            {
                if (defaultPageSettings.DefaultProductID == defaultPageSettings.FeedModuleID)
                    Context.Response.Redirect("feed.aspx");

                var products = WebItemManager.Instance.GetItemsAll<IProduct>();
                foreach (var p in products)
                {
                    if (p.ID.Equals(defaultPageSettings.DefaultProductID))
                    {
                        var productInfo = WebItemSecurity.GetSecurityInfo(p.ID.ToString());
                        if (productInfo.Enabled && WebItemSecurity.IsAvailableForUser(p.ID.ToString(), SecurityContext.CurrentAccount.ID))
                        {
                            Context.Response.Redirect(p.StartURL);
                        }
                    }
                }
            }

            Master.DisabledSidePanel = true;
            Master.TopStudioPanel.DisableProductNavigation = true;

            Title = Resources.Resource.MainPageTitle;
            var items = WebItemManager.Instance.GetItems(Web.Core.WebZones.WebZoneType.StartProductList);
            _showDocs = (Product)items.Find(r => r.ID == WebItemManager.DocumentsProductID);
            if (_showDocs != null)
            {
                items.RemoveAll(r => r.ID == _showDocs.ProductID);
            }
            _productRepeater.DataSource = items;
            _productRepeater.DataBind();



            _welcomeBoxContainer.Options.IsPopup = true;
            var showWelcomePopup = ((Request["first"] ?? "") == "1");
            if (showWelcomePopup && Session["first"] == null)
                Session["first"] = new object();
            else
                showWelcomePopup = false;

            _afterRegistryWelcomePopupBoxHolder.Visible = showWelcomePopup;

            var isVisitor = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor();
            var collaboratorPopupSettings = SettingsManager.Instance.LoadSettingsFor<CollaboratorSettings>(SecurityContext.CurrentAccount.ID);

            if (showWelcomePopup)
            {
                Page.RegisterInlineScript("StudioBlockUIManager.blockUI('#studio_welcomeMessageBox', 400, 300, 0);");
            }

            if (isVisitor && collaboratorPopupSettings.FirstVisit)
            {
                AjaxPro.Utility.RegisterTypeForAjax(GetType());

                ShowWelcomePopupForCollaborator = true;
                _welcomePopupForCollaborators.Visible = true;
                _welcomeCollaboratorContainer.Options.IsPopup = true;

                Page.RegisterInlineScript("StudioBlockUIManager.blockUI('#studio_welcomeCollaboratorContainer', 500, 400, 0);");
            }
        }

        [AjaxMethod]
        public void CloseWelcomePopup()
        {
            var collaboratorPopupSettings = SettingsManager.Instance.LoadSettingsFor<CollaboratorSettings>(SecurityContext.CurrentAccount.ID);
            collaboratorPopupSettings.FirstVisit = false;
            SettingsManager.Instance.SaveSettingsFor(collaboratorPopupSettings, SecurityContext.CurrentAccount.ID);
        }
    }
}