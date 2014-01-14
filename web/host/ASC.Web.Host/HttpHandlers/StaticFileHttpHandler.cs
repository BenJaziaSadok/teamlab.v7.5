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
using System.IO;
using System.Reflection;
using ASC.Web.Host.Config;
using System.Net;

namespace ASC.Web.Host.HttpHandlers
{
    class StaticFileHttpHandler : IHttpHandler
    {
        private static List<string> compressedExt;


        static StaticFileHttpHandler()
        {
            compressedExt = new List<string>(10);
            compressedExt.AddRange(new[] { ".js", ".css", ".less", ".htm", ".htc", ".html", ".xml", ".skin" });
            compressedExt.AddRange(new[] { ".txt", ".doc", ".docx", ".rtf", ".odt", ".xls" });
        }

        #region IHttpHandler Members

        public void ProcessRequest(HttpHandlerContext context)
        {
            try
            {
                if (IsInRestrictDir(context.Connection.ListenerContext.Request.Url.LocalPath))
                {
                    context.Connection.WriteErrorWithExtraHeadersAndKeepAlive(403, null);
                    return;
                }
                var filename = GetFileName(context);
                if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                {
                    context.Connection.WriteErrorWithExtraHeadersAndKeepAlive(404, null);
                    return;
                }

                if (!CheckFileCache(context, filename))
                {
                    using (var f = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var keepAlive = string.CompareOrdinal(context.Connection.ListenerContext.Request.Headers["Connection"], "keep-alive") == 0;
                        var compress = false;
#if (GZIP)
                        if (compressedExt.Contains(Path.GetExtension(filename)))
                        {
                            var acceptEncoding = context.Connection.ListenerContext.Request.Headers["Accept-Encoding"];
                            if (!string.IsNullOrEmpty(acceptEncoding) && acceptEncoding.Contains("gzip"))
                                compress = true;
                        }
#endif
                        context.Connection.WriteEntireResponseFromFile(filename, keepAlive, compress);
                    }
                }
            }
            catch (Exception)
            {
                context.Connection.WriteErrorAndClose(500);
            }
        }

        public bool CanHandle(HttpHandlerContext context)
        {
            var filename = GetFileName(context);
            return !(string.IsNullOrEmpty(filename) || !File.Exists(filename));
        }

        #endregion

        private bool CheckFileCache(HttpHandlerContext context, string filename)
        {
#if (CACHECONTROL)
            var connection = context.Connection;
            var request = connection.ListenerContext.Request;

            var finfo = new FileInfo(filename);
            var nochacecontrol = request.Headers["Cache-Control"];
            if (!string.IsNullOrEmpty(nochacecontrol) && nochacecontrol.IndexOf("no-cache") != -1)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(nochacecontrol) && nochacecontrol.IndexOf("only-if-cached") != -1)
            {
                return true;
            }
            var ifModifiedScince = request.Headers["If-Modified-Since"];

            if (!string.IsNullOrEmpty(ifModifiedScince))
            {
                DateTime cacheModified;
                if (DateTime.TryParseExact(ifModifiedScince, "R", CultureInfo.InvariantCulture, DateTimeStyles.None, out cacheModified) ||
                    DateTime.TryParseExact(ifModifiedScince, "ddd, dd MMM yyyy hh", CultureInfo.InvariantCulture, DateTimeStyles.None, out cacheModified))
                {
                    if (cacheModified.Date >= finfo.LastWriteTimeUtc.Date)
                    {
                        connection.WriteEntireResponseFromString(304, null, "", false);
                        return true;
                    }
                }
            }

            //Split req etag
            var requestEtag = request.Headers["If-None-Match"];
            var etag = string.Format("\"{0}.{1}\"", finfo.LastWriteTimeUtc.ToFileTimeUtc(), finfo.Length);
            if (!string.IsNullOrEmpty(requestEtag))
            {
                string[] reqetags = requestEtag.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var reqetag in reqetags)
                {
                    if (etag.Equals(reqetag))
                    {
                        connection.WriteEntireResponseFromString(304, null, "", false);
                        return true;
                    }
                }
            }
#if (ETAG)
            SendUnknownResponseHeader("Etag", etag);
#endif
            connection.ListenerContext.Response.Headers[HttpResponseHeader.LastModified] = finfo.LastWriteTimeUtc.ToString("R");
            connection.ListenerContext.Response.Headers[HttpResponseHeader.Expires] = DateTime.UtcNow.AddDays(1).ToString("R");
#endif
            return false;
        }

        private string GetFileName(HttpHandlerContext context)
        {
            var virtRoot = context.Server.VirtualPath;
            var physRoot = context.Server.PhysicalPath;
            var virtPath = context.Connection.ListenerContext.Request.Url.LocalPath;
            if (virtPath.StartsWith(virtRoot, StringComparison.OrdinalIgnoreCase))
            {
                virtPath = virtPath.Substring(virtRoot.Length).Replace('/', '\\');
                return Path.Combine(physRoot, virtPath);
            }
            return null;
        }

        private bool IsInRestrictDir(string url)
        {
            var lowerUrl = url.ToLowerInvariant();
            foreach (var restrictDir in RequestConfiguration.RestrictedDirs)
            {
                if (lowerUrl.Contains(restrictDir))
                {
                    return true;
                }
            }
            return false;
        }
    }
}