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
using System.Net;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using ASC.Core;
using ASC.Web.Core;

namespace ASC.Web.Studio.HttpHandlers
{
    public class TemplatingHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (String.IsNullOrEmpty(context.Request["id"]) || String.IsNullOrEmpty(context.Request["name"])) return;

            var templatePath = context.Request["id"];
            var templateName = context.Request["name"];

            var hashToken = HttpServerUtility.UrlTokenEncode(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(
                String.Join("_", new[] { templatePath, templateName, Thread.CurrentThread.CurrentCulture.Name }))));
            
            if (String.Equals(context.Request.Headers["If-None-Match"], hashToken))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotModified;

                SetCache(context, hashToken);

                return;
            }

            var template = RenderDocument(context, templatePath, templateName);

            context.Response.ContentType = "text/xml";
            context.Response.Write(template.InnerXml);

            SetCache(context, hashToken);

        }

        private void SetCache(HttpContext context, String hashToken)
        {
            context.Response.Cache.SetVaryByCustom("*");
            context.Response.Cache.SetMaxAge(TimeSpan.FromMinutes(10));
            context.Response.Cache.SetAllowResponseInBrowserHistory(true);
            context.Response.Cache.SetETag(hashToken);
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        }


        private static XmlDocument RenderDocument(HttpContext context, string templatePath, string templateName)
        {
            var template = new XmlDocument();
            var path = String.Format("~{0}{1}{2}",
                                     templatePath,
                                     templateName,
                                     string.IsNullOrEmpty(VirtualPathUtility.GetExtension(templateName)) ? ".xsl" : string.Empty);
            try
            {
                template.Load(context.Server.MapPath(path));
            }
            catch (Exception)
            {
                return template;
            }
            if (template.GetElementsByTagName("xsl:stylesheet").Count == 0)
            {
                return template;
            }

            var includes = template.GetElementsByTagName("xsl:include");
            while (includes.Count > 0)
            {
                var currentInclude = includes.Item(0);
                var includeName = currentInclude.Attributes["href"].Value;
                if (!String.IsNullOrEmpty(includeName))
                {
                    var templateInclude = new XmlDocument();
                    try
                    {
                        templateInclude = RenderDocument(context, templatePath, includeName);

                        if (templateInclude.GetElementsByTagName("xsl:stylesheet").Count != 0)
                        {
                            var templateIncludeInner = templateInclude.GetElementsByTagName("xsl:stylesheet")[0];
                            for (var i = 0; i < templateIncludeInner.ChildNodes.Count; i++)
                            {
                                var nodeItem = templateIncludeInner.ChildNodes.Item(i);
                                if (nodeItem.LocalName == "output") continue;
                                var newNode = template.ImportNode(nodeItem, true);
                                currentInclude.ParentNode.InsertBefore(newNode, currentInclude);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        var excValue = template.CreateTextNode("<!--" + e.Message + "-->");
                        currentInclude.ParentNode.InsertBefore(excValue, currentInclude);
                    }
                }

                currentInclude.ParentNode.RemoveChild(currentInclude);
            }

            var aliases = new Dictionary<String, String>();
            var registerAliases = template.GetElementsByTagName("register");
            while (registerAliases.Count > 0)
            {
                var registerAlias = registerAliases.Item(0);
                if (!String.IsNullOrEmpty(registerAlias.Attributes["alias"].Value)
                    && !String.IsNullOrEmpty(registerAlias.Attributes["type"].Value))
                {
                    aliases.Add(registerAlias.Attributes["alias"].Value, registerAlias.Attributes["type"].Value);
                }
                registerAlias.ParentNode.RemoveChild(registerAlias);
            }

            var currentResources = template.GetElementsByTagName("resource");
            while (currentResources.Count > 0)
            {
                var currentResource = currentResources.Item(0);
                if (!String.IsNullOrEmpty(currentResource.Attributes["name"].Value))
                {
                    var FullName = currentResource.Attributes["name"].Value.Split('.');
                    if (FullName.Length == 2 && aliases.ContainsKey(FullName[0]))
                    {
                        var ResourceValue = template.CreateTextNode(GetModuleResource(aliases[FullName[0]], FullName[1]));
                        currentResource.ParentNode.InsertBefore(ResourceValue, currentResource);
                    }
                }
                currentResource.ParentNode.RemoveChild(currentResource);
            }
            return template;
        }

        private static String GetModuleResource(String resourceClassTypeName, String resourseKey)
        {
            if (string.IsNullOrEmpty(resourseKey)) return string.Empty;
            try
            {
                var type = Type.GetType(resourceClassTypeName);

                var resManager =
                    (ResourceManager) type.InvokeMember(
                        "resourceMan",
                        BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public, null, type, null);

                //custom
                if (!SecurityContext.IsAuthenticated)
                {
                    SecurityContext.AuthenticateMe(CookiesManager.GetCookies(CookiesType.AuthKey));
                }
                var u = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
                var culture = !string.IsNullOrEmpty(u.CultureName)
                                  ? CultureInfo.GetCultureInfo(u.CultureName)
                                  : CoreContext.TenantManager.GetCurrentTenant().GetCulture();
                return resManager.GetString(resourseKey, culture);
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }
    }
}