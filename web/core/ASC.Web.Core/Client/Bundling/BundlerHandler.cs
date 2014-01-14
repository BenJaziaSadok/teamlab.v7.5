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
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Routing;
using ASC.Data.Storage;
using HtmlAgilityPack;
using log4net;

namespace ASC.Web.Core.Client.Bundling
{
    public class BundlerHandler
    {
        private static volatile BundlerHandler _instance;
        private static readonly object SyncLock = new object();
        private readonly ILog _log = LogManager.GetLogger("ASC.Web.BundlerHandler");


        private BundlerHandler()
        {
            //Register route
            RouteTable.Routes.Add(new Route("bundle/{type}/{hash}/{version}" + ClientSettings.BundleExtension,
                                            new BundleRouteHandler(token => HttpRuntime.Cache.Get(token) as Bundle)));
        }

        public static BundlerHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new BundlerHandler();
                        }
                    }
                }
                return _instance;
            }
        }


        public Bundle RegisterBundle(string tokenKey, string html, HttpContextBase context)
        {
            return HttpRuntime.Cache.Get(tokenKey) as Bundle ?? BuildBundle(tokenKey, html, context);
        }

        private Bundle BuildBundle(string tokenKey, string html, HttpContextBase context)
        {
            var bundle = new Bundle();

            //Parse html and register for processing
            var document = new HtmlDocument();
            document.LoadHtml(html);
            HtmlNodeCollection styles = document.DocumentNode.SelectNodes("/style | /link[@rel='stylesheet'] | /link[@rel='stylesheet/less']");
            //Build a version based on files or content
            var hash = new StringBuilder();
            if (styles != null)
            {
                foreach (HtmlNode style in styles)
                {
                    if (style.Name == "style" && !string.IsNullOrEmpty(style.InnerHtml))
                    {
                        if (ClientSettings.IsResourceStoreEnabled)
                        {
                            throw new Exception("Resource Bundle Control should not contain Embedded Style");
                        }

                        //Inner style
                        hash.Append(style.InnerHtml);
                        bundle.BundleStyles.Add(new BundleResource(BundleResourceType.EmbeddedStyle, style.InnerHtml));
                    }
                    if (style.Name == "link")
                    {
                        string serverPath = style.Attributes["href"].Value;

                        if (Uri.IsWellFormedUriString(serverPath, UriKind.Absolute))
                            serverPath = WebPath.GetRelativePath(serverPath);

                        //Append refernce
                        if (Uri.IsWellFormedUriString(serverPath, UriKind.Relative))
                        {
                            //Get file path
                            string path = context.Server.MapPath(serverPath);
                            if (File.Exists(path))
                            {
                                bundle.BundleStyles.Add(
                                    new BundleResource(
                                        Path.GetExtension(path) == ".less"
                                            ? BundleResourceType.LessFile
                                            : BundleResourceType.StyleFile, path, serverPath));
                                hash.Append(new FileInfo(path).LastWriteTimeUtc.ToFileTimeUtc());
                            }
                        }
                        else
                            throw new UriFormatException("Resource Bundle Control should not contain absolute style uri " + serverPath);

                    }
                }
            }

            var scripts = document.DocumentNode.SelectNodes("/script");
            if (scripts != null)
            {
                foreach (HtmlNode script in scripts)
                {
                    if (script.Attributes["src"] == null && !string.IsNullOrEmpty(script.InnerHtml))
                    {
                        if (ClientSettings.IsResourceStoreEnabled)
                        {
                            throw new Exception("Resource Bundle Control should not contain Embedded Script");
                        }

                        //Inner style
                        bundle.BundleScripts.Add(new BundleResource(BundleResourceType.EmbeddedScript, script.InnerHtml));
                        hash.Append(script.InnerHtml);
                    }
                    else
                    {
                        //Append refernce
                        string serverPath = script.Attributes["src"].Value;

                        if (Uri.IsWellFormedUriString(serverPath, UriKind.Absolute))
                            serverPath = WebPath.GetRelativePath(serverPath);

                        if (Uri.IsWellFormedUriString(serverPath, UriKind.Relative))
                        {
                            //Get file path
                            string path = context.Server.MapPath(serverPath);
                            if (File.Exists(path))
                            {
                                var obfuscateJs = script.Attributes["notobfuscate"] == null;

                                bundle.BundleScripts.Add(new BundleResource(BundleResourceType.ScriptFile, path, serverPath) { ObfuscateJs = obfuscateJs });
                                hash.Append(new FileInfo(path).LastWriteTimeUtc.ToFileTimeUtc());
                            }
                            else if(serverPath.Contains("/clientscriptbundle/"))
                            {
                                bundle.BundleScripts.Add(new BundleResource(BundleResourceType.ClientScript, path, serverPath));
                                hash.Append(serverPath);
                            }
                        }
                        else
                            throw new UriFormatException("Resource Bundle Control should not contain absolute script uri " + serverPath);
                        
                    }
                }
            }
            //Get bundle version
            bundle.Hash = HttpServerUtility.UrlTokenEncode(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(hash.ToString())));
            //Register in cache
            var dependencyFiles = bundle.BundleScripts.Where(x => x.Type == BundleResourceType.ScriptFile)
                .Select(x => x.Content)
                .Union(
                bundle.BundleStyles.Where(x => x.Type == BundleResourceType.StyleFile || x.Type == BundleResourceType.LessFile).Select(x => x.Content));

            HttpRuntime.Cache.Insert(tokenKey,
                                     bundle,
                                     new CacheDependency(dependencyFiles.ToArray()),
                                     Cache.NoAbsoluteExpiration,
                                     TimeSpan.FromDays(1),
                                     CacheItemPriority.Normal,
                                     OnBundleRemoved);

            //Set urls 
            bundle.ScriptReferences = new[]
                                          {
                                              string.Format("bundle/script/{0}/{1}" + ClientSettings.BundleExtension,
                                                            tokenKey, bundle.Hash)
                                          };
            bundle.StyleReferences = new[]
                                         {
                                             string.Format("bundle/style/{0}/{1}" + ClientSettings.BundleExtension,
                                                           tokenKey, bundle.Hash)
                                         };
            return bundle;
        }

        private void OnBundleRemoved(string key, object value, CacheItemRemovedReason reason)
        {
            _log.InfoFormat("Bundle {0} was removed. reason:{1}", key, reason);
        }

        #region Nested type: BundleRouteHandler

        private class BundleRouteHandler : IRouteHandler
        {
            private readonly Func<string, Bundle> _bundleRetrive;

            public BundleRouteHandler(Func<string, Bundle> bundleRetrive)
            {
                if (bundleRetrive == null) throw new ArgumentNullException("bundleRetrive");
                _bundleRetrive = bundleRetrive;
            }

            #region IRouteHandler Members

            public IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                var token = requestContext.RouteData.Values["hash"] as string;
                var type = requestContext.RouteData.Values["type"] as string;
                var version = requestContext.RouteData.Values["version"] as string;
                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(version))
                {
                    Bundle bundle = _bundleRetrive(token);
                    if (bundle.Hash == version)
                    {
                        if (type == "style" && bundle.BundleStyles.Any())
                            return new BundlerHttpHandler(bundle.BundleStyles, token, version,
                                                          new ContentType("text/css") {CharSet = Encoding.UTF8.WebName});
                        if (type == "script" && bundle.BundleScripts.Any())
                            return new BundlerHttpHandler(bundle.BundleScripts, token, version,
                                                          new ContentType("application/x-javascript")
                                                              {CharSet = Encoding.UTF8.WebName});
                    }
                }
                throw new NotSupportedException();
            }

            #endregion
        }

        #endregion
    }
}