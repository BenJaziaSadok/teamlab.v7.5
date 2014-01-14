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
using System.Linq;
using System.Web.Routing;

namespace ASC.Api.Impl.Constraints
{
    public class ApiHttpMethodConstraint : HttpMethodConstraint
    {
        public ApiHttpMethodConstraint(params string[] allowedMethods):base(allowedMethods)
        {
            
        }

        protected override bool Match(System.Web.HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var baseMatch = base.Match(httpContext, route, parameterName, values, routeDirection);
            if (!baseMatch && routeDirection==RouteDirection.IncomingRequest)
            {
                baseMatch = AllowedMethods.Any(method => string.Equals(method, httpContext.Request.RequestType, StringComparison.OrdinalIgnoreCase));
            }
            return baseMatch;
        }
    }
}