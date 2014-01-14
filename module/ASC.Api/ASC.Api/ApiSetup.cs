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

using System.Web.Routing;
using ASC.Api.Interfaces;
using Microsoft.Practices.EnterpriseLibrary.Caching.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace ASC.Api
{
    public static class ApiSetup
    {
        private static object locker = new object();

        private static volatile bool initialized = false;

        private static UnityServiceLocator locator;

        public static UnityContainer Container { get; private set; }


        static ApiSetup()
        {
        }


        private static void Init()
        {
            if (!initialized)
            {
                lock (locker)
                {
                    if (!initialized)
                    {
                        Container = new UnityContainer();
                        locator = new UnityServiceLocator(Container);

                        Container.AddNewExtension<EnterpriseLibraryCoreExtension>();

                        ServiceLocator.SetLocatorProvider(() => locator);
                        Container.LoadConfiguration("api");
                        ApiDefaultConfig.DoDefaultRegistrations(Container);

                        initialized = true;
                    }
                }
            }
        }


        public static void RegisterRoutes()
        {
            Init();

            var registrators = Container.ResolveAll<IApiRouteRegistrator>();
            foreach (var registrator in registrators)
            {
                registrator.RegisterRoutes(RouteTable.Routes);
            }
        }

        public static IUnityContainer ConfigureEntryPoints()
        {
            Init();

            //Do boot stuff
            var configurator = Container.Resolve<IApiRouteConfigurator>();
            configurator.RegisterEntryPoints();

            //Do boot auto search
            var boot = Container.ResolveAll<IApiBootstrapper>();
            foreach (var apiBootstrapper in boot)
            {
                apiBootstrapper.Configure();
            }

            return Container;
        }
    }
}