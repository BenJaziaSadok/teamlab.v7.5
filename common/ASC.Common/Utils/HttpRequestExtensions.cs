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

using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;

namespace System.Web
{
    public static class HttpRequestExtensions
    {
        public static readonly string UrlRewriterHeader = "X-REWRITER-URL";


        public static Uri GetUrlRewriter(this HttpRequest request)
        {
            return request != null ? GetUrlRewriter(request.Headers, request.Url) : null;
        }

        public static Uri GetUrlRewriter(NameValueCollection headers, Uri requestUri)
        {
            if (headers == null || requestUri == null)
            {
                return requestUri;
            }

            var rewriterUri = ParseRewriterUrl(headers[UrlRewriterHeader]);
            if (rewriterUri != null)
            {
                var result = new UriBuilder(requestUri);
                result.Scheme = rewriterUri.Scheme;
                result.Host = rewriterUri.Host;
                result.Port = rewriterUri.Port;
                return result.Uri;
            }
            else
            {
                return requestUri;
            }
        }

        public static Uri PushRewritenUri(this HttpContext context)
        {
            return context != null ? PushRewritenUri(context, GetUrlRewriter(context.Request)) : null;
        }

        public static Uri PushRewritenUri(this HttpContext context, Uri rewrittenUri)
        {
            Uri oldUri = null;
            if (context != null)
            {
                var request = context.Request;

                if (request.Url != rewrittenUri)
                {
                    var requestUri = request.Url;
                    try
                    {
                        //Push it
                        request.ServerVariables.Set("HTTPS", rewrittenUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ? "on" : "off");
                        request.ServerVariables.Set("SERVER_NAME", rewrittenUri.Host);
                        request.ServerVariables.Set("SERVER_PORT",
                                                    rewrittenUri.Port.ToString(CultureInfo.InvariantCulture));

                        if (rewrittenUri.IsDefaultPort)
                        {
                            request.ServerVariables.Set("HTTP_HOST",
                                                    rewrittenUri.Host);
                        }
                        else
                        {
                            request.ServerVariables.Set("HTTP_HOST",
                                                    rewrittenUri.Host + ":" + requestUri.Port);
                        }
                        //Hack:
                        typeof(HttpRequest).InvokeMember("_url",
                                                          BindingFlags.NonPublic | BindingFlags.SetField |
                                                          BindingFlags.Instance,
                                                          null, HttpContext.Current.Request,
                                                          new object[] { null });
                        oldUri = requestUri;
                        context.Items["oldUri"] = oldUri;

                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return oldUri;
        }

        public static Uri PopRewritenUri(this HttpContext context)
        {
            if (context != null && context.Items["oldUri"] != null)
            {
                var rewriteTo = context.Items["oldUri"] as Uri;
                if (rewriteTo != null)
                {
                    return PushRewritenUri(context, rewriteTo);
                }
            }
            return null;
        }


        private static Uri ParseRewriterUrl(string s)
        {
            Uri result = null;
            var cmp = StringComparison.OrdinalIgnoreCase;

            if (string.IsNullOrEmpty(s))
            {
                return result;
            }
            if (0 < s.Length && (s.StartsWith("0", cmp)))
            {
                s = Uri.UriSchemeHttp + s.Substring(1);
            }
            else if (3 < s.Length && s.StartsWith("OFF", cmp))
            {
                s = Uri.UriSchemeHttp + s.Substring(3);
            }
            else if (0 < s.Length && (s.StartsWith("1", cmp)))
            {
                s = Uri.UriSchemeHttps + s.Substring(1);
            }
            else if (2 < s.Length && s.StartsWith("ON", cmp))
            {
                s = Uri.UriSchemeHttps + s.Substring(2);
            }
            else if (s.StartsWith(Uri.UriSchemeHttp + "%3A%2F%2F", cmp) || s.StartsWith(Uri.UriSchemeHttps + "%3A%2F%2F", cmp))
            {
                s = HttpUtility.UrlDecode(s);
            }

            Uri.TryCreate(s, UriKind.Absolute, out result);
            return result;
        }
    }
}
