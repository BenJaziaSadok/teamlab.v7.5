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
using System.Runtime.Serialization;
using ASC.Core.Tenants;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.UserControls.Statistics;

namespace ASC.Api.Settings
{
    [DataContract(Name = "quota", Namespace = "")]
    public class QuotaWrapper
    {
        [DataMember]
        public ulong StorageSize { get; set; }

        [DataMember]
        public ulong MaxFileSize { get; set; }

        [DataMember]
        public ulong UsedSize { get; set; }

        [DataMember]
        public int MaxUsersCount { get; set; }

        [DataMember]
        public int UsersCount { get; set; }

        [DataMember]
        public ulong AvailableSize
        {
            get { return Math.Max(0, StorageSize - UsedSize); }
        }

        [DataMember]
        public int AvailableUsersCount
        {
            get { return Math.Max(0, MaxUsersCount - UsersCount); }
        }

        [DataMember]
        public IList<QuotaUsage> StorageUsage { get; set; }

        private QuotaWrapper()
        {
            
        }

        public QuotaWrapper(TenantQuota quota, IList<TenantQuotaRow> quotaRows)
        {
            StorageSize = (ulong) Math.Max(0, quota.MaxTotalSize);
            UsedSize = (ulong) Math.Max(0, quotaRows.Sum(r => r.Counter));
            MaxFileSize = Math.Min(AvailableSize, (ulong)quota.MaxFileSize);
            MaxUsersCount = TenantExtra.GetTenantQuota().ActiveUsers;
            UsersCount = TenantStatisticsProvider.GetUsersCount();

            StorageUsage =
                quotaRows.Select(x => new QuotaUsage() {Path = x.Path.TrimStart('/').TrimEnd('/'), Size = x.Counter,}).
                    ToList();
        }

        public static QuotaWrapper GetSample()
        {
            return new QuotaWrapper()
                       {
                           MaxFileSize = 25 * 1024 * 1024, 
                           StorageSize = 1024 * 1024 * 1024,
                           UsedSize = 250 * 1024 * 1024,
                           StorageUsage = new List<QuotaUsage>()
                                              {
                                                  new QuotaUsage(){Size = 100 * 1024 * 1024,Path = "crm"},
                                                  new QuotaUsage(){Size = 150 * 1024 * 1024,Path = "files"}
                                              }
                       };
        }

}
}