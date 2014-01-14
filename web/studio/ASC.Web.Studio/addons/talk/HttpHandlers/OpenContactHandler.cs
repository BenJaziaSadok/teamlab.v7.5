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
using System.Text;
using System.Web;
using System.Xml.Linq;
using ASC.Web.Studio.Utility;
using ASC.Xmpp.Common;

namespace ASC.Web.Talk.HttpHandlers
{
    public class OpenContactHandler : IHttpHandler
    {
        public bool IsReusable
        {
            // To enable pooling, return true here.
            // This keeps the handler in memory.
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var response = String.Empty;
            if (context.Request.Url != null && !string.IsNullOrEmpty(context.Request.Url.Query))
            {
                var to = String.Empty;
                var from = String.Empty;
                foreach (var query in context.Request.Url.Query.Trim().Trim('?').Split('&'))
                {
                    var el = query.Split('=');
                    switch (el[0].ToLower())
                    {
                        case "from":
                            from = el[1];
                            break;
                        case "to":
                            to = el[1];
                            break;
                    }
                }

                if (!String.IsNullOrEmpty(to) && !String.IsNullOrEmpty(from))
                {
                    try
                    {
                        new JabberServiceClient().SendCommand(from, to, "open", TenantProvider.CurrentTenantID);
                    }
                    catch (Exception ex)
                    {
                        response = ex.Message;
                    }
                }
                else
                {
                    response = "Invalid param";
                }
            }

            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.ContentType = "application/xml";
            context.Response.Charset = Encoding.UTF8.WebName;
            var xml = new XDocument(new XElement("response", response)).ToString();
            context.Response.Write(Encoding.UTF8.GetString(Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(xml))));
        }
    }
}
