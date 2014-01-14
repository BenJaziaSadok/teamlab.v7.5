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

using ASC.Api.Attributes;
using ASC.Core;
using System.Threading;

namespace ASC.Specific.GloabalFilters
{
    public class UserCultureFilter : ApiCallFilter
    {
        public override void PreMethodCall(ASC.Api.Interfaces.IApiMethodCall method, ASC.Api.Impl.ApiContext context, System.Collections.Generic.IEnumerable<object> arguments)
        {
            //Check culture and set
            var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
            if (tenant != null)
            {
                var culture = tenant.GetCulture();
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }

            if (SecurityContext.IsAuthenticated)
            {
                var culture = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).GetCulture();
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }
    }
}