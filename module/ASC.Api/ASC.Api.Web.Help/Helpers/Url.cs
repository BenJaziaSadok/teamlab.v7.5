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

using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using ASC.Api.Web.Help.DocumentGenerator;

namespace ASC.Api.Web.Help.Helpers
{
    public static class Url
    {
        public static string DocUrl(this UrlHelper url, MsDocEntryPoint section,
                                    MsDocEntryPointMethod method)
        {
            return url.DocUrl(section.Name, method.HttpMethod, method.Path);
        }


        public static string DocUrl(this UrlHelper url, MsDocEntryPoint section)
        {
            return url.DocUrl(section.Name);
        }

        public static string DocUrl(this UrlHelper url)
        {
            return DocUrl(url, (string) null);
        }

        public static string DocUrl(this UrlHelper url, string section)
        {
            return DocUrl(url, section, null);
        }

        public static string DocUrl(this UrlHelper url, string section, string method)
        {
            return DocUrl(url, section, method, null);
        }

        public static string DocUrl(this UrlHelper url, string section, string method, string apiUrl)
        {
            return url.RouteUrl("Docs",
                                GetRouteValues(section, method, apiUrl));
        }

        public static object GetRouteValues(string section, string method, string apiUrl)
        {
            return new
                       {
                           section =
                               string.IsNullOrEmpty(section)
                                   ? UrlParameter.Optional
                                   : (object) section.ToLowerInvariant(),
                           type =
                               string.IsNullOrEmpty(method)
                                   ? UrlParameter.Optional
                                   : (object) method.ToLowerInvariant(),
                           url =
                               string.IsNullOrEmpty(apiUrl)
                                   ? UrlParameter.Optional
                                   : (object) apiUrl.ToLowerInvariant()
                       };
        }

        public static string GetDocUrl(string section, string method, string apiUrl, RequestContext context)
        {
            return UrlHelper.GenerateUrl("Docs", "Index", "Documentation", new RouteValueDictionary(GetRouteValues(section, method, apiUrl)), RouteTable.Routes, context,false);
        }

    }
}