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

using System.Linq;
using System.Web;
using System.Web.Routing;
using ASC.Api.Impl;
using ASC.Api.Interfaces;
using Microsoft.Practices.Unity;

namespace ASC.Api.Batch
{
    public class ApiBatchRouteHandler : ApiRouteHandler
    {
        public override IHttpHandler GetHandler(IUnityContainer container, RequestContext requestContext)
        {
            return container.Resolve<ApiBatchHttpHandler>(new DependencyOverride(typeof(RouteData), requestContext.RouteData));
        }
    }
}