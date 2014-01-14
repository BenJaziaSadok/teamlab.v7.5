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
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Runtime.Remoting;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Routing;

namespace ASC.Web.Core.Client.Bundling
{
    class ClientScriptVirtualPathProvider : VirtualPathProvider, IRouteHandler
    {
        public override bool FileExists(string virtualPath)
        {
            return virtualPath.Contains(BundleHelper.CLIENT_SCRIPT_VPATH) ? true : GetPrevius().FileExists(virtualPath);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return virtualPath.Contains(BundleHelper.CLIENT_SCRIPT_VPATH) ? new ClientScriptFile(virtualPath) : GetPrevius().GetFile(virtualPath);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return GetPrevius().GetCacheDependency(virtualPath, virtualPathDependencies.Cast<string>().Where(p => !p.Contains(BundleHelper.CLIENT_SCRIPT_VPATH)), utcStart);
        }


        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            return GetPrevius().GetFileHash(virtualPath, virtualPathDependencies);
        }

        public override string CombineVirtualPaths(string basePath, string relativePath)
        {
            return GetPrevius().CombineVirtualPaths(basePath, relativePath);
        }

        public override ObjRef CreateObjRef(Type requestedType)
        {
            return GetPrevius().CreateObjRef(requestedType);
        }

        public override bool DirectoryExists(string virtualDir)
        {
            return GetPrevius().DirectoryExists(virtualDir);
        }

        public override bool Equals(object obj)
        {
            return GetPrevius().Equals(obj);
        }

        public override string GetCacheKey(string virtualPath)
        {
            return GetPrevius().GetCacheKey(virtualPath);
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            return GetPrevius().GetDirectory(virtualDir);
        }

        public override int GetHashCode()
        {
            return GetPrevius().GetHashCode();
        }

        public override object InitializeLifetimeService()
        {
            return GetPrevius().InitializeLifetimeService();
        }

        public override string ToString()
        {
            return GetPrevius().ToString();
        }


        private VirtualPathProvider GetPrevius()
        {
            return Previous == null && HostingEnvironment.VirtualPathProvider != this ? HostingEnvironment.VirtualPathProvider : Previous;
        }


        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ClientScriptHandler();
        }


        class ClientScriptHandler : IHttpHandler
        {
            public bool IsReusable
            {
                get { return true; }
            }

            public void ProcessRequest(HttpContext context)
            {
                var version = ClientScriptReference.GetContentHash(context.Request.Url.AbsolutePath);
                if (string.Equals(context.Request.Headers["If-None-Match"], version))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotModified;
                }
                else
                {
                    context.Response.Write(CopyrigthTransform.CopyrigthText);
                    context.Response.Write(ClientScriptReference.GetContent(context.Request.Url.AbsolutePath));
                    context.Response.Charset = Encoding.UTF8.WebName;
                    context.Response.ContentType = new ContentType("application/x-javascript") { CharSet = Encoding.UTF8.WebName }.ToString();

                    // cache
                    context.Response.Cache.SetVaryByCustom("*");
                    context.Response.Cache.SetMaxAge(TimeSpan.FromDays(365));
                    context.Response.Cache.SetAllowResponseInBrowserHistory(true);
                    context.Response.Cache.SetETag(version);
                    context.Response.Cache.SetCacheability(HttpCacheability.Public);
                    context.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                }
            }
        }


        public class ClientScriptFile : VirtualFile
        {
            public override bool IsDirectory
            {
                get { return false; }
            }


            public ClientScriptFile(string virtualPath)
                : base(virtualPath)
            {
            }

            public override Stream Open()
            {
                var stream = new MemoryStream();
                var buffer = Encoding.UTF8.GetBytes(ClientScriptReference.GetContent(VirtualPath));
                stream.Write(buffer, 0, buffer.Length);
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }
    }
}