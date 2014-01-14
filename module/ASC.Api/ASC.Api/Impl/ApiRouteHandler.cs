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

#region usings

using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using ASC.Api.Interfaces;
using ASC.Api.Logging;
using ASC.Api.Utils;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

#endregion

namespace ASC.Api.Impl
{
    public class ApiRouteHandler : IApiRouteHandler
    {

        #region IApiRouteHandler Members


        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            var authorizations = container.ResolveAll<IApiAuthorization>().ToList();
            var log = container.Resolve<ILog>();

            //Authorize request first
            log.Debug("Authorizing {0}",requestContext.HttpContext.Request.Url);
            
            
            if (requestContext.RouteData.DataTokens.ContainsKey(DataTokenConstants.RequiresAuthorization) 
                && !(bool)requestContext.RouteData.DataTokens[DataTokenConstants.RequiresAuthorization])
            {
                //Authorization is not required for method
                log.Debug("Authorization is not required");
                return GetHandler(container, requestContext);
            }
            foreach (var apiAuthorization in authorizations)
            {
                log.Debug("Authorizing with:{0}",apiAuthorization.GetType().ToString());
                if (apiAuthorization.Authorize(requestContext.HttpContext))
                {

                    return GetHandler(container,requestContext);
                }
            }
            if (authorizations.Any(apiAuthorization => apiAuthorization.OnAuthorizationFailed(requestContext.HttpContext)))
            {
                log.Debug("Unauthorized");
                return new ErrorHttpHandler((int)HttpStatusCode.Unauthorized, HttpStatusCode.Unauthorized.ToString());
            }
            log.Debug("Forbidden");
            return new ErrorHttpHandler((int)HttpStatusCode.Unauthorized, HttpStatusCode.Unauthorized.ToString());
        }

        public virtual IHttpHandler GetHandler(IUnityContainer container,RequestContext requestContext)
        {
            return container.Resolve<IApiHttpHandler>(new DependencyOverride(typeof(RouteData), requestContext.RouteData));
        }

        #endregion
    }

    class ApiAsyncRouteHandler : ApiRouteHandler
    {

        public override IHttpHandler GetHandler(IUnityContainer container, RequestContext requestContext)
        {
            throw new NotImplementedException("This handler is not yet implemented");
            
        }
    }
}