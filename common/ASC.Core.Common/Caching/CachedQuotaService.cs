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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ASC.Core.Tenants;

namespace ASC.Core.Caching
{
    public class CachedQuotaService : IQuotaService
    {
        private const string KEY_QUOTA = "quota";
        private const string KEY_QUOTA_ROWS = "quotarows";
        private readonly IQuotaService service;
        private readonly ICache cache;
        private readonly TrustInterval interval;
        private int syncQuotaRows;


        public TimeSpan CacheExpiration
        {
            get;
            set;
        }


        public CachedQuotaService(IQuotaService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            this.service = service;
            this.cache = new AspCache();
            this.interval = new TrustInterval();
            this.syncQuotaRows = 0;

            CacheExpiration = TimeSpan.FromMinutes(10);
        }


        private Hashtable GetTenantQuotasInernal()
        {
            var key = "quota";
            var store = cache.Get(key) as Hashtable;
            if (store == null)
            {
                store = Hashtable.Synchronized(new Hashtable());
                foreach (var q in service.GetTenantQuotas())
                {
                    store[q.Id] = q;
                }
                cache.Insert(key, store, DateTime.UtcNow.Add(CacheExpiration));
            }
            return store;
        }

        public IEnumerable<TenantQuota> GetTenantQuotas()
        {
            return GetTenantQuotasInernal().Values.Cast<TenantQuota>();
        }

        public TenantQuota GetTenantQuota(int tenant)
        {
            var store = GetTenantQuotasInernal();
            return (TenantQuota)store[tenant];
        }

        public TenantQuota SaveTenantQuota(TenantQuota quota)
        {
            quota = service.SaveTenantQuota(quota);
            var store = GetTenantQuotasInernal();
            store[quota.Id] = quota;
            return quota;
        }

        public void RemoveTenantQuota(int tenant)
        {
            service.RemoveTenantQuota(tenant);
            var store = GetTenantQuotasInernal();
            store.Remove(tenant);
        }


        public void SetTenantQuotaRow(TenantQuotaRow row, bool exchange)
        {
            service.SetTenantQuotaRow(row, exchange);
            interval.Expire();
        }

        public IEnumerable<TenantQuotaRow> FindTenantQuotaRows(TenantQuotaRowQuery query)
        {
            if (query == null) throw new ArgumentNullException("query");

            if (Interlocked.CompareExchange(ref syncQuotaRows, 1, 0) == 0)
            {
                try
                {
                    var rows = cache.Get(KEY_QUOTA_ROWS) as Dictionary<string, List<TenantQuotaRow>>;
                    if (rows == null || interval.Expired)
                    {
                        var date = rows != null ? interval.StartTime : DateTime.MinValue;
                        interval.Start(CacheExpiration);

                        var changes = service.FindTenantQuotaRows(new TenantQuotaRowQuery(Tenant.DEFAULT_TENANT).WithLastModified(date))
                            .GroupBy(r => r.Tenant.ToString())
                            .ToDictionary(g => g.Key, g => g.ToList());

                        // merge changes from db to cache
                        if (rows == null)
                        {
                            rows = changes;
                        }
                        else
                        {
                            foreach (var p in changes)
                            {
                                if (rows.ContainsKey(p.Key))
                                {
                                    var cachedRows = rows[p.Key];
                                    foreach (var r in p.Value)
                                    {
                                        cachedRows.RemoveAll(c => c.Path == r.Path);
                                        cachedRows.Add(r);
                                    }
                                }
                                else
                                {
                                    rows[p.Key] = p.Value;
                                }
                            }
                        }

                        cache.Insert(KEY_QUOTA_ROWS, rows, DateTime.UtcNow.Add(CacheExpiration));
                    }
                }
                finally
                {
                    syncQuotaRows = 0;
                }
            }
            var quotaRows = cache.Get(KEY_QUOTA_ROWS) as IDictionary<string, List<TenantQuotaRow>>;
            if (quotaRows == null) return new TenantQuotaRow[0];

            lock (quotaRows)
            {
                var list = quotaRows.ContainsKey(query.Tenant.ToString()) ?
                    quotaRows[query.Tenant.ToString()] :
                    new List<TenantQuotaRow>();

                if (query != null && !string.IsNullOrEmpty(query.Path))
                {
                    return list.Where(r => query.Path == r.Path);
                }
                return list.ToList();
            }
        }
    }
}
