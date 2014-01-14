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
using System.Web.Configuration;

namespace ASC.Web.Core.Client
{
    public static class ClientSettings
    {
        private static bool? bundlesEnabled;
        private static bool? storeEnabled;
        private static bool? gzipEnabled;

        public static bool BundlingEnabled
        {
            get
            {
                if (!bundlesEnabled.HasValue)
                {
                    bundlesEnabled = bool.Parse(WebConfigurationManager.AppSettings["web.client.bundling"] ?? "false");
                }
                return bundlesEnabled.Value;
            }
        }

        public static String ResetCacheKey
        {
            get
            {
                return WebConfigurationManager.AppSettings["web.client.cache.resetkey"] ?? "1";
            }
        }

        public static bool StoreBundles
        {
            get
            {
                if (!storeEnabled.HasValue)
                {
                    storeEnabled = bool.Parse(WebConfigurationManager.AppSettings["web.client.store"] ?? "false");
                }
                return storeEnabled.Value;
            }
        }

        public static String StorePath
        {
            get { return WebConfigurationManager.AppSettings["web.client.store.path"] ?? "/App_Data/static/"; }

        }

        public static bool GZipEnabled
        {
            get
            {
                if (!gzipEnabled.HasValue)
                {
                    gzipEnabled = bool.Parse(WebConfigurationManager.AppSettings["web.client.store.gzip"] ?? "true");
                }
                return StoreBundles && gzipEnabled.Value;
            }
        }
    }
}