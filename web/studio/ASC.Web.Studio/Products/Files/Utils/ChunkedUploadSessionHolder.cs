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
using System.Web;
using System.Web.Caching;
using ASC.Core;
using ASC.Files.Core;
using ASC.Web.Files.Classes;

namespace ASC.Web.Files.Utils
{
    internal static class ChunkedUploadSessionHolder
    {
        private static readonly DateTime AbsoluteExpiration = Cache.NoAbsoluteExpiration;
        public static readonly TimeSpan SlidingExpiration = TimeSpan.FromHours(12);

        public static void StoreSession(ChunkedUploadSession uploadSession)
        {
            HttpRuntime.Cache.Insert(uploadSession.Id, uploadSession, null, AbsoluteExpiration, SlidingExpiration, OnCacheItemRemoved);
        }

        public static void RemoveSession(ChunkedUploadSession uploadSession)
        {
            HttpRuntime.Cache.Remove(uploadSession.Id);
        }

        public static ChunkedUploadSession GetSession(string sessionId)
        {
            return HttpRuntime.Cache.Get(sessionId) as ChunkedUploadSession;
        }

        private static void OnCacheItemRemoved(string key, CacheItemUpdateReason reason, out object obj, out CacheDependency dependency, out DateTime absoluteExpiration, out TimeSpan slidingExpiration)
        {
            var uploadSession = GetSession(key);
            
            CoreContext.TenantManager.SetCurrentTenant(uploadSession.TenantId);
            
            using (var dao = Global.DaoFactory.GetFileDao())
            {
                dao.AbortUploadSession(uploadSession);
            }

            obj = null;
            dependency = null;
            absoluteExpiration = AbsoluteExpiration;
            slidingExpiration = SlidingExpiration;
        }
    }
}
