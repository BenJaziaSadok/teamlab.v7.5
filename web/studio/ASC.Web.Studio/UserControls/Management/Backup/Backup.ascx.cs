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
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Common.Contracts;
using ASC.Core.Common.Logging;
using ASC.Core.Tenants;
using ASC.Security.Cryptography;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;
using Newtonsoft.Json;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxPro.AjaxNamespace("Backup")]
    public partial class Backup : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Management/Backup/Backup.ascx"; } }

        public string Url
        {
            get
            {
                var url = Request.GetUrlRewriter().OriginalString;
                var i = url.LastIndexOf("/");
                if (i > 0)
                    return url.Substring(0, i);
                else return "";
            }
        }

        protected bool EnableBackup
        {
            get
            {
                return (TenantExtra.GetTenantQuota().HasBackup);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(typeof(Backup), this.Page);
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/backup/js/backup.js"));
            RegisterScript();
        }

        [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)]
        public string Deactivate(bool flag)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var t = CoreContext.TenantManager.GetCurrentTenant();
            if (t != null)
            {
                SendMailDeactivate(t);
                var u = CoreContext.UserManager.GetUsers(t.OwnerId);
                var emailLink = string.Format("<a href=\"mailto:{0}\">{0}</a>", u.Email);

                AdminLog.PostAction("Settings: deactivated portal");

                return ((string)Resources.Resource.AccountDeactivationMsg).Replace(":email", emailLink);
            }
            return string.Empty;
        }

        [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)]
        public string Delete(bool flag)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var t = CoreContext.TenantManager.GetCurrentTenant();
            if (t != null)
            {
                SendMailDelete(t);
                var u = CoreContext.UserManager.GetUsers(t.OwnerId);
                var emailLink = string.Format("<a href=\"mailto:{0}\">{0}</a>", u.Email);

                AdminLog.PostAction("Settings: deleted portal");

                return ((string)Resources.Resource.AccountDeletionMsg).Replace(":email", emailLink);
            }
            return string.Empty;
        }

        private static string ToJsonSuccess(BackupResult result)
        {
            return JsonConvert.SerializeObject(new {success = true, data = result});
        }

        private static string ToJsonError(string message)
        {
            return JsonConvert.SerializeObject(new {success = false, data = message});
        }

        private void SendMailDeactivate(Tenant t)
        {
            var u = CoreContext.UserManager.GetUsers(t.OwnerId);
            StudioNotifyService.Instance.SendMsgPortalDeactivation(t,
                GetConfirmLink(u.Email, ConfirmType.PortalSuspend),
                GetConfirmLink(u.Email, ConfirmType.PortalContinue));
        }

        private void SendMailDelete(Tenant t)
        {
            var u = CoreContext.UserManager.GetUsers(t.OwnerId);
            StudioNotifyService.Instance.SendMsgPortalDeletion(t,
                GetConfirmLink(u.Email, ConfirmType.PortalRemove));
        }

        private string GetConfirmLink(string email, ConfirmType confirmType)
        {
            var validationKey = EmailValidationKeyProvider.GetEmailKey(email + confirmType.ToString());

            return CommonLinkUtility.GetFullAbsolutePath("~/confirm.aspx") +
                string.Format("?type={0}&email={1}&key={2}", confirmType.ToString(), HttpUtility.UrlEncode(email), validationKey);
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"BackupManager.init(""{0}"", ""{1}"");",
                Url,
                Resources.Resource.BackupError
            );

            Page.RegisterInlineScript(sb.ToString());
        }
    }
}