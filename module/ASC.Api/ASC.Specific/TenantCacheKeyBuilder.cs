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
using ASC.Api.Impl;
using ASC.Api.Interfaces;
using ASC.Api.Interfaces.Cache;

namespace ASC.Specific
{
    public class TenantCacheKeyBuilder : ApiCacheKeyBuilder
    {
        public override string BuildCacheKeyForMethodCall(IApiMethodCall apiMethodCall, IEnumerable<object> callArgs, ApiContext context)
        {
            return Core.CoreContext.TenantManager.GetCurrentTenant().TenantId + base.BuildCacheKeyForMethodCall(apiMethodCall,callArgs,context);
        }
    }
}