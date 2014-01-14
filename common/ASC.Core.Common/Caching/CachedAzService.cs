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
using ASC.Core.Common.Caching;

namespace ASC.Core.Caching
{
    public class CachedAzService : IAzService
    {
        private readonly IAzService service;
        private readonly ICache cache;

        public TimeSpan CacheExpiration { get; set; }

        public CachedAzService(IAzService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            this.service = service;
            this.cache = new AspCache();

            CacheExpiration = TimeSpan.FromMinutes(10);
        }

        public IEnumerable<AzRecord> GetAces(int tenant, DateTime from)
        {
            var key = GetKey(tenant);
            var aces = cache.Get(key) as AzRecordStore;
            if (aces == null)
            {
                aces = cache.Get(key) as AzRecordStore;
                if (aces == null)
                {
                    var records = service.GetAces(tenant, default(DateTime));
                    cache.Insert(key, aces = new AzRecordStore(records), DateTime.UtcNow.Add(CacheExpiration));
                }
            }
            return aces;
        }

        public AzRecord SaveAce(int tenant, AzRecord r)
        {
            r = service.SaveAce(tenant, r);
            var aces = cache.Get(GetKey(tenant)) as AzRecordStore;
            if (aces != null)
            {
                lock (aces)
                {
                    aces.Add(r);
                }
            }
            return r;
        }

        public void RemoveAce(int tenant, AzRecord r)
        {
            service.RemoveAce(tenant, r);
            var aces = cache.Get(GetKey(tenant)) as AzRecordStore;
            if (aces != null)
            {
                lock (aces)
                {
                    aces.Remove(r);
                }
            }
        }


        private string GetKey(int tenant)
        {
            return "acl" + tenant.ToString();
        }
    }
}