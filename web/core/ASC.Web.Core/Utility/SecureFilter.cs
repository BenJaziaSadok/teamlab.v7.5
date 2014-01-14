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
using System.Web;
using System.Web.Caching;
using System.Linq;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Tenants;

namespace ASC.Web.Core.Utility
{
    public class SecureFilter
    {
        private List<string> securedPages = new List<string>();

        private SecureFilter(string filterString)
        {
            var pages = filterString.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (pages.Length > 0)
            {
                securedPages.AddRange(pages.Select(x => x.Trim().TrimStart('/')));
            }
        }

        public void ProcessRequest(Uri requestUri, string sslPort, string httpPort)
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.Headers["X-Forwarded-For"]))
            {
                return;
            }

            if (!securedPages.Any()) return;

            var appPath = HttpContext.Current.Request.ApplicationPath;
            var pageUrl = requestUri.AbsolutePath.Substring(appPath.Length).TrimStart('/');
            bool isCurrentSecured = requestUri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase);

            //if httpS is not enabled for whole tennant - then go standart way
            var filtered = securedPages.Where(pageUrl.WildcardMatch).FirstOrDefault();
            if (isCurrentSecured)
            {
                //All pages that is not in filter should go to http
                if (string.IsNullOrEmpty(filtered))
                {
                    ReplaceSchemeAndRedirect(Uri.UriSchemeHttp, requestUri);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(filtered))
                {
                    //All pages that is in filter should go to https
                    ReplaceSchemeAndRedirect(Uri.UriSchemeHttps, requestUri);
                }
            }
        }

        private void ReplaceSchemeAndRedirect(string scheme, Uri path)
        {
            //Cut the scheme
            var redirUri = scheme + path.AbsoluteUri.Substring(path.Scheme.Length);
            HttpContext.Current.Response.Redirect(redirUri, true);
        }

        public static SecureFilter GetInstance(string filterString)
        {
            var secureFilter = HttpContext.Current.Cache.Get(filterString) as SecureFilter;
            if (secureFilter == null)
            {
                secureFilter = new SecureFilter(filterString);
                HttpContext.Current.Cache.Add(filterString, secureFilter, null, Cache.NoAbsoluteExpiration,
                                              Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
            }
            return secureFilter;
        }
    }
}