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

using ASC.Core;
using ASC.Web.Core.Client.HttpHandlers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Core.Client.Bundling
{
    [ToolboxData("<{0}:ClientScriptReference runat=server></{0}:ClientScriptReference>")]
    public class ClientScriptReference : WebControl
    {
        private static readonly ConcurrentDictionary<string, List<ClientScript>> cache = new ConcurrentDictionary<string, List<ClientScript>>();


        public virtual ICollection<Type> Includes
        {
            get;
            private set;
        }


        public ClientScriptReference()
        {
            Includes = new HashSet<Type>();
        }


        public override void RenderBeginTag(HtmlTextWriter writer)
        {
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            var link = GetLink(false);
            if (ClientSettings.BundlingEnabled)
            {
                var bundle = BundleHelper.GetJsBundle(link);
                if (bundle == null)
                {
                    bundle = BundleHelper.JsBundle(link).Include(link, true);
                    bundle.UseCache = false;
                    BundleHelper.AddBundle(bundle);
                }
                output.Write(BundleHelper.HtmlScript(link));
            }
            else
            {
                output.Write(BundleHelper.HtmlScript(VirtualPathUtility.ToAbsolute(link), false, false));
            }
        }

        public string GetLink(bool onlyLocalization)
        {
            var filename = string.Empty;
            foreach (var type in Includes)
            {
                filename += type.FullName.ToLowerInvariant().Replace('.', '_');
            }
            var filenameHash = GetHash(filename);

            var scripts = new List<ClientScript>();
            if (!cache.TryGetValue(filenameHash, out scripts))
            {
                scripts = Includes.Select(type => (ClientScript)Activator.CreateInstance(type)).ToList();
                cache.TryAdd(filenameHash, scripts);
                if (onlyLocalization && scripts.Any(s => !(s is ClientScriptLocalization)))
                {
                    throw new InvalidOperationException("Only localization script available.");
                }
            }

            return string.Format("~{0}{1}.js?ver={2}", BundleHelper.CLIENT_SCRIPT_VPATH, filenameHash, GetContentHash(filenameHash));
        }

        public static string GetContent(string uri)
        {
            var content = new StringBuilder();
            foreach (var script in cache[Path.GetFileNameWithoutExtension(uri)])
            {
                content.Append(script.GetData(HttpContext.Current));
            }
            return content.ToString();
        }

        public static string GetContentHash(string uri)
        {
            var version = string.Empty;
            var types = new List<Type>();
            foreach (var s in cache[Path.GetFileNameWithoutExtension(uri)])
            {
                version += s.GetCacheHash();
                types.Add(s.GetType());
            }

            var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
            if (tenant != null && types.All(r => r.BaseType != typeof(ClientScriptLocalization)))
            {
                version = String.Join(string.Empty, new[] { ToString(tenant.TenantId), ToString(tenant.Version), ToString(tenant.LastModified.Ticks), version });
            }

            return GetHash(version);
        }

        private static string ToString(long value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private static string GetHash(string s)
        {
            return HttpServerUtility.UrlTokenEncode(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(s)));
        }
    }
}
