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
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using ASC.Core;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class SmsBuy : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/SmsControls/SmsBuy.ascx"; }
        }

        protected string CurrencySymbol = new RegionInfo("US").CurrencySymbol;
        protected bool CanBuy;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/Management/SmsControls/js/smsbuy.js"));

            SmsBuyContainer.Options.IsPopup = true;

            var smsQuotas = CoreContext.TenantManager.GetTenantQuotas(true).Where(q => q.GetFeature("sms-product"));

            CanBuy = smsQuotas.Any();

            SmsQuotas.DataSource = smsQuotas;
            SmsQuotas.DataBind();
        }
    }
}