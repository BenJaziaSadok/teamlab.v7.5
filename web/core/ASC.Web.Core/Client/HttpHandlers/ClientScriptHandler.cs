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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace ASC.Web.Core.Client.HttpHandlers
{
    public class ClientScriptHandler : IHttpHandler
    {
        public List<ClientScript> ClientScriptHandlers { get; set; }

        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            var aggregateHash = ClientScriptHandlers.Select(r => r.GetCacheHash()).Aggregate((a, b) => a + b);
            var aggregateHashToken = ClientScript.GetHashToken(aggregateHash);

            if (!string.IsNullOrEmpty(aggregateHash) && string.Equals(context.Request.Headers["If-None-Match"], aggregateHashToken) &&
                ClientSettings.IsResourceCachingEnabled)
            {
                context.Response.StatusCode = (int) HttpStatusCode.NotModified;
            }
            else
            {
                var builder = new StringBuilder();
                builder.AppendLine(
@"/*
    Copyright (c) Ascensio System SIA " + DateTime.UtcNow.Year + @". All rights reserved.
    http://www.teamlab.com
*/");
                builder.Append(GetData(context));
                context.Response.Write(builder.ToString());
            }

            if (ClientSettings.IsResourceCachingEnabled)
            {
                context.Response.Cache.SetVaryByCustom("*");
                context.Response.Cache.SetMaxAge(TimeSpan.FromDays(365));
                context.Response.Cache.SetAllowResponseInBrowserHistory(true);
                context.Response.Cache.SetETag(aggregateHashToken);
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            }
            context.Response.Charset = Encoding.UTF8.WebName;
            context.Response.ContentType = new ContentType("application/x-javascript") { CharSet = Encoding.UTF8.WebName }.ToString();
        }

        public string GetData(HttpContext context)
        {
            var builder = new StringBuilder();

            foreach (var clientScriptHandler in ClientScriptHandlers)
            {
                builder.Append(clientScriptHandler.GetData(context));
            }

            return builder.ToString();
        }

        public bool IsReusable
        {
            get { return true; }
        }

        #endregion
    }
}