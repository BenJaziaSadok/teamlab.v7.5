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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ASC.Api.Interfaces;
using ASC.Api.Interfaces.Storage;

namespace ASC.Api.Impl
{
    internal class ApiKeyValueInMemoryStorage : IApiKeyValueStorage
    {
        private static readonly Hashtable Cache = Hashtable.Synchronized(new Hashtable());

        #region IApiKeyValueStorage Members

        public object Get(IApiEntryPoint entrypoint, string key)
        {
            return Cache[GetKey(entrypoint, key)];
        }

        public void Set(IApiEntryPoint entrypoint, string key, object @object)
        {
            Cache[GetKey(entrypoint, key)] = @object;
        }

        public bool Exists(IApiEntryPoint entrypoint, string key)
        {
            return Cache.ContainsKey(GetKey(entrypoint, key));
        }

        public void Remove(IApiEntryPoint entrypoint, string key)
        {
            Cache.Remove(GetKey(entrypoint, key));
        }

        public void Clear(IApiEntryPoint entrypoint)
        {
            IEnumerable<string> toRemove = Cache.Keys.Cast<string>().Where(x => x.StartsWith(entrypoint.GetType() + ":"));
            foreach (string remove in toRemove)
            {
                Cache.Remove(remove);
            }
        }

        #endregion

        private static string GetKey(IApiEntryPoint point, string key)
        {
            return point.GetType() + ":" + key;
        }
    }
}