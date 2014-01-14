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
using System.Configuration;
using System.Linq;
using System.Web.UI;
using ASC.Core;
using ASC.Core.Common.Contracts;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using System.Web;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxPro.AjaxNamespace("TransferPortal")]
    public partial class TransferPortal : UserControl
    {
        protected class TransferRegionWithName : TransferRegion
        {
            public string FullName { get; set; }
        }
        
        public static string Location { get { return "~/UserControls/Management/TransferPortal/TransferPortal.ascx"; } }

        private List<TransferRegionWithName> _transferRegions;

        protected string CurrentRegion 
        { 
            get
            {
                return TransferRegions.Where(x => x.IsCurrentRegion).Select(x => x.Name).FirstOrDefault() ?? string.Empty;
            } 
        }

        protected string BaseDomain
        {
            get
            {
                return TransferRegions.Where(x => x.IsCurrentRegion).Select(x => x.BaseDomain).FirstOrDefault() ?? string.Empty;
            }
        }

        protected List<TransferRegionWithName> TransferRegions
        {
            get { return _transferRegions ?? (_transferRegions = GetRegions()); }
        }

        private UserInfo CurUser
        {
            get { return CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID); }
        }

        protected bool EnableMigration
        {
            get
            {
                if (!CurUser.IsOwner()) 
                    return false;

                var secretEmailPattern = ConfigurationManager.AppSettings["web.autotest.secret-email"];
                if (!string.IsNullOrEmpty(secretEmailPattern) && Regex.IsMatch(CurUser.Email, secretEmailPattern))
                    return true;

                return !TenantExtra.GetTenantQuota().Trial && ConfigurationManager.AppSettings["web.migration.status"] == "true";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(typeof(TransferPortal), Page);

            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/TransferPortal/js/transferportal.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/transferportal/css/transferportal.less"));

            popupTransferStart.Options.IsPopup = true; 
        }

        [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)]
        public string StartTransfer(string targetRegion, bool notify, bool backupmail)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            if (!EnableMigration)
                return null;

            if (CurrentRegion.Equals(targetRegion, StringComparison.OrdinalIgnoreCase))
                return ToJsonError(Resources.Resource.ErrorTransferPortalInRegion);

            try
            {
                using (var backupClient = new BackupServiceClient())
                {
                    var transferRequest = new TransferRequest
                        {
                            TenantId = TenantProvider.CurrentTenantID,
                            TargetRegion = targetRegion,
                            NotifyUsers = notify,
                            BackupMail = backupmail
                        };

                    return ToJsonSuccess(backupClient.TransferPortal(transferRequest));
                }
            }
            catch (Exception error)
            {
                return ToJsonError(error.Message);
            }
        }

        [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)]
        public string CheckTransferProgress(string tenantID)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            if (string.IsNullOrEmpty(tenantID))
                return ToJsonSuccess(new BackupResult());

            try
            {               
                using (var backupClient = new BackupServiceClient())
                {
                    BackupResult status = backupClient.GetTransferStatus(Convert.ToInt32(tenantID));                  
                    return ToJsonSuccess(status);
                }
            }
            catch (Exception error)
            {
                return ToJsonError(error.Message);
            }
        }

        private static string ToJsonSuccess(BackupResult result)
        {
            var data = new {result.Completed, result.Id, result.Percent};
            return JsonConvert.SerializeObject(new {success = true, data});
        }

        private static string ToJsonError(string message)
        {
            return JsonConvert.SerializeObject(new { success = false, data = message });
        }

        private static List<TransferRegionWithName> GetRegions()
        {
            try
            {
                using (var backupClient = new BackupServiceClient())
                {
                    return backupClient.GetTransferRegions()
                                       .Select(x => new TransferRegionWithName
                                           {
                                               Name = x.Name,
                                               IsCurrentRegion = x.IsCurrentRegion,
                                               BaseDomain = x.BaseDomain,
                                               FullName = TransferResourceHelper.GetRegionDescription(x.Name)
                                           })
                                       .ToList();
                }
            }
            catch
            {
                return new List<TransferRegionWithName>();
            }
        }
    }
}