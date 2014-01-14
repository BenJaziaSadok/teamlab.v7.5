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

#region Import

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ASC.Collections;
using System.Collections.Specialized;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Data.Storage;
using ASC.Web.Studio.Utility;
using Newtonsoft.Json;
using log4net;

#endregion

namespace ASC.Web.Core.Client.Bundling
{
    public class Bundler
    {
        private static readonly ConcurrentDictionary<String, String> _cacheUri = new ConcurrentDictionary<String, String>();
        private static readonly Object _locker = new Object();
        private readonly ILog _log = LogManager.GetLogger("ASC.Web.BundlerHandler");
        private static readonly String _pathToCacheFile = String.Empty;


        static Bundler()
        {
            var hashResetKey = HttpServerUtility.UrlTokenEncode(MD5.Create()
                                  .ComputeHash(Encoding.UTF8.GetBytes(ClientSettings.ResourceResetCacheKey)))
                                  .Replace("/", "_");

            var fileName = String.Format("info_{0}.txt", hashResetKey);

            _pathToCacheFile = HttpContext.Current.Server.MapPath(String.Concat("~/", ClientSettings.ResourceStorePath, "/bundle/", fileName));

            if (!File.Exists(_pathToCacheFile)) return;

            var jsonString = File.ReadAllText(_pathToCacheFile);

            _cacheUri = new ConcurrentDictionary<string, string>(JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonString));
        }

        private String GetCategoryFromPath(String controlPath)
        {
            var result = "common";

            controlPath = controlPath.ToLower();
            var matches = Regex.Match(controlPath, "(products|addons)/(\\w+)/?", RegexOptions.Compiled);

            if (matches.Success && matches.Groups.Count > 2 && matches.Groups[2].Success)
                result = matches.Groups[2].Value;

            return result;
        }


        private String GetStorePath(HttpContextBase context, String category, String uriString, ContentType contentType)
        {
            if (String.IsNullOrEmpty(category))
            {
                if (context.Request.Url != null)
                    category = GetCategoryFromPath(context.Request.Url.AbsolutePath);
                else if (String.IsNullOrEmpty(category))
                    category = "common";
            }

            var filePath = GetFullFileName(category, uriString, contentType);

            var cacheKey = String.Format("{0}-{1}", category, filePath);

            if (_cacheUri.ContainsKey(cacheKey)) return _cacheUri[cacheKey];

            if (!StaticDataStorage.IsFile("common_static", filePath))
            {
                lock (_locker)
                {
                    if (_cacheUri.ContainsKey(cacheKey)) return _cacheUri[cacheKey];

                    var requestUri = uriString;

                    if (Uri.IsWellFormedUriString(uriString, UriKind.Relative))
                    {
                        var u = context.Request.GetUrlRewriter();
                        var uriBuilder = new UriBuilder(u.Scheme, u.Host, u.Port, uriString);

                        requestUri = uriBuilder.ToString();
                    }

                    try
                    {
                        var req = (HttpWebRequest)WebRequest.Create(requestUri);

                        var currentTenant = CoreContext.TenantManager.GetCurrentTenant(false);

                        if (currentTenant != null && currentTenant.TenantId > -1)
                        {
                            req.CookieContainer = new CookieContainer();

                            var cookieDomain = CoreContext.TenantManager.GetCurrentTenant().TenantDomain;

                            if (req.RequestUri.Host.ToLower() == "localhost")
                                cookieDomain = "localhost";

                            req.CookieContainer.Add(new Cookie("asc_auth_key", CookiesManager.GetCookies(CookiesType.AuthKey), "/", cookieDomain));

                        }

                        using (var resp = (HttpWebResponse)req.GetResponse())
                        using (var stream = resp.GetResponseStream())
                        {
                            if (resp.StatusCode != HttpStatusCode.OK)
                            {
                                throw new HttpException((int)resp.StatusCode, resp.StatusDescription);
                            }
                            if (ClientSettings.IsGZipEnabled)
                            {
                                var compressedFileStream = new MemoryStream();

                                using (var compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress, true))
                                {
                                    stream.CopyTo(compressionStream);
                                }

                                Uri fileUri = StaticDataStorage.Save(String.Empty, filePath, compressedFileStream, "gzip", 365);

                            }
                            else
                            {
                                StaticDataStorage.Save(filePath, stream);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        _log.Error(exception);
                        _log.Error("Current Uri: " + context.Request.GetUrlRewriter().ToString());
                        _log.Error("Request Uri: " + requestUri);
                        throw;
                    }
                }
            }

            //HACK: support  for multi-cdn
            var result = WebPath.GetPath(filePath);

            if (!_cacheUri.ContainsKey(cacheKey))
            {
                _cacheUri.TryAdd(cacheKey, result);

                File.WriteAllText(_pathToCacheFile, JsonConvert.SerializeObject(_cacheUri));
            }

            return result;
        }

        public static Collections.ReadOnlyDictionary<String, List<String>> GetReadOnlyCache()
        {
            var cloneCachePath = new Dictionary<String, String>(_cacheUri);

            if (cloneCachePath.Keys.Count == 0) return (new Dictionary<string, List<string>>()).AsReadOnly();

            var result = new Dictionary<String, List<String>>();

            foreach (var keyValuePair in cloneCachePath)
            {
                var category = keyValuePair.Key.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries)[0];

                if (!result.ContainsKey(category))
                    result.Add(category, new List<String>());

                result[category].Add(keyValuePair.Value);

            }

            return result.AsReadOnly();
        }

        public string ProcessBundle(String categoryName, String html, HttpContextBase context)
        {
            //Calculate MD5 of string and register it to handler
            var tokenKey = HttpServerUtility.UrlTokenEncode(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(html)));
            var bundle = BundlerHandler.Instance.RegisterBundle(tokenKey, html, context);
            if (bundle != null)
            {
                var builder = new StringBuilder();

                if (bundle.BundleStyles.Any())
                {
                    foreach (var styleReference in bundle.StyleReferences)
                    {
                        var href = VirtualPathUtility.ToAbsolute("~/" + styleReference);

                        if (ClientSettings.IsResourceStoreEnabled)
                        {
                            href = GetStorePath(context, categoryName, href, new ContentType { MediaType = "text/css" });
                        }

                        builder.AppendFormat("<link type=\"text/css\" href=\"{0}\" rel=\"stylesheet\" />",
                                              href);
                    }
                }

                if (bundle.BundleScripts.Any())
                {
                    foreach (var scriptReference in bundle.ScriptReferences)
                    {
                        var href = VirtualPathUtility.ToAbsolute("~/" + scriptReference);

                        if (ClientSettings.IsResourceStoreEnabled)
                        {
                            href = GetStorePath(context, categoryName, href, new ContentType { MediaType = "text/javascript" });
                        }

                        builder.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>",
                                             href);
                    }
                }
                return builder.ToString();
            }
            return html;
        }

        private static String GetFullFileName(String category, String href, ContentType contentType)
        {
            var hrefTokenSource = href;

            if (Uri.IsWellFormedUriString(href, UriKind.Relative))
                hrefTokenSource = (SecureHelper.IsSecure() ? "https" : "http") + href;

            var hrefToken = String.Concat(HttpServerUtility.UrlTokenEncode(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(hrefTokenSource))));

            if (String.Compare(contentType.MediaType, "text/css", StringComparison.OrdinalIgnoreCase) == 0)
                return ClientSettings.ResourceStorePath + String.Format("bundle/{0}/css/{1}.css", category, hrefToken);

            return ClientSettings.ResourceStorePath + String.Format("bundle/{0}/javascript/{1}.js", category, hrefToken);

        }


        private IDataStore StaticDataStorage
        {
            get { return StorageFactory.GetStorage(Tenant.DEFAULT_TENANT.ToString(CultureInfo.InvariantCulture), "common_static"); }
        }
    }
}