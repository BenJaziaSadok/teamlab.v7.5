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

using ASC.Core;
using ASC.Core.Configuration;
using ASC.Core.Tenants;

namespace ASC.Xmpp.Host
{
    static class ASCContext
    {
        public static IUserManagerClient UserManager
        {
            get
            {
                return CoreContext.UserManager;
            }
        }

        public static IAuthManagerClient Authentication
        {
            get
            {
                return CoreContext.Authentication;
            }
        }

        public static IGroupManagerClient GroupManager
        {
            get
            {
                return CoreContext.GroupManager;
            }
        }

        public static Tenant GetCurrentTenant()
        {
            return CoreContext.TenantManager.GetCurrentTenant(false);
        }

        public static void SetCurrentTenant(string domain)
        {
            SecurityContext.AuthenticateMe(Constants.CoreSystem);

            var current = CoreContext.TenantManager.GetCurrentTenant(false);
            if (current == null || string.Compare(current.TenantDomain, domain, true) != 0)
            {
                CoreContext.TenantManager.SetCurrentTenant(domain);
            }
        }
    }
}