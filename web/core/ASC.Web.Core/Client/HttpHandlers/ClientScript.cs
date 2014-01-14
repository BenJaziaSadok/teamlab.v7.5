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

using ASC.Web.Core.Client.Templates;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using TMResourceData;

namespace ASC.Web.Core.Client.HttpHandlers
{
    public abstract class ClientScript
    {
        protected virtual string BaseNamespace
        {
            get { return "ASC.Resources"; }
        }


        protected abstract IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context);


        public string GetData(HttpContext context)
        {
            var namespaces = BaseNamespace.Split('.');
            var builder = new StringBuilder();
            var content = string.Empty;

            for (var index = 1; index <= namespaces.Length; index++)
            {
                var ns = string.Join(".", namespaces, 0, index);
                builder.AppendFormat("if (typeof({0})==='undefined'){{{0} = {{}};}} ", ns);
            }

            var store = GetClientVariables(context);
            if (store != null)
            {
                foreach (var clientObject in store)
                {
                    var resourceSet = clientObject.Value as ClinetResourceSet;
                    if (resourceSet != null)
                    {
                        builder.AppendFormat("{0}.{1}={2};", BaseNamespace, clientObject.Key, JsonConvert.SerializeObject(resourceSet.GetResources()));
                        continue;
                    }

                    var templateSet = clientObject.Value as ClientTemplateSet;
                    if (templateSet != null)
                    {
                        builder.AppendFormat("{0}{1}", Environment.NewLine, templateSet.GetClientTemplates());
                        continue;
                    }

                    builder.AppendFormat("{0}.{1}={2};", BaseNamespace, clientObject.Key, JsonConvert.SerializeObject(clientObject.Value));
                }

                content = builder.ToString();
            }
            return content;
        }

        protected internal virtual string GetCacheHash()
        {
            return string.Empty;
        }


        protected KeyValuePair<string, object> RegisterObject(string key, object value)
        {
            return new KeyValuePair<string, object>(key, value);
        }

        protected KeyValuePair<string, object> RegisterResourceSet(string key, ResourceManager resourceManager)
        {
            return new KeyValuePair<string, object>(key, new ClinetResourceSet(resourceManager, () => Thread.CurrentThread.CurrentUICulture));
        }

        protected KeyValuePair<string, object> RegisterClientTemplatesPath(string virtualPathToControl, HttpContext context)
        {
            var page = new Page();
            page.Controls.Add(page.LoadControl(virtualPathToControl));

            var output = new StringWriter();
            context.Server.Execute(page, output, false);

            var doc = new HtmlDocument();
            doc.LoadHtml(output.GetStringBuilder().ToString());

            var nodes = doc.DocumentNode.SelectNodes("//script[@type='text/x-jquery-tmpl']");
            var templates = nodes.ToDictionary(x => x.Attributes["id"].Value, y => y.InnerHtml);
            return new KeyValuePair<string, object>(Guid.NewGuid().ToString(), new ClientTemplateSet(() => templates));
        }


        class ClientTemplateSet
        {
            private static readonly JqTemplateCompiler compiler = new JqTemplateCompiler();
            private readonly Func<Dictionary<string, string>> getTemplates;


            public ClientTemplateSet(Func<Dictionary<string, string>> clientTemplates)
            {
                getTemplates = clientTemplates;
            }

            public string GetClientTemplates()
            {
                var result = new StringBuilder();
                lock (compiler)
                {
                    foreach (var template in getTemplates())
                    {
                        // only for jqTmpl for now
                        result.AppendFormat("jQuery.template('{0}', {1});{2}", template.Key, compiler.GetCompiledCode(template.Value), Environment.NewLine);
                    }
                }
                return result.ToString();
            }
        }

        class ClinetResourceSet
        {
            private readonly Func<CultureInfo> getCulture;
            private readonly ResourceManager manager;

            public ClinetResourceSet(ResourceManager manager, Func<CultureInfo> culture)
            {
                this.manager = manager;
                getCulture = culture;
            }

            public IDictionary<string, string> GetResources()
            {
                var culture = getCulture();
                var baseFromDbSet = manager.GetResourceSet(CultureInfo.InvariantCulture, true, true);

                var dbManager = manager as DBResourceManager;
                var baseNeutral = baseFromDbSet;

                if (dbManager != null)
                {
                    baseNeutral = dbManager.GetBaseNeutralResourceSet();
                }
                var set = manager.GetResourceSet(culture, true, true);
                var result = new Dictionary<string, string>();
                foreach (DictionaryEntry entry in baseNeutral)
                {
                    var value = set.GetString((string)entry.Key) ?? baseFromDbSet.GetString((string)entry.Key) ?? baseNeutral.GetString((string)entry.Key) ?? string.Empty;
                    result.Add(entry.Key.ToString(), value);
                }

                return result;
            }
        }
    }
}