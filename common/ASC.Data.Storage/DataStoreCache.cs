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

using System.Web;
using System.Web.Caching;

namespace ASC.Data.Storage
{
    internal static class DataStoreCache
    {
        internal static void Put(IDataStore store, string tenantId, string module)
        {
            HttpRuntime.Cache.Add(MakeCacheKey(tenantId, module), store, null, Cache.NoAbsoluteExpiration,
                                  Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
        }

        internal static IDataStore Get(string tenantId, string module)
        {
            return HttpRuntime.Cache.Get(MakeCacheKey(tenantId, module)) as IDataStore;
        }


        private static string MakeCacheKey(string tennantId, string module)
        {
            return string.Format("{0}:\\{1}", tennantId, module);
        }
    }
}