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
using System.Web;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Tenants;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.UserControls.Statistics;
using Resources;

namespace ASC.Web.Studio.Utility
{
    public static class TenantExtra
    {
        public static bool EnableTarrifSettings
        {
            get { return SetupInfo.IsVisibleSettings<TariffSettings>(); }
        }

        public static string GetTariffPageLink()
        {
            return VirtualPathUtility.ToAbsolute("~/tariffs.aspx");
        }

        public static Tariff GetCurrentTariff()
        {
            return CoreContext.PaymentManager.GetTariff(TenantProvider.CurrentTenantID);
        }

        public static TenantQuota GetTenantQuota()
        {
            return GetTenantQuota(TenantProvider.CurrentTenantID);
        }

        public static TenantQuota GetTenantQuota(int tenant)
        {
            return CoreContext.TenantManager.GetTenantQuota(tenant);
        }

        public static IEnumerable<TenantQuota> GetTenantQuotas()
        {
            return CoreContext.TenantManager.GetTenantQuotas();
        }

        private static TenantQuota GetPrevQuota(TenantQuota curQuota)
        {
            TenantQuota prev = null;
            foreach (var quota in GetTenantQuotas().OrderBy(r => r.ActiveUsers).Where(r => r.DocsEdition && r.Year == curQuota.Year))
            {
                if (quota.Id == curQuota.Id)
                    return prev;

                prev = quota;
            }
            return null;
        }

        public static int GetPrevUsersCount(TenantQuota quota)
        {
            var prevQuota = GetPrevQuota(quota);
            if (prevQuota == null || prevQuota.Trial)
                return 1;
            if (prevQuota.DocsEdition != quota.DocsEdition)
                return 1;
            return prevQuota.ActiveUsers + 1;
        }

        public static int GetRightQuotaId()
        {
            var q = GetRightQuota();
            return q != null ? q.Id : 0;
        }

        public static TenantQuota GetRightQuota()
        {
            var usedSpace = TenantStatisticsProvider.GetUsedSize();
            var needUsersCount = TenantStatisticsProvider.GetUsersCount();

            return GetTenantQuotas().OrderBy(r => r.ActiveUsers)
                                    .FirstOrDefault(quota =>
                                                    quota.ActiveUsers > needUsersCount
                                                    && quota.MaxTotalSize > usedSpace
                                                    && quota.DocsEdition
                                                    && !quota.Year);
        }

        public static string GetTariffNotify()
        {
            var tariff = GetCurrentTariff();
            if (tariff.State == TariffState.Trial)
            {
                var count = tariff.DueDate.Subtract(DateTime.Today.Date).Days;
                if (count <= 0)
                    return Resource.TrialPeriodExpired;

                string end;
                var num = count%100;

                if (num >= 11 && num <= 19)
                {
                    end = Resource.DaysTwo;
                }
                else
                {
                    var i = count%10;
                    switch (i)
                    {
                        case (1):
                            end = Resource.Day;
                            break;
                        case (2):
                        case (3):
                        case (4):
                            end = Resource.DaysOne;
                            break;
                        default:
                            end = Resource.DaysTwo;
                            break;
                    }
                }
                return string.Format(Resource.TrialPeriod, count, end);
            }

            if (tariff.State == TariffState.Paid)
            {
                var quota = GetTenantQuota();
                long notifySize;
                long.TryParse(ConfigurationManager.AppSettings["web.tariff-notify.storage"] ?? "314572800", out notifySize); //300 MB
                if (notifySize > 0 && quota.MaxTotalSize - TenantStatisticsProvider.GetUsedSize() < notifySize)
                {
                    return string.Format(Resource.TariffExceedLimit, FileSizeComment.FilesSizeToString(quota.MaxTotalSize));
                }
            }

            return string.Empty;
        }

        public static void TrialRequest()
        {
            CoreContext.PaymentManager.SendTrialRequest(
                TenantProvider.CurrentTenantID,
                CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID));
        }

        public static int GetRemainingCountUsers()
        {
            return GetTenantQuota().ActiveUsers - TenantStatisticsProvider.GetUsersCount();
        }
    }
}