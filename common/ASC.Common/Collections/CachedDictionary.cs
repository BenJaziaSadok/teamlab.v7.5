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
using System.Diagnostics;
using System.Web;
using System.Web.Caching;

namespace ASC.Collections
{
    public sealed class CachedDictionary<T> : CachedDictionaryBase<T>
    {
        private readonly DateTime _absoluteExpiration;
        private readonly TimeSpan _slidingExpiration;

        public CachedDictionary(string baseKey, DateTime absoluteExpiration, TimeSpan slidingExpiration,
                                Func<T, bool> cacheCodition)
        {
            if (cacheCodition == null) throw new ArgumentNullException("cacheCodition");
            _baseKey = baseKey;
            _absoluteExpiration = absoluteExpiration;
            _slidingExpiration = slidingExpiration;
            _cacheCodition = cacheCodition;
            InsertRootKey(_baseKey);
        }

        public CachedDictionary(string baseKey)
            : this(baseKey, (x) => true)
        {
        }

        public CachedDictionary(string baseKey, Func<T, bool> cacheCodition)
            : this(baseKey, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, cacheCodition)
        {
        }

        protected override void InsertRootKey(string rootKey)
        {
#if (DEBUG)
            Debug.Print("inserted root key {0}", rootKey);
#endif
            HttpRuntime.Cache.Insert(rootKey, DateTime.UtcNow.Ticks, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration,
                                     CacheItemPriority.NotRemovable,(key,value,reason)=> Debug.Print("gloabl root key: {0} removed. reason: {1}", key, reason));
        }

        public override void Reset(string rootKey, string key)
        {
            HttpRuntime.Cache.Remove(BuildKey(key, rootKey));
        }

        protected override object GetObjectFromCache(string fullKey)
        {
            return HttpRuntime.Cache.Get(fullKey);
        }

        public override void Add(string rootkey, string key, T newValue)
        {
            var builtrootkey = BuildKey(string.Empty, string.IsNullOrEmpty(rootkey)?"root":rootkey);
            if (HttpRuntime.Cache[builtrootkey] == null)
            {
#if (DEBUG)
                Debug.Print("added root key {0}",builtrootkey);
#endif
                //Insert root if no present
                HttpRuntime.Cache.Insert(builtrootkey,DateTime.UtcNow.Ticks, null, _absoluteExpiration, _slidingExpiration,
                                         CacheItemPriority.NotRemovable, (removedkey, value, reason) => Debug.Print("root key: {0} removed. reason: {1}", removedkey, reason));
            }
            CacheItemRemovedCallback removeCallBack = null;
#if (DEBUG)
            removeCallBack = ItemRemoved;
#endif
            if (newValue != null)
            {
                HttpRuntime.Cache.Insert(BuildKey(key, rootkey), newValue,
                                         new CacheDependency(null, new[] {_baseKey, builtrootkey}),
                                         _absoluteExpiration, _slidingExpiration,
                                         CacheItemPriority.Normal, removeCallBack);
            }
            else
            {
                HttpRuntime.Cache.Remove(BuildKey(key, rootkey));//Remove if null
            }
        }

        private static void ItemRemoved(string key, object value, CacheItemRemovedReason reason)
        {
            Debug.Print("key: {0} removed. reason: {1}",key,reason);
        }
    }
}