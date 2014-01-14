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
using System.Globalization;
using System.Linq;
using System.Security;
using System.Threading;
using System.Web.Routing;
using ASC.Api.Attributes;
using ASC.Api.Logging;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;
using Microsoft.Practices.ServiceLocation;

namespace ASC.Api.GloabalFilters
{


    public class RateLimitingFilter : ApiCallFilter
    {
        private readonly int _cooldown;
        private readonly bool _sliding;
        private readonly string _basedomain;
        private readonly ILog _log;

        private ICacheItemExpiration Time
        {
            get
            {
                if (_sliding)
                {
                    return new SlidingTime(TimeSpan.FromMilliseconds(_cooldown));
                }
                return new AbsoluteTime(TimeSpan.FromMilliseconds(_cooldown));
            }
        }

        private class CallCount
        {
            private int _count;

            internal CallCount(int count)
            {
                _count = count;
            }

            internal int AddCall()
            {
                return Interlocked.Add(ref _count, 1);
            }
        }

        public int MaxRate { get; set; }

        public RateLimitingFilter(int maxRate, int cooldown, bool sliding, string basedomain)
        {
            _cooldown = cooldown;
            _sliding = sliding;
            _basedomain = basedomain;
            _log = ServiceLocator.Current.GetInstance<ILog>();

            MaxRate = maxRate;
        }

        public override void PreMethodCall(Interfaces.IApiMethodCall method, Impl.ApiContext context, System.Collections.Generic.IEnumerable<object> arguments)
        {
            //Store to cache
            try
            {
                if (!IsNeededToThrottle(context.RequestContext))//Local server requests
                {
                    return;
                } 

                //Try detect referer if it's from site call
                var cache = ServiceLocator.Current.GetInstance<ICacheManager>();
                var callCount = cache[method + context.RequestContext.HttpContext.Request.UserHostAddress] as CallCount;
                if (callCount == null)
                {
                    //This means it's not in cache
                    cache.Add(method + context.RequestContext.HttpContext.Request.UserHostAddress, new CallCount(1), CacheItemPriority.Normal, null, Time);
                }
                else
                {
                    if (callCount.AddCall() > MaxRate)
                    {
                        context.RequestContext.HttpContext.Response.AddHeader("Retry-After", ((int)TimeSpan.FromMilliseconds(_cooldown).TotalSeconds).ToString(CultureInfo.InvariantCulture));
                        context.RequestContext.HttpContext.Response.StatusCode = 503;
                        context.RequestContext.HttpContext.Response.StatusDescription = "Limit reached";
                        _log.Warn("limiting requests for {0} to cd:{1}", method + context.RequestContext.HttpContext.Request.UserHostAddress, _cooldown);
                        throw new SecurityException("Rate limit reached. Try again after " + _cooldown + " ms");
                    }
                }
            }
            catch (SecurityException e)
            {
                _log.Error(e,"limit requests");
                throw;//Throw this exceptions
            }
            catch (Exception)
            {

            }
        }

        protected virtual bool IsNeededToThrottle(RequestContext request)
        {
            if (request.HttpContext.Request.IsLocal)
                return false;
            if (request.HttpContext.Request.UrlReferrer != null &&
                request.HttpContext.Request.UrlReferrer.Host.EndsWith(_basedomain))
            {
                return false;
            }
            return true;
        }
    }
}