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
using System.Linq;
using System.Web;

namespace ASC.Web.Core.Utility
{
    public static class UrlSwitcher
    {
        public static string SelectCurrentUriScheme(string uri)
        {
            return HttpContext.Current != null ? SelectUriScheme(uri, HttpContext.Current.Request.GetUrlRewriter().Scheme) : uri;
        }

        public static string SelectUriScheme(string uri, string scheme)
        {
            return Uri.IsWellFormedUriString(uri,UriKind.Absolute) ? SelectUriScheme(new Uri(uri, UriKind.Absolute),scheme).ToString() : uri;
        }

        public static Uri SelectCurrentUriScheme(Uri uri)
        {
            if (HttpContext.Current!=null)
            {
                return SelectUriScheme(uri, HttpContext.Current.Request.GetUrlRewriter().Scheme);
            }
            return uri;
        }

        public static Uri SelectUriScheme(Uri uri, string scheme)
        {
            if (!string.IsNullOrEmpty(scheme) && !scheme.Equals(uri.Scheme,StringComparison.OrdinalIgnoreCase))
            {
                //Switch
                var builder = new UriBuilder(uri) { Scheme = scheme.ToLowerInvariant(), Port = scheme.Equals("https",StringComparison.OrdinalIgnoreCase)?443:80};//Set proper port!
                return builder.Uri;
            }
            return uri;
        }

        public static Uri ToCurrentScheme(this Uri uri)
        {
            return SelectCurrentUriScheme(uri);
        }

        public static Uri ToScheme(this Uri uri, string scheme)
        {
            return SelectUriScheme(uri,scheme);
        }
    }
}