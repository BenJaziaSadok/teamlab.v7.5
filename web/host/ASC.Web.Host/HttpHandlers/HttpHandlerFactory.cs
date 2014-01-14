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
using System.IO;
using System.Linq;
using System.Net;
using ASC.Web.Host.Config;
using log4net;

namespace ASC.Web.Host.HttpHandlers
{
    static class HttpHandlerFactory
    {
        private static readonly IDictionary<string, IHttpHandler> handlers;

        private static readonly ILog log = LogManager.GetLogger("ASC.Web.Host");

        public static IHttpHandler AspNetHandler
        {
            get;
            private set;
        }

        public static IHttpHandler DefaultHttpHandler
        {
            get;
            private set;
        }

        public static IHttpHandler DirectoryHttpHandler
        {
            get;
            private set;
        }


        static HttpHandlerFactory()
        {
            DefaultHttpHandler = new StaticFileHttpHandler();
            DirectoryHttpHandler = AspNetHandler = new AspNetHttpHandler();

            var cache = new Dictionary<string, IHttpHandler>();
            handlers = new Dictionary<string, IHttpHandler>();
            foreach (HttpHandlerElement handlerElement in ServerConfiguration.HttpHandlers)
            {
                try
                {
                    if (handlerElement.Extension == "*")
                    {
                        DefaultHttpHandler = GetOrCreateHttpHandler(handlerElement.HandlerType, cache);
                        continue;
                    }
                    if (handlerElement.Extension == "/")
                    {
                        DirectoryHttpHandler = GetOrCreateHttpHandler(handlerElement.HandlerType, cache);
                        continue;
                    }
                    if (!handlers.ContainsKey(handlerElement.Extension))
                    {
                        handlers[handlerElement.Extension] = GetOrCreateHttpHandler(handlerElement.HandlerType, cache);
                    }
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Can not create HttpHandler '{0}'.\r\n{1}", handlerElement.HandlerType, ex);
                }
            }
        }

        private static IHttpHandler GetOrCreateHttpHandler(string handlerType, IDictionary<string, IHttpHandler> cache)
        {
            var handler = cache.ContainsKey(handlerType) ?
                cache[handlerType] :
                (IHttpHandler)Activator.CreateInstance(Type.GetType(handlerType, true));
            cache[handlerType] = handler;
            return handler;
        }

        public static IHttpHandler GetHttpHandler(HttpHandlerContext context)
        {
            return GetHandlers(context).FirstOrDefault(x=>x.CanHandle(context)) ?? DirectoryHttpHandler;
        }

        private static IEnumerable<IHttpHandler> GetHandlers(HttpHandlerContext context)
        {
            var extension = Path.GetExtension(context.Connection.ListenerContext.Request.Url.AbsolutePath);
            if (extension != null && extension.StartsWith("."))
            {
                extension = extension.Substring(1);
                IHttpHandler handler;
                if (handlers.TryGetValue(extension, out handler))
                {
                    yield return handler;
                }
            }
            yield return DefaultHttpHandler;
            yield return AspNetHandler;
            yield return DirectoryHttpHandler;
        }
    }
}