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

using ASC.Core.Billing;
using ASC.Web.Studio.UserControls.Statistics;
using AjaxPro;
using ASC.Core;
using ASC.Web.Studio.Utility;
using System;
using System.Web;
using System.Web.UI;
using Resources;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("TariffPartnerController")]
    public partial class TariffPartner : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/TariffSettings/TariffPartner.ascx"; }
        }

        public Partner CurPartner;
        public bool TariffNotPaid = false;
        public bool TariffProlongable = true;
        public static string PartnerCache = "PartnerCache";


        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/tariffsettings/js/tariffpartner.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/tariffsettings/css/tariffpartner.less"));

            PartnerPayKeyContainer.Options.IsPopup = true;
            PartnerApplyContainer.Options.IsPopup = true;
            PartnerRequestContainer.Options.IsPopup = true;
            PartnerPayExceptionContainer.Options.IsPopup = true;
            AjaxPro.Utility.RegisterTypeForAjax(GetType());

            if (HttpRuntime.Cache.Get(PartnerCache) == null)
                HttpRuntime.Cache.Insert(PartnerCache, DateTime.UtcNow);
        }

        [AjaxMethod]
        public void ActivateKey(string code)
        {
            CoreContext.PaymentManager.ActivateKey(code);
        }

        [AjaxMethod]
        public void RequestKey(int qoutaId)
        {
            var partnerId = CoreContext.TenantManager.GetCurrentTenant().PartnerId;
            CoreContext.PaymentManager.RequestClientPayment(partnerId, qoutaId, true);
        }

        [AjaxMethod]
        public AjaxResponse RequestPayPal(int qoutaId)
        {
            var res = new AjaxResponse();

            try
            {
                if (!HttpRuntime.Cache.Get(PartnerCache).Equals(DateTime.UtcNow))
                    HttpRuntime.Cache.Insert(PartnerCache, DateTime.UtcNow);

                var partnerId = CoreContext.TenantManager.GetCurrentTenant().PartnerId;
                var partner = CoreContext.PaymentManager.GetPartner(partnerId);

                if (partner == null || partner.Status != PartnerStatus.Approved || partner.Removed || partner.PaymentMethod != PartnerPaymentMethod.PayPal)
                {
                    throw new MethodAccessException(Resource.PartnerPayPalExc);
                }

                var tenantQuota = TenantExtra.GetTenantQuota(qoutaId);

                var curruntQuota = TenantExtra.GetTenantQuota();
                if (TenantExtra.GetCurrentTariff().State == TariffState.Paid
                    && tenantQuota.ActiveUsers < curruntQuota.ActiveUsers
                    && tenantQuota.Year == curruntQuota.Year)
                {
                    throw new MethodAccessException(Resource.PartnerPayPalDowngrade);
                }

                if (tenantQuota.Price > partner.AvailableCredit)
                {
                    CoreContext.PaymentManager.RequestClientPayment(partnerId, qoutaId, false);
                    throw new Exception(Resource.PartnerRequestLimitInfo);
                }

                var usersCount = TenantStatisticsProvider.GetUsersCount();
                var usedSize = TenantStatisticsProvider.GetUsedSize();

                if (tenantQuota.ActiveUsers < usersCount || tenantQuota.MaxTotalSize < usedSize)
                {
                    res.rs2 = "quotaexceed";
                    return res;
                }

                res.rs1 = CoreContext.PaymentManager.GetButton(partner.Id, qoutaId);
            }
            catch (Exception e)
            {
                res.message = e.Message;
            }
            return res;
        }
    }
}