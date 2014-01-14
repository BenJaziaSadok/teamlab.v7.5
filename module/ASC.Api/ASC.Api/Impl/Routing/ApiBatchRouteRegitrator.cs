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
using ASC.Api.Batch;
using ASC.Api.Impl.Constraints;
using ASC.Api.Interfaces;
using Microsoft.Practices.Unity;

namespace ASC.Api.Impl.Routing
{
    public class ApiBatchRouteRegitrator : IApiRouteRegistrator
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        [Dependency]
        public IApiConfiguration Config { get; set; }

        public void RegisterRoutes(RouteCollection routes)
        {
            var constrasints = new RouteValueDictionary {{"method", new ApiHttpMethodConstraint("POST", "GET")}};
            var basePath = Config.GetBasePath();
            foreach (var extension in Container.ResolveAll<IApiResponder>().SelectMany(apiSerializer => apiSerializer.GetSupportedExtensions().Select(x => x.StartsWith(".") ? x : "." + x)))
            {
                routes.Add(new Route(basePath + "batch" + extension, null, constrasints, null, new ApiBatchRouteHandler()));
            }
            routes.Add(new Route(basePath + "batch", null, constrasints, null, new ApiBatchRouteHandler()));
        }
    }
}