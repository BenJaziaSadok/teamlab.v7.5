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
using System.Linq;
using System.Web;
using System.Web.UI;

using ASC.Web.Studio.Utility;
using ASC.Core;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class TariffSettings : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/TariffSettings/TariffSettings.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/tariffsettings/js/tariffsettings.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/tariffsettings/css/tariffsettings.less"));

            tariffHolder.Controls.Add(LoadControl(TariffUsage.Location));

            var payments = CoreContext.PaymentManager.GetTariffPayments(TenantProvider.CurrentTenantID);
            PaymentsRepeater.Visible =
                payments.Any()
                && !TenantExtra.GetTenantQuota().Trial
                && CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, ASC.Core.Users.Constants.GroupAdmin.ID);
            PaymentsRepeater.DataSource = payments;
            PaymentsRepeater.DataBind();
        }
    }
}