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
using ASC.Web.Core.Utility.Skins;
using System.Web.UI.HtmlControls;
using System.Web;
using System.Text;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class TariffLicenseOver : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/TariffSettings/TariffLicenseOver.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/tariffsettings/css/tarifflimitexceed.less"));

            tariffLimitExceedLicense.Options.IsPopup = true;

            RegisterScript();
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.Append(@"
                (function () {
                    jq(function () {
                        StudioBlockUIManager.blockUI('#tariffLimitExceedLicense', 500, 300, 0);
                    });
                })();"
            );

            Page.RegisterInlineScript(sb.ToString(), onReady: false);
        }
    }
}