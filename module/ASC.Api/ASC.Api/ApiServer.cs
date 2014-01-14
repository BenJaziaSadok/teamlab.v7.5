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
using System.Web;
using System.Web.Routing;
using ASC.Api.Batch;
using ASC.Api.Impl;
using ASC.Api.Utils;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System.Text;

namespace ASC.Api
{
    public class ApiServer
    {
        private readonly HttpContextBase _context;
        private readonly ApiBatchHttpHandler _batchHandler;

        public ApiServer()
            : this(HttpContext.Current)
        {
        }

        public ApiServer(HttpContext context)
            : this(new HttpContextWrapper(context))
        { }

        public ApiServer(HttpContextBase context)
        {
            _context = context;
            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            var routeHandler = container.Resolve<ApiBatchRouteHandler>();
            var requestContext = new RequestContext(context, new RouteData(new Route("batch", routeHandler), routeHandler));
            _batchHandler = routeHandler.GetHandler(container, requestContext) as ApiBatchHttpHandler;
            if (_batchHandler==null)
                throw new ArgumentException("Couldn't resolve api");
        }

        public string GetApiResponse(string apiUrl)
        {
            return GetApiResponse(apiUrl, null);
        }

        public string GetApiResponse(string apiUrl, string httpMethod)
        {
            return GetApiResponse(apiUrl, httpMethod, null);
        }

        public string GetApiResponse(string apiUrl, string httpMethod, string body)
        {
            return GetApiResponse(new ApiBatchRequest() { Method = httpMethod, RelativeUrl = apiUrl, Body = body });
        }

        public string GetApiResponse(ApiBatchRequest request)
        {
            var response = CallApiMethod(request);
            return response!=null ? response.Data : null;
        }

        public ApiBatchResponse CallApiMethod(string apiUrl)
        {
            return CallApiMethod(apiUrl, "GET");
        }

        public ApiBatchResponse CallApiMethod(string apiUrl, string httpMethod)
        {
            return CallApiMethod(apiUrl, httpMethod, null);
        }

        public ApiBatchResponse CallApiMethod(string apiUrl, string httpMethod, string body)
        {
            return CallApiMethod(new ApiBatchRequest(){Method = httpMethod,RelativeUrl = apiUrl, Body = body});
        }

        public ApiBatchResponse CallApiMethod(ApiBatchRequest request)
        {
            return CallApiMethod(request, true);
        }

        public ApiBatchResponse CallApiMethod(ApiBatchRequest request, bool encode)
        {
            var response = _batchHandler.ProcessBatchRequest(_context, request);
            if (encode && response!=null && response.Data!=null)
                response.Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(response.Data));
            return response;
        }


        public IEnumerable<ApiBatchResponse> CallApiMethods(IEnumerable<ApiBatchRequest> requests)
        {
            return requests.Select(request =>CallApiMethod(request));
        }
    }
}