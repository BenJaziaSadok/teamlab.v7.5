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

using System.Configuration;
using ASC.Core.Billing;
using ASC.Core.Caching;
using ASC.Core.Configuration;
using ASC.Core.Data;
using ASC.Core.Notify;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace ASC.Core
{
    public static class CoreContext
    {
        static CoreContext()
        {
            var unityConfigurationSection = ConfigurationManager.GetSection("unity");
            if (unityConfigurationSection == null)
            {
                ConfigureCoreContextByDefault();
            }
            else
            {
                ConfigureCoreContextByUnity(unityConfigurationSection);
            }
        }


        public static IConfigurationClient Configuration
        {
            get;
            private set;
        }

        public static ITenantManagerClient TenantManager
        {
            get;
            private set;
        }

        public static IUserManagerClient UserManager
        {
            get;
            private set;
        }

        public static IGroupManagerClient GroupManager
        {
            get;
            private set;
        }

        public static IAuthManagerClient Authentication
        {
            get;
            private set;
        }

        public static IAzManagerClient AuthorizationManager
        {
            get;
            private set;
        }

        public static IPaymentManagerClient PaymentManager
        {
            get;
            private set;
        }

        internal static ISubscriptionManagerClient SubscriptionManager
        {
            get;
            private set;
        }


        private static void ConfigureCoreContextByDefault()
        {
            var cs = ConfigurationManager.ConnectionStrings["core"];
            if (cs == null)
            {
                throw new ConfigurationErrorsException("Can not configure CoreContext: connection string with name core not found.");
            }

            var tenantService = new CachedTenantService(new DbTenantService(cs));
            var userService = new CachedUserService(new DbUserService(cs));
            var azService = new CachedAzService(new DbAzService(cs));
            var quotaService = new CachedQuotaService(new DbQuotaService(cs));
            var subService = new CachedSubscriptionService(new DbSubscriptionService(cs));
            var tariffService = new TariffService(cs, quotaService, tenantService);

            Configuration = new ClientConfiguration(tenantService);
            TenantManager = new ClientTenantManager(tenantService, quotaService, tariffService);
            PaymentManager = new ClientPaymentManager(Configuration, quotaService, tariffService);
            UserManager = new ClientUserManager(userService);
            GroupManager = new ClientUserManager(userService);
            Authentication = new ClientAuthManager(userService);
            AuthorizationManager = new ClientAzManager(azService);
            SubscriptionManager = new ClientSubscriptionManager(subService);
        }

        private static void ConfigureCoreContextByUnity(object section)
        {
            ConfigureCoreContextByDefault();

            if (((UnityConfigurationSection)section).Containers["Core"] != null)
            {
                var unity = new UnityContainer().LoadConfiguration("Core");
                if (unity.IsRegistered<IConfigurationClient>())
                {
                    Configuration = unity.Resolve<IConfigurationClient>();
                }
                if (unity.IsRegistered<ITenantManagerClient>())
                {
                    TenantManager = unity.Resolve<ITenantManagerClient>();
                }
                if (unity.IsRegistered<IUserManagerClient>())
                {
                    UserManager = unity.Resolve<IUserManagerClient>();
                }
                if (unity.IsRegistered<IGroupManagerClient>())
                {
                    GroupManager = unity.Resolve<IGroupManagerClient>();
                }
                if (unity.IsRegistered<IAuthManagerClient>())
                {
                    Authentication = unity.Resolve<IAuthManagerClient>();
                }
                if (unity.IsRegistered<IAzManagerClient>())
                {
                    AuthorizationManager = unity.Resolve<IAzManagerClient>();
                }
                if (unity.IsRegistered<ISubscriptionManagerClient>())
                {
                    SubscriptionManager = unity.Resolve<ISubscriptionManagerClient>();
                }
            }
        }
    }
}