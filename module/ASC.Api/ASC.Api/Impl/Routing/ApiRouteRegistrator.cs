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

using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using ASC.Api.Interfaces;
using Microsoft.Practices.Unity;

namespace ASC.Api.Impl.Routing
{
    class ApiRouteRegistrator : ApiRouteRegistratorBase
    {
        protected override void RegisterEntryPoints(RouteCollection routes,IEnumerable<IApiMethodCall> entryPoints, List<string> extensions)
        {
            foreach (IApiMethodCall apiMethodCall in entryPoints.OrderBy(x => x.RoutingUrl.IndexOf('{')).ThenBy(x => x.RoutingUrl.LastIndexOf('}')))
            {
                foreach (string extension in extensions)
                {
                    routes.Add(GetRoute(Container.Resolve<IApiRouteHandler>(), apiMethodCall, extension));
                }
                routes.Add(GetRoute(Container.Resolve<IApiRouteHandler>(), apiMethodCall));
            }
        }

        public Route GetRoute(IApiRouteHandler routeHandler, IApiMethodCall method)
        {
            return GetRoute(routeHandler, method, string.Empty);
        }

        public Route GetRoute(IApiRouteHandler routeHandler, IApiMethodCall method, string extension)
        {
            var dataTokens = new RouteValueDictionary
                                 {
                                     {DataTokenConstants.RequiresAuthorization,method.RequiresAuthorization}
                                 };
            return new Route(method.FullPath + extension, null, method.Constraints, dataTokens, routeHandler);
        }
    }
}