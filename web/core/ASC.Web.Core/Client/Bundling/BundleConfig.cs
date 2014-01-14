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

using System.Web.Optimization;
using System.Web.Routing;

namespace ASC.Web.Core.Client.Bundling
{
    public static class BundleConfig
    {
        public static void Configure()
        {
            var scriptProvider = new ClientScriptVirtualPathProvider();
            if (ClientSettings.BundlingEnabled)
            {
                BundleTable.VirtualPathProvider = scriptProvider;
            }
            else
            {
                RouteTable.Routes.Add(new Route(BundleHelper.CLIENT_SCRIPT_VPATH.TrimStart('/') + "{path}.js", scriptProvider));
            }
            BundleTable.Bundles.UseCdn = ClientSettings.StoreBundles;
            BundleTable.EnableOptimizations = true;
            PreApplicationStartCode.Start();
        }
    }
}
