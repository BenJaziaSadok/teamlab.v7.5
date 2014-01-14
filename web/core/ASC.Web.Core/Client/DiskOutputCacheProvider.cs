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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using ASC.Common.Utils;
using System.Web;
using System.Web.Caching;

namespace ASC.Web.Core.Client
{
    public class DiskOutputCacheProvider : OutputCacheProvider
    {
        [Serializable]
        public class CacheItem
        {
            public object Item { get; set; }
            public DateTime Expiry { get; set; }
        }

        private string CacheLocation
        {
            get
            {
                string strCacheLocation = ClientSettings.ResourceStorePath;
                strCacheLocation = HttpContext.Current.Server.MapPath("~/" + strCacheLocation);

                return String.Concat(strCacheLocation, "controls/");
            }
        }

        private string GetFullPathForKey(string key)
        {
            var hashResetKey = HttpServerUtility.UrlTokenEncode(MD5.Create()
                                   .ComputeHash(Encoding.UTF8.GetBytes(ClientSettings.ResourceResetCacheKey)));
            
            return String.Concat(CacheLocation, key.Replace('/', '$'), "_", hashResetKey);

        }

        public override object Get(string key)
        {
            string filePath = GetFullPathForKey(key);
            if (!File.Exists(filePath))
            {
                return null;
            }
            CacheItem item = null;

            using (var fileStream = File.OpenRead(filePath))
            {
                var formatter = new BinaryFormatter();
                item = (CacheItem)formatter.Deserialize(fileStream);
            }

            if (item.Expiry <= DateTime.UtcNow)
            {
                Remove(key);

                return null;
            }

            return item.Item;
        }

        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            var obj = Get(key);
            
            if (obj != null)
            {
                return obj;
            }
            
            Set(key, entry, utcExpiry);
            
            return entry;
        }

        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            string filePath = GetFullPathForKey(key);
            var item = new CacheItem { Expiry = utcExpiry, Item = entry };

            using (var fileStream = File.OpenWrite(filePath))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, item);
            }
        }

        public override void Remove(string key)
        {
            var filePath = GetFullPathForKey(key);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

}
