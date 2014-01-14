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
using System.Net;
using System.Collections.Specialized;
using System.Threading;
using System.Web.Configuration;
using ASC.Api;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common.Contracts;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;
using log4net;

namespace ASC.Web.Studio.UserControls.FirstTime
{
    public static class FirstTimeTenantSettings
    {
        public static void SetDefaultTenantSettings()
        {
            try
            {
                WebItemSecurity.SetSecurity("community-wiki", false, null);
                WebItemSecurity.SetSecurity("community-forum", false, null);
            }
            catch (Exception error)
            {
                LogManager.GetLogger("ASC.Web").Error(error);
            }

            try
            {
                if (CoreContext.Configuration.Standalone)
                {
                    TenantExtra.TrialRequest();
                }
            }
            catch (Exception error)
            {
                LogManager.GetLogger("ASC.Web").Error(error);
            }
        }

        public static void SetTenantData(string language)
        {
            try
            {
                using (var backupClient = new BackupServiceClient())
                {
                    int tenantID = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                    BackupResult result = null;
                    do
                    {
                        if (result == null)
                        {
                            result = backupClient.RestorePortal(tenantID, language);
                        }
                        else
                        {
                            result = backupClient.GetRestoreStatus(tenantID);
                        }
                        Thread.Sleep(TimeSpan.FromSeconds(5));

                    } while (!result.Completed);
                    //Thread.Sleep(TimeSpan.FromSeconds(15)); // wait to invalidate users cache...
                }

                var apiServer = new ApiServer();
                apiServer.CallApiMethod(SetupInfo.WebApiBaseUrl + "/crm/settings", "PUT");
            }
            catch (Exception error)
            {
                LogManager.GetLogger("ASC.Web").Error("Can't set default data", error);
            }
        }

        public static void SendInstallInfo(UserInfo user)
        {
            try
            {
                StudioNotifyService.Instance.SendCongratulations(user);
            }
            catch (Exception error)
            {
                LogManager.GetLogger("ASC.Web").Error(error);
            }
            try
            {
                var url = WebConfigurationManager.AppSettings["web.install-url"];
                if (!string.IsNullOrEmpty(url))
                {
                    var tenant = CoreContext.TenantManager.GetCurrentTenant();
                    var q = new MailQuery
                    {
                        Email = user.Email,
                        Id = CoreContext.Configuration.GetKey(tenant.TenantId),
                        Alias = tenant.TenantDomain,
                    };
                    var index = url.IndexOf("?v=");
                    if (0 < index)
                    {
                        q.Version = url.Substring(index + 3);
                        url = url.Substring(0, index);
                    }
                    using (var webClient = new WebClient())
                    {
                        var values = new NameValueCollection();
                        values.Add("query", Signature.Create<MailQuery>(q, "4be71393-0c90-41bf-b641-a8d9523fba5c"));
                        webClient.UploadValues(url, values);
                    }
                }
            }
            catch (Exception error)
            {
                LogManager.GetLogger("ASC.Web").Error(error);
            }
        }

        private class MailQuery
        {
            public string Email { get; set; }
            public string Version { get; set; }
            public string Id { get; set; }
            public string Alias { get; set; }
        }

    }
}