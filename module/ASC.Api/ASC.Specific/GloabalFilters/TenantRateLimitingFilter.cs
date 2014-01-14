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
using ASC.Api.GloabalFilters;
using ASC.Api.Logging;

namespace ASC.Specific.GloabalFilters
{


    public class TenantRateLimitingFilter : RateLimitingFilter
    {

        public TenantRateLimitingFilter(int maxRate, int cooldown, bool sliding, string basedomain)
            : base(maxRate, cooldown, sliding, basedomain)
        {
        }

        protected override bool IsNeededToThrottle(System.Web.Routing.RequestContext request)
        {
            if (request.HttpContext.Request.UrlReferrer != null)
            {
                var host = request.HttpContext.Request.UrlReferrer.Host;
                var tenant = Core.CoreContext.TenantManager.GetCurrentTenant();
                if (tenant != null && !string.IsNullOrEmpty(host))
                {
                    if (host.Equals(tenant.TenantDomain, StringComparison.OrdinalIgnoreCase))
                        return false; //Belongs to tennat base domain

                }
            }
            return base.IsNeededToThrottle(request);
        }

    }
}