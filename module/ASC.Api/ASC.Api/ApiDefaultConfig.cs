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
using System.IO;
using System.Linq;
using ASC.Api.Attributes;
using ASC.Api.Impl;
using ASC.Api.Impl.Invokers;
using ASC.Api.Impl.Poll;
using ASC.Api.Impl.Responders;
using ASC.Api.Impl.Routing;
using ASC.Api.Impl.Serializers;
using ASC.Api.Interfaces;
using ASC.Api.Interfaces.Storage;
using ASC.Api.Logging;
using ASC.Api.Publisher;
using ASC.Api.Utils;
using Microsoft.Practices.Unity;
using Unity.AutoRegistration;

namespace ASC.Api
{
    public static class ApiDefaultConfig
    {
        private static void RegisterIfNever<T, TT>(this IUnityContainer container)
        {
            RegisterIfNever<T, TT>(container,(string)null);
        }

        private static void RegisterIfNever<T, TT>(this IUnityContainer container, string name)
        {
            RegisterIfNever<T, TT>(container, name, new SingletonLifetimeManager());
        }

        private static void RegisterIfNever<T, TT>(this IUnityContainer container, LifetimeManager manager)
        {
            RegisterIfNever<T, TT>(container, null, manager);
        }

        private static void RegisterIfNever<T, TT>(this IUnityContainer container, string name, LifetimeManager manager)
        {
            if (!container.IsRegistered<T>(name))
            {
                container.RegisterType(typeof(T), typeof(TT), name, manager);
            }
        }

        internal static void DoDefaultRegistrations(UnityContainer container)
        {
            container.RegisterIfNever<IApiManager,ApiManager>();
            
            //NOTE: Disable for now
            //container.RegisterIfNever<IApiAsyncHttpHandler, ApiAsyncHttpHandler>(new PerResolveLifetimeManager());
            //container.RegisterIfNever<IApiPubSub, ApiPubSub>();
            //container.RegisterIfNever<IApiRouteHandler,ApiAsyncRouteHandler>(new ContainerControlledLifetimeManager());
            //container.RegisterIfNever<IApiRouteRegistrator, ApiPollRouteRegistrator>("polling");


            container.RegisterIfNever<IApiStandartResponce, ApiStandartResponce>(new NewInstanceLifetimeManager());
            container.RegisterIfNever<IApiResponceFilter, ApiSmartListResponceFilter>("smartfilter", new NewInstanceLifetimeManager());

            container.RegisterIfNever<IApiMethodCall, ApiMethodCall>(new NewInstanceLifetimeManager());
            container.RegisterIfNever<IApiArgumentBuilder, ApiArgumentBuilder>();

            //Serializers
            container.RegisterIfNever<IApiSerializer, JsonNetSerializer>("json.net.serializer");

            //Responders
            container.RegisterIfNever<IApiResponder, ContentResponder>("content_responder");
            container.RegisterIfNever<IApiResponder, DirectResponder>("direct_responder");
            container.RegisterIfNever<IApiResponder, SerializerResponder>("serialzer");

            container.RegisterIfNever<IApiMethodInvoker, ApiSimpleMethodInvoker>();
            container.RegisterIfNever<IApiStoragePath, ApiStoragePath>();
            container.RegisterIfNever<IApiKeyValueStorage, ApiKeyValueInMemoryStorage>();

            container.RegisterIfNever<IApiRouteConfigurator, ApiRouteConfigurator>();
            container.RegisterIfNever<IApiRouteRegistrator, ApiRouteRegistrator>("rest");
            container.RegisterIfNever<IApiRouteRegistrator, ApiBatchRouteRegitrator>("batch");
            container.RegisterIfNever<IApiRouteRegistrator, ApiAccessControlRouteRegistrator>("access");


            container.RegisterIfNever<IApiHttpHandler, ApiHttpHandler>(new NewInstanceLifetimeManager());
            container.RegisterIfNever<IApiRouteHandler, ApiRouteHandler>(new NewInstanceLifetimeManager());
            container.RegisterType<ApiContext>(new ApiContextLifetimeManager());

        }
        
    }
}