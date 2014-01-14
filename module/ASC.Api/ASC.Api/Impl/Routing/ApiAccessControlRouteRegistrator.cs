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

using System.Web;
using System.Web.Routing;
using ASC.Api.Impl.Constraints;
using ASC.Api.Interfaces;
using Microsoft.Practices.Unity;

namespace ASC.Api.Impl.Routing
{
    public class ApiAccessControlRouteRegistrator : IApiRouteRegistrator
    {
        [Dependency]
        public IApiConfiguration Config { get; set; }

        public void RegisterRoutes(RouteCollection routes)
        {
            //Register 1 route
            var basePath = Config.GetBasePath();
            var constrasints = new RouteValueDictionary {{"method", new ApiHttpMethodConstraint("OPTIONS")}};
            routes.Add(new Route(basePath + "{*path}", null, constrasints, new ApiAccessRouteHandler()));
        }

        public class ApiAccessRouteHandler : IRouteHandler
        {
            public IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                return new ApiAccessHttpHandler();
            }

            public class ApiAccessHttpHandler : IHttpHandler
            {
                public void ProcessRequest(HttpContext context)
                {
                    //Set access headers
                    //Access-Control-Allow-Origin: http://foo.example  
                    //Access-Control-Allow-Methods: POST, GET, OPTIONS  
                    //Access-Control-Allow-Headers: X-PINGOTHER  
                    //Access-Control-Max-Age: 1728000  
                    context.Response.Headers["Access-Control-Allow-Origin"] = "*"; //Allow all
                    context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE";
                    context.Response.Headers["Access-Control-Allow-Headers"] = "origin, authorization, accept";
                    context.Response.Headers["Access-Control-Max-Age"] = "1728000";

                }

                public bool IsReusable
                {
                    get { return false; }
                }
            }
        }
    }
}