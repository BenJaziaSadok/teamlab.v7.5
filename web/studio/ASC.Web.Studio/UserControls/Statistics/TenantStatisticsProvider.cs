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
using System.Linq;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Statistics
{
    public static class TenantStatisticsProvider
    {
        public static bool IsNotPaid()
        {
            return TenantExtra.GetCurrentTariff().State >= TariffState.NotPaid;
        }

        public static int GetUsersCount()
        {
            return CoreContext.UserManager.GetUsers(EmployeeStatus.Default, EmployeeType.User).Length;
        }

        public static long GetUsedSize()
        {
            return GetUsedSize(TenantProvider.CurrentTenantID);
        }

        public static long GetUsedSize(int tenant)
        {
            return GetQuotaRows(tenant).Sum(r => r.Counter);
        }

        public static long GetUsedSize(Guid moduleId)
        {
            return GetQuotaRows(TenantProvider.CurrentTenantID).Where(r => new Guid(r.Tag).Equals(moduleId)).Sum(r => r.Counter);
        }

        private static IEnumerable<TenantQuotaRow> GetQuotaRows(int tenant)
        {
            return CoreContext.TenantManager.FindTenantQuotaRows(new TenantQuotaRowQuery(tenant))
                .Where(r => !string.IsNullOrEmpty(r.Tag) && new Guid(r.Tag) != Guid.Empty);
        }
    }
}