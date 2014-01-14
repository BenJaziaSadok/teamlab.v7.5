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
using System.Linq;
using ASC.Api.Interfaces;
using ASC.Api.Publisher;
using Microsoft.Practices.ServiceLocation;

namespace ASC.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PollAttribute : ApiCallFilter
    {
        private const string DefaultPoll = "poll";

        public string PollUrl { get; set; }

        public PollAttribute()
            : this(DefaultPoll)
        {
        }

        public PollAttribute(string pollUrl)
        {
            PollUrl = pollUrl;
        }

        public override void PostMethodCall(IApiMethodCall method, ASC.Api.Impl.ApiContext context, object methodResponce)
        {
            var pubSub = ServiceLocator.Current.GetInstance<IApiPubSub>();

            if (pubSub != null)
            {
                pubSub.PublishDataForKey(
                    method.RoutingPollUrl + ":" +
                    PubSubKeyHelper.GetKeyForRoute(context.RequestContext.RouteData.Route.GetRouteData(context.RequestContext.HttpContext)),
                    new ApiMethodCallData(){Method = method,Result = methodResponce});
            }
            base.PostMethodCall(method, context, methodResponce);
        }
    }
}