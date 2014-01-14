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
using ASC.Core;
using ASC.Security.Cryptography;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;
using ASC.Core.Users;
using System.Web;
using System.Text;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class InviteLink : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/InviteLink/InviteLink.ascx"; }
        }

        protected bool EnableInviteLink = TenantStatisticsProvider.GetUsersCount() < TenantExtra.GetTenantQuota().ActiveUsers;

        protected string GeneratedUserLink;
        protected string GeneratedVisitorLink;

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(ResolveUrl("~/js/third-party/zeroclipboard.js"));
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/invitelink/js/invitelink.js"));

            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/invitelink/css/invitelink.less"));

            GeneratedUserLink = GenerateLink(EmployeeType.User);
            GeneratedVisitorLink = GenerateLink(EmployeeType.Visitor);

            RegisterScript();
        }

        public static string GenerateLink(EmployeeType employeeType)
        {
            var type = ConfirmType.LinkInvite.ToString();
            var emplType = (int) employeeType;

            var validationKey = EmailValidationKeyProvider.GetEmailKey(type + emplType);

            return CommonLinkUtility.GetFullAbsolutePath(String.Format("~/confirm.aspx?type={0}&key={1}&uid={2}&emplType={3}", type, validationKey, SecurityContext.CurrentAccount.ID, emplType));
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"ZeroClipboard.setMoviePath('{0}');",
                CommonLinkUtility.ToAbsolute("~/js/flash/zeroclipboard/ZeroClipboard10.swf")
            );

            Page.RegisterInlineScript(sb.ToString(), true);
        }
    }
}