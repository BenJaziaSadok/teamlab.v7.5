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
using System.Text;
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Core.WebZones;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.HelpCenter;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;
using Resources;

namespace ASC.Web.Studio.UserControls.Common
{
    public partial class TopStudioPanel : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Common/TopStudioPanel/TopStudioPanel.ascx"; }
        }

        protected IProduct CurrentProduct;
        protected Guid CurrentProductID;
        protected Guid CurrentModuleID;
        protected UserInfo CurrentUser;
        protected bool DisplayModuleList;
        protected string TariffNotify;

        protected List<VideoGuideItem> VideoGuideItems { get; set; }
        protected string AllVideoLink = CommonLinkUtility.GetHelpLink(true) + "video.aspx";

        public bool? DisableUserInfo { get; set; }

        public bool DisableProductNavigation { get; set; }
        public bool DisableTariffNotify { get; set; }
        public bool DisableSearch { get; set; }
        public bool DisableSettings { get; set; }
        public bool DisableTariff { get; set; }
        public bool DisableVideo { get; set; }

        protected IEnumerable<IWebItem> SearchProducts { get; set; }

        protected bool ShowTopPanelNavigation { get; set; }

        private List<IWebItem> _customNavItems;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (DebugInfo.ShowDebugInfo)
                debugInfoPopUpContainer.Options.IsPopup = true;

            if (Page is Auth || Page is _Default)
                CurrentProductID = Guid.Empty;
            else
            {
                CurrentProductID =
                    !String.IsNullOrEmpty(Request["productID"])
                        ? new Guid(Request["productID"])
                        : CommonLinkUtility.GetProductID();

                if (!String.IsNullOrEmpty(Request["moduleID"]))
                {
                    CurrentModuleID = new Guid(Request["moduleID"]);
                }
            }

            CurrentProduct = (IProduct)WebItemManager.Instance[CurrentProductID];

            if (SecurityContext.CurrentAccount.IsAuthenticated && !TenantStatisticsProvider.IsNotPaid())
                Page.RegisterBodyScripts(ResolveUrl("~/UserControls/Common/TopStudioPanel/js/FeedReaderScripts.js"));

            RenderVideoHandlers();

            if (!DisableSearch)
            {
                RenderSearchProducts();
                DisableSearch = DisableSearch || !SearchProducts.Any() || CoreContext.Configuration.YourDocs;
            }

            _guestInfoHolder.Visible = false;
            _userInfoHolder.Visible =
                (!DisableUserInfo.HasValue || !DisableUserInfo.Value)
                && SecurityContext.IsAuthenticated
                && !(Page is Wizard);

            ShowTopPanelNavigation = SecurityContext.IsAuthenticated && !(Page is Wizard);

            if (SecurityContext.IsAuthenticated)
                CurrentUser = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

            TariffNotify = TenantExtra.GetTariffNotify();
            if (!SecurityContext.IsAuthenticated || !TenantExtra.EnableTarrifSettings || CoreContext.Configuration.YourDocs || CurrentUser.IsVisitor())
            {
                DisableTariffNotify = true;
                DisableTariff = true;
            }
            else if (string.IsNullOrEmpty(TariffNotify))
            {
                DisableTariffNotify = true;
            }
            _customNavItems = WebItemManager.Instance.GetItems(WebZoneType.CustomProductList, ItemAvailableState.Normal);

            if (CurrentUser.IsVisitor())
            {
                _customNavItems.RemoveAll(item => item.ID == WebItemManager.MailProductID); // remove mail for guest
            }

            if (DisableProductNavigation)
                _productListHolder.Visible = false;
            else
            {
                var productsList = WebItemManager.Instance.GetItems(WebZoneType.TopNavigationProductList, ItemAvailableState.Normal); //.Where(pr => !CurrentProductID.Equals(pr.ID));
                DisplayModuleList = productsList.Any() && !CoreContext.Configuration.YourDocs;
                _productRepeater.DataSource = productsList;

                _productRepeater.DataBind();

                var addons = _customNavItems.Where(pr => ((pr.ID == WebItemManager.CalendarProductID || pr.ID == WebItemManager.TalkProductID || pr.ID == WebItemManager.MailProductID)));
                //if (GetCurrentWebItem != null)
                //    _addons = _addons.Where(pr => pr.ID != GetCurrentWebItem.ID);
                _addonRepeater.DataSource = addons.ToList();
                _addonRepeater.DataBind();

                MoreProductsRepeater.DataBind();
            }

            foreach (var item in _customNavItems)
            {
                var render = WebItemManager.Instance[item.ID] as IRenderCustomNavigation;
                if (render == null) continue;

                try
                {
                    var control = render.LoadCustomNavigationControl(Page);
                    if (control != null)
                    {
                        _customNavControls.Controls.Add(control);
                    }
                }
                catch (Exception ex)
                {
                    log4net.LogManager.GetLogger("ASC.Web.Studio").Error(ex);
                }
            }
        }

        private IWebItem GetCurrentWebItem
        {
            get { return CommonLinkUtility.GetWebItemByUrl(Context.Request.Url.AbsoluteUri); }
        }

        private string GetAddonNameOrEmptyClass()
        {
            if (Page is Studio.Feed)
                return "feed";

            if (Page is Studio.Management)
                return "settings";

            var item = GetCurrentWebItem;

            if (item == null)
                return "";

            if (item.ID == WebItemManager.CalendarProductID || item.ID == WebItemManager.TalkProductID || item.ID == WebItemManager.MailProductID)
                return item.ProductClassName;

            return "";
        }

        private string GetAddonNameOrEmpty()
        {
            if (Page is Studio.Feed)
                return UserControlsCommonResource.FeedTitle;

            if (Page is Studio.Management)
                return Resource.Administration;

            var item = GetCurrentWebItem;

            if (item == null)
                return Resource.SelectProduct;

            if (item.ID == WebItemManager.CalendarProductID || item.ID == WebItemManager.TalkProductID || item.ID == WebItemManager.MailProductID)
                return item.Name;

            return Resource.SelectProduct;
        }

        protected string CurrentProductName
        {
            get
            {
                return
                    CurrentProduct == null
                        ? GetAddonNameOrEmpty()
                        : CurrentProduct.Name;
            }
        }

        protected string CurrentProductClassName
        {
            get
            {
                return
                    CurrentProduct == null
                        ? GetAddonNameOrEmptyClass()
                        : CurrentProduct.ProductClassName;
            }
        }

        protected bool IsAdministrator
        {
            get { return CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, Constants.GroupAdmin.ID); }
        }

        protected string RenderCustomNavigation()
        {
            if (TenantStatisticsProvider.IsNotPaid())
                return string.Empty;

            var sb = new StringBuilder();
            _customNavItems.Reverse();
            string rendered;
            foreach (var item in _customNavItems)
            {
                var render = WebItemManager.Instance[item.ID] as IRenderCustomNavigation;
                if (render == null) continue;

                rendered = render.RenderCustomNavigation(Page);
                if (!string.IsNullOrEmpty(rendered))
                {
                    sb.Append(rendered);
                }
            }

            rendered = Studio.Feed.RenderCustomNavigation(Page);
            if (!string.IsNullOrEmpty(rendered))
            {
                sb.Append(rendered);
            }

            return sb.ToString();
        }

        protected string GetAbsoluteCompanyTopLogoPath()
        {
            var baseLogo = (CoreContext.Configuration.YourDocsDemo && Page is Auth) ? WebImageSupplier.GetAbsoluteWebPath("logo/logo-office.png") : WebImageSupplier.GetAbsoluteWebPath("logo/top_logo.png");
            return string.IsNullOrEmpty(SetupInfo.MainLogoURL) ? baseLogo : SetupInfo.MainLogoURL;
        }

        protected void RenderVideoHandlers()
        {
            if (string.IsNullOrEmpty(CommonLinkUtility.GetHelpLink(false)))
            {
                DisableVideo = true;
                return;
            }

            VideoGuideItems = HelpCenterHelper.GetVideoGuides();

            if (VideoGuideItems.Count > 0)
            {
                AjaxPro.Utility.RegisterTypeForAjax(typeof(UserVideoSettings));
            }
        }

        protected void RenderSearchProducts()
        {
            var handlers = SearchHandlerManager.GetAllHandlersEx();

            SearchProducts = handlers
                .Select(sh => sh.ProductID)
                .Distinct()
                .Select(productID => WebItemManager.Instance[productID])
                .Where(product => product != null && !product.IsDisabled());
        }
    }
}