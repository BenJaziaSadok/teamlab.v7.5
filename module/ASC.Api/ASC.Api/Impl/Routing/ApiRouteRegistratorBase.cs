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
using ASC.Api.Logging;
using Microsoft.Practices.Unity;

namespace ASC.Api.Impl.Routing
{
    internal abstract class ApiRouteRegistratorBase : IApiRouteRegistrator
    {
        [Dependency]
        public IUnityContainer Container { get; set; }

        [Dependency]
        public IApiConfiguration Config { get; set; }

        [Dependency]
        public ILog Log { get; set; }

        public void RegisterRoutes(RouteCollection routes)
        {
            var entryPoints = Container.Resolve<IEnumerable<IApiMethodCall>>();
            var extensions = new List<string>();
            foreach (IApiResponder apiSerializer in Container.ResolveAll<IApiResponder>())
            {
                extensions.AddRange(apiSerializer.GetSupportedExtensions().Select(x => x.StartsWith(".") ? x : "." + x));
            }
            RegisterEntryPoints(routes, entryPoints, extensions);
        }

        protected abstract void RegisterEntryPoints(RouteCollection routes, IEnumerable<IApiMethodCall> entryPoints, List<string> extensions);
    }
}