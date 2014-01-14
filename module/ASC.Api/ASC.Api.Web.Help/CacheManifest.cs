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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;

namespace ASC.Api.Web.Help
{
    public class CacheManifest
    {
        private const string CacheHeader = "CACHE MANIFEST";
        private const string CacheSection = "CACHE:";
        private const string NetworkSection = "NETWORK:";
        private const string FallbackSection = "FALLBACK:";

        private readonly Dictionary<string, HashSet<string>> _mainfestSections = new Dictionary<string, HashSet<string>>();

        private DateTime _lastChange;

        public class StringComparer:IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return string.Equals(x, y, StringComparison.Ordinal);
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }

        public CacheManifest()
        {
            _lastChange = DateTime.UtcNow;
            _mainfestSections.Add(CacheSection, new HashSet<string>(new StringComparer()));
            _mainfestSections.Add(NetworkSection, new HashSet<string>(new StringComparer()) { "*" });
            _mainfestSections.Add(FallbackSection, new HashSet<string>(new StringComparer()));
        }

        public IEnumerable<IGrouping<string, HashSet<string>>> GetRegisteredCacheSections()
        {
            return _mainfestSections.GroupBy(key=>key.Key,value=>value.Value);
        }

        public void AddFallback(Uri uriOnline, Uri uriOffline)
        {
            AddToManifest(FallbackSection, uriOnline, uriOffline);
        }

        public void AddOnline(Uri uri)
        {
            AddToManifest(NetworkSection, uri);
        }

        public void AddCached(Uri uri)
        {
            AddToManifest(CacheSection,uri);
        }

        public void AddServerFile(HttpContextBase context,string virtualPathToFile)
        {
            var physicalPath = context.Server.MapPath(virtualPathToFile);
            var appPhysicalRoot = context.Server.MapPath("~/");
            var url = VirtualPathUtility.ToAbsolute("~/" +
                                              physicalPath.Substring(appPhysicalRoot.Length).Replace("\\", "/").TrimStart('/'));
            AddCached(new Uri(url,UriKind.Relative));
        }

        public void AddServerFolder(HttpContextBase context,string virtualPath,string mask)
        {
            var physicalPath = context.Server.MapPath(virtualPath);
            var appPhysicalRoot = context.Server.MapPath("~/");
            var files = Directory.GetFiles(physicalPath, mask, SearchOption.TopDirectoryOnly).Select(x=>VirtualPathUtility.ToAbsolute("~/"+x.Substring(appPhysicalRoot.Length).Replace("\\","/").TrimStart('/')));
            foreach (var file in files)
            {
                AddCached(new Uri(file,UriKind.Relative));
            }
        }


        protected void AddToManifest(string section,params Uri[] uri)
        {
            var urls = string.Join(" ", uri.Select(x => x.ToString()).ToArray());
            var changed = _mainfestSections[section].Add(urls);
            if (changed)
            {
                _lastChange = DateTime.UtcNow;
            }
        }

        public void Write(TextWriter output)
        {
            output.WriteLine(CacheHeader);
            output.WriteLine("#{0}", _lastChange.ToString("R"));
            output.WriteLine();
            foreach (var mainfestSection in _mainfestSections)
            {
                if (mainfestSection.Value.Any())
                {
                    output.WriteLine(mainfestSection.Key);
                    foreach (var url in mainfestSection.Value)
                    {
                        output.WriteLine(url);
                    }
                    output.WriteLine();
                }
            }
        }
    }
}