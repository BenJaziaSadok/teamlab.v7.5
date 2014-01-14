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
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using ASC.Core;
using ASC.Xmpp.Common;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Server;

namespace ASC.Xmpp.Host.Responders
{
    public abstract class BaseAscResponder : IHttpResponder
    {
        public abstract string Path { get; }

        public XDocument Process(HttpListenerRequest request)
        {
            HttpContext.Current = new HttpContext(new HttpFakeWorkerRequest(request));
            //Process authorization
            var cookie = request.Cookies["asc_auth_key"];//TODO:??? move it somewhere
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {

                if (SecurityContext.AuthenticateMe(cookie.Value))
                {
                    if (SecurityContext.IsAuthenticated)
                    {
                        //Process
                        if (JabberService.CurrentXmppServer != null)
                        {
                            var username =
                                CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).UserName.ToLowerInvariant();
                            Jid jid = GetJid(username);
                            string input;
                            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                            {
                                input = reader.ReadToEnd();
                            }
                            var values = new NameValueCollection(request.QueryString);
                            if (request.ContentType == "application/x-www-form-urlencoded")
                            {
                                //Parse urle encoded form
                                var queryString = HttpUtility.ParseQueryString(input);
                                foreach (var key in queryString.AllKeys)
                                {
                                    values.Add(key, queryString[key]);
                                }
                            }
                            else if (request.ContentType == "application/json")
                            {
                                var serializer = new JavaScriptSerializer();
                                var dict = serializer.Deserialize<Dictionary<string, string>>(input);
                                foreach (var keypair in dict)
                                {
                                    values.Add(keypair.Key, keypair.Value);
                                }
                            }

                            return new XDocument(new XElement("responce", ProcessRequest(JabberService.CurrentXmppServer, jid, values)));
                        }
                    }
                }
            }
            throw new UnauthorizedAccessException("unathorized");
        }

        protected Jid GetJid(string username)
        {
            return new Jid(username.ToLowerInvariant(), CoreContext.TenantManager.GetCurrentTenant().TenantDomain, null);
        }

        protected abstract XElement ProcessRequest(XmppServer server, Jid username, NameValueCollection values);
    }
}
