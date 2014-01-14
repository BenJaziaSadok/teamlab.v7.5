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
using System.Net.Mime;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using System.Web.Script.Serialization;
using ASC.Api.Collections;
using ASC.Api.Impl;
using ASC.Api.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ASC.Api.Batch
{
    public class ApiBatchHttpHandler : ApiHttpHandlerBase
    {
        public ApiBatchHttpHandler(RouteData routeData)
            : base(routeData)
        {
        }

        protected override void DoProcess(HttpContextBase context)
        {
            //Read body
            var batch = context.Request["batch"];
            IEnumerable<ApiBatchRequest> requests = null;
            if (!string.IsNullOrEmpty(batch))
            {
                requests = JsonConvert.DeserializeObject<IEnumerable<ApiBatchRequest>>(batch,new JsonSerializerSettings
                            {
                                DefaultValueHandling = DefaultValueHandling.Ignore,
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            });

            }
            else
            {
                //Try bind form
                requests = Utils.Binder.Bind<IEnumerable<ApiBatchRequest>>(context.Request.Form);
            }

            if (requests!=null && requests.Any())
            {
                try
                {
                    Log.Debug("processing batch started");
                    ProcessBatch(context, requests);
                    Log.Debug("processing batch finished");
                }
                catch (Exception e)
                {
                    Log.Error(e, "batch process error");
                    //Set status to error
                    SetError(context, e, HttpStatusCode.InternalServerError);
                    //Try respond again
                    RespondTo(null, context);
                }
            }
            else
            {
                ApiResponce.Error = new ErrorWrapper(new InvalidOperationException("no batch specified"));
            }

            RespondTo(null, context);
        }

        private void ProcessBatch(HttpContextBase context, IEnumerable<ApiBatchRequest> requests)
        {
            var resonse = requests.OrderBy(x => x.Order).Select(x => ProcessBatchRequest(context, x)).ToList();

            ApiResponce.Response = resonse;
            PostProcessResponse(context, resonse);
        }

        internal ApiBatchResponse ProcessBatchRequest(HttpContextBase context, ApiBatchRequest apiBatchRequest)
        {
            if (context.Request == null) throw new InvalidOperationException("Request is empty");
            if (context.Request.Url == null) throw new InvalidOperationException("Url is empty");

            using (var writer = new StringWriter())
            {
                var path = apiBatchRequest.RelativeUrl;
                if (context.Request.ApplicationPath != null && path.StartsWith(context.Request.ApplicationPath))
                {
                    path = path.Substring(context.Request.ApplicationPath.Length);
                }
                var uri = new Uri(context.Request.Url,"/"+path.TrimStart('/'));
                
                var workerRequest = new ApiWorkerRequest(Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),uri.Query.TrimStart('?'), writer, context, new ContentType(apiBatchRequest.BodyContentType));
                workerRequest.HttpVerb = apiBatchRequest.Method;

                if (!string.IsNullOrEmpty(apiBatchRequest.Body))
                {
                    var contentType = new ContentType(apiBatchRequest.BodyContentType);
                    var encoding = Encoding.GetEncoding(contentType.CharSet);
                    workerRequest.EntityBody = encoding.GetBytes(apiBatchRequest.Body);
                }
                var workContext = new HttpContext(workerRequest);

                var newContext = new HttpContextWrapper(workContext);
                
                //Make a faked request
                var routeData = RouteTable.Routes.GetRouteData(newContext);
                if (routeData != null)
                {
                    //Construct new context
                    Container.Resolve<IApiHttpHandler>(new DependencyOverride(typeof(RouteData), routeData)).Process(newContext);
                    newContext.Response.Flush();
                    
                    //Form response
                    var response = new ApiBatchResponse(apiBatchRequest)
                                       {
                                           Data = writer.GetStringBuilder().ToString(),
                                           Headers = new ItemDictionary<string, string>()
                                       };
                    foreach (var responseHeaderKey in workerRequest.ResponseHeaders.AllKeys)
                    {
                        response.Headers.Add(responseHeaderKey, workerRequest.ResponseHeaders[responseHeaderKey]);
                    }
                    response.Status = workerRequest.HttpStatus;
                    response.Name = apiBatchRequest.Name;
                    return response;
                }
                
            }
            return null;
        }
    }
}