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

namespace ASC.Core.Caching
{
    public class AspCache : ICache
    {
        public object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public void Insert(string key, object value, TimeSpan sligingExpiration)
        {
            HttpRuntime.Cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, sligingExpiration);
        }

        public void Insert(string key, object value, DateTime absolutExpiration)
        {
            HttpRuntime.Cache.Insert(key, value, null, absolutExpiration, Cache.NoSlidingExpiration);
        }

        public object Remove(string key)
        {
            return HttpRuntime.Cache.Remove(key);
        }
    }
}
