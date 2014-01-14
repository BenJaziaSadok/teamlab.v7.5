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
using System.Web.UI;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;
using System.Web.UI.HtmlControls;
using System.Web;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class TariffLimitExceed : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/TariffSettings/TariffLimitExceed.ascx"; }
        }

        protected bool IsDefaultTariff;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/tariffsettings/js/tarifflimitexceed.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/tariffsettings/css/tarifflimitexceed.less"));

            tariffLimitExceedUsersDialog.Options.IsPopup = true;
            tariffLimitExceedStorageDialog.Options.IsPopup = true;
            tariffLimitDocsEditionDialog.Options.IsPopup = true;

            IsDefaultTariff = TenantExtra.GetCurrentTariff().QuotaId.Equals(Tenant.DEFAULT_TENANT);
        }
    }
}