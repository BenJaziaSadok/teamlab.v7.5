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

namespace ASC.Collections
{
    public sealed class HttpRequestDictionary<T> : CachedDictionaryBase<T>
    {
        private class CachedItem
        {
            internal T Value { get; set; }

            internal CachedItem(T value)
            {
                Value = value;
            }
        }

        public HttpRequestDictionary(string baseKey)
        {
            _cacheCodition = (T) => true;
            _baseKey = baseKey;
        }

        protected override void InsertRootKey(string rootKey)
        {
            //We can't expire in HtppContext in such way
        }

        public override void Reset(string rootKey, string key)
        {
            if (HttpContext.Current != null)
            {
                var builtkey = BuildKey(key, rootKey);
                HttpContext.Current.Items[builtkey] = null;
            }
        }

        public override void Add(string rootkey, string key, T newValue)
        {
            if (HttpContext.Current != null)
            {
                var builtkey = BuildKey(key, rootkey);
                HttpContext.Current.Items[builtkey] = new CachedItem(newValue);
            }
        }

        protected override object GetObjectFromCache(string fullKey)
        {
            return HttpContext.Current != null ? HttpContext.Current.Items[fullKey] : null;
        }

        protected override bool FitsCondition(object cached)
        {
            return cached is CachedItem;
        }
        protected override T ReturnCached(object objectCache)
        {
            return ((CachedItem)objectCache).Value;
        }

        protected override void OnHit(string fullKey)
        {
            Debug.Print("{0} http cache hit:{1}", HttpContext.Current.Request.Url, fullKey);
        }

        protected override void OnMiss(string fullKey)
        {
            Uri uri = null;
            if (HttpContext.Current != null)
            {
                uri = HttpContext.Current.Request.Url;
            }
            Debug.Print("{0} http cache miss:{1}", uri == null ? "no-context" : uri.AbsolutePath, fullKey);
        }

    }
}