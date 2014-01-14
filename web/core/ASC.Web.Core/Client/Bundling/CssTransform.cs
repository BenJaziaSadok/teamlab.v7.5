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

using dotless.Core;
using dotless.Core.configuration;
using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;

namespace ASC.Web.Core.Client.Bundling
{
    class CssTransform : IItemTransform
    {
        private readonly string bundlepath;


        public CssTransform(string bundlepath)
        {
            this.bundlepath = bundlepath;
        }


        public string Process(string includedVirtualPath, string input)
        {
            if (VirtualPathUtility.GetExtension(includedVirtualPath) == ".less")
            {
                var cfg = DotlessConfiguration.GetDefaultWeb();
                cfg.Web = true;
                cfg.MinifyOutput = BundleTable.EnableOptimizations;
                cfg.MapPathsToWeb = true;
                cfg.CacheEnabled = false;

                var importRegex = new Regex(@"@import (.*?);", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
                input = importRegex.Replace(input, m => ResolveImportLessPath(includedVirtualPath, m));
                input = LessWeb.Parse(input, cfg);
            }

            var urlRegex = new Regex(@"url\((.*?)\)", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            input = urlRegex.Replace(input, m => ResolveUrlPath(includedVirtualPath, m));
            return input;
        }

        private string ResolveImportLessPath(string virtualPath, Match m)
        {
            if (m.Success && m.Groups[1].Success)
            {
                var path = m.Groups[1].Value.Trim().Trim('\'', '"');
                var vpath = VirtualPathUtility.Combine(VirtualPathUtility.GetDirectory(virtualPath), path);
                return m.Value.Replace(m.Groups[1].Value, "\"" + vpath + "\"");
            }
            return m.Value;
        }

        private string ResolveUrlPath(string virtualPath, Match m)
        {
            if (m.Success && m.Groups[1].Success)
            {
                var path = m.Groups[1].Value.Trim().Trim('\'', '"');
                if (path.StartsWith("data:", StringComparison.InvariantCultureIgnoreCase))
                {
                    return m.Value;
                }
                if (Uri.IsWellFormedUriString(path, UriKind.Relative))
                {
                    path = VirtualPathUtility.Combine(VirtualPathUtility.GetDirectory(virtualPath), path);
                }
                else if (path.StartsWith(Uri.UriSchemeHttp))
                {
                    path = new Uri(path).PathAndQuery;
                }
                if (HostingEnvironment.ApplicationVirtualPath != "/" && path.StartsWith(HostingEnvironment.ApplicationVirtualPath))
                {
                    path = path.Substring(HostingEnvironment.ApplicationVirtualPath.Length);
                }
                if (path.StartsWith("/"))
                {
                    path = "~" + path;
                }
                path = VirtualPathUtility.MakeRelative(bundlepath, path).ToLowerInvariant();
                return m.Value.Replace(m.Groups[1].Value, "\"" + path + "\"");
            }
            return m.Value;
        }
    }
}