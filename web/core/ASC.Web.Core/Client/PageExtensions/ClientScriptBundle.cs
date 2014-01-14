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
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Routing;
using ASC.Collections;
using ASC.Web.Core.Client.HttpHandlers;

namespace ASC.Web.Core.Client.PageExtensions
{
    public class ClientScriptBundle
    {
        private static readonly IDictionary<string, ClientScriptHandler> Hash =
            new SynchronizedDictionary<string, ClientScriptHandler>();


        public static ClientScriptHandler GetHttpHandler(string path)
        {
            ClientScriptHandler handler;
            if (Hash.TryGetValue(path, out handler))
            {
                return handler;
            }
            throw new NotSupportedException();
        }

        static ClientScriptBundle()
        {
            RouteTable.Routes.Add(new Route("clientscriptbundle/{version:\\d+}/{path}" + ClientSettings.ClientScriptExtension, new HttpBundleRouteHandler(Hash)));
        }

        public static string ResolveHandlerPath(ICollection<Type> types)
        {
            var tenant = ASC.Core.CoreContext.TenantManager.GetCurrentTenant();
            var version = ""; 
            var resultPath = "";
            var listHandlers  = new List<ClientScript>();

            foreach (var type in types)
            {
                if (!typeof(ClientScript).IsAssignableFrom(type))
                {
                    throw new ArgumentException(string.Format("{0} is not assignable to ClientScriptHandler", type));
                }
                var instance = (ClientScript)Activator.CreateInstance(type);
                version += instance.GetCacheHash();
                resultPath = resultPath + type.FullName.ToLowerInvariant().Replace('.', '_');

                listHandlers.Add(instance);
            }

            resultPath = HttpServerUtility.UrlTokenEncode(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(resultPath)));

            if (!Hash.ContainsKey(resultPath))
            {
                Hash[resultPath] = new ClientScriptHandler {ClientScriptHandlers = listHandlers};
            }

            if (tenant != null && types.All(r => r.BaseType != typeof(ClientScriptLocalization)))
                version = String.Join("_",
                                      new[]
                                              {
                                                  tenant.TenantId.ToString(CultureInfo.InvariantCulture),
                                                  tenant.Version.ToString(CultureInfo.InvariantCulture),
                                                  tenant.LastModified.Ticks.ToString(CultureInfo.InvariantCulture),
                                                  version
                                              });

            var versionHash = HttpServerUtility.UrlTokenEncode(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(version)));

            return new Uri(VirtualPathUtility.ToAbsolute("~/clientscriptbundle/" + versionHash + "/" + resultPath + ClientSettings.ClientScriptExtension),
                    UriKind.Relative).ToString();
        }

        #region Nested type: HttpRouteHandler

        public class HttpBundleRouteHandler : IRouteHandler
        {
            private readonly IDictionary<string, ClientScriptHandler> hash;

            public HttpBundleRouteHandler(IDictionary<string, ClientScriptHandler> hash)
            {
                this.hash = hash;
            }

            #region IRouteHandler Members

            public IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                //get name
                var path = (string) requestContext.RouteData.Values["path"];
                ClientScriptHandler handler;
                if (hash.TryGetValue(path, out handler))
                {
                    return handler;
                }
                throw new NotSupportedException();
            }

            #endregion
        }

        #endregion
    }
}