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
using ASC.Api.Attributes;
using ASC.Api.Impl;
using ASC.Api.Interfaces;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Studio.Utility;
using ASC.Xmpp.Common;

namespace ASC.Api.Portal
{
    ///<summary>
    /// Portal info access
    ///</summary>
    public class PortalApi : IApiEntryPoint
    {
        private readonly ApiContext context;

        ///<summary>
        /// Api name entry
        ///</summary>
        public string Name
        {
            get { return "portal"; }
        }

        public PortalApi(ApiContext context)
        {
            this.context = context;
        }

        ///<summary>
        ///Returns the current portal
        ///</summary>
        ///<short>
        ///Current portal
        ///</short>
        /// <category>Portal info</category>
        ///<returns>Portal</returns>
        [Read("")]
        public Tenant Get()
        {
            return CoreContext.TenantManager.GetCurrentTenant();
        }

        ///<summary>
        ///Returns the user with specified userID from the current portal
        ///</summary>
        ///<short>
        ///User with specified userID
        ///</short>
        /// <category>Portal info</category>
        ///<returns>User</returns>
        [Read("users/{userID}")]
        public UserInfo GetUser(Guid userID)
        {
            return CoreContext.UserManager.GetUsers(userID);
        }
        
        ///<summary>
        ///Returns the used space of the current portal
        ///</summary>
        ///<short>
        ///Used space of the current portal
        ///</short>
        /// <category>Portal info</category>
        ///<returns>Used space</returns>
        [Read("usedspace")]
        public double GetUsedSpace()
        {
            return Math.Round(
                CoreContext.TenantManager.FindTenantQuotaRows(new TenantQuotaRowQuery(CoreContext.TenantManager.GetCurrentTenant().TenantId))
                           .Where(q => !string.IsNullOrEmpty(q.Tag) && new Guid(q.Tag) != Guid.Empty)
                           .Sum(q => q.Counter) / 1024f / 1024f / 1024f, 2);
        }

        ///<summary>
        ///Returns the users count of the current portal
        ///</summary>
        ///<short>
        ///Users count of the current portal
        ///</short>
        /// <category>Portal info</category>
        ///<returns>Users count</returns>
        [Read("userscount")]
        public long GetUsersCount()
        {
            return CoreContext.UserManager.GetUserNames(EmployeeStatus.Active).Count();
        }

        ///<summary>
        ///Returns the current tariff of the current portal
        ///</summary>
        ///<short>
        ///Tariff of the current portal
        ///</short>
        /// <category>Portal info</category>
        ///<returns>Tariff</returns>
        [Read("tariff")]
        public Tariff GetTariff()
        {
            return CoreContext.PaymentManager.GetTariff(CoreContext.TenantManager.GetCurrentTenant().TenantId);
        }

        ///<summary>
        ///Returns the current quota of the current portal
        ///</summary>
        ///<short>
        ///Quota of the current portal
        ///</short>
        /// <category>Portal info</category>
        ///<returns>Quota</returns>
        [Read("quota")]
        public TenantQuota GetQuota()
        {
            return CoreContext.TenantManager.GetTenantQuota(CoreContext.TenantManager.GetCurrentTenant().TenantId);
        }

        ///<summary>
        ///Returns the recommended quota of the current portal
        ///</summary>
        ///<short>
        ///Quota of the current portal
        ///</short>
        /// <category>Portal info</category>
        ///<returns>Quota</returns>
        [Read("quota/right")]
        public TenantQuota GetRightQuota()
        {
            var usedSpace = GetUsedSpace();
            var needUsersCount = GetUsersCount();

            return CoreContext.TenantManager.GetTenantQuotas().OrderBy(r => r.Price)
                                    .FirstOrDefault(quota =>
                                                    quota.ActiveUsers > needUsersCount
                                                    && quota.MaxTotalSize > usedSpace
                                                    && quota.DocsEdition
                                                    && !quota.Year);
        }

        ///<summary>
        ///Returns path
        ///</summary>
        ///<short>
        ///path
        ///</short>
        ///<category>Portal info</category>
        ///<returns>path</returns>
        ///<visible>false</visible>
        [Read("path")]
        public string GetFullAbsolutePath(string virtualPath)
        {
            return CommonLinkUtility.GetFullAbsolutePath(virtualPath);
        }

        ///<visible>false</visible>
        [Read("talk/unreadmessages")]
        public int GetMessageCount()
        {
            try
            {
                var username = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).UserName;
                return new JabberServiceClient().GetNewMessagesCount(username, TenantProvider.CurrentTenantID);
            }
            catch { }
            return 0;
        }
    }
}