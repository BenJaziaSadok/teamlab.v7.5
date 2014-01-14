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
using System.Collections.Generic;
using ASC.Common.Module;
using ASC.Web.Host.Config;
using ASC.Web.Host.HttpRequestProcessor;
using log4net;

namespace ASC.Web.Host
{
    public class WebServerLauncher : IServiceController
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Web.Host");
        private readonly List<IServer> webServers = new List<IServer>();


        public void Start()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            ServerConfiguration.Configure();
            foreach (SiteElement s in ServerConfiguration.Sites)
            {
                var webServer = new Server(s.Binding, s.Path);
                webServer.Start();

                log.InfoFormat("Web server start site {0} on {1}", s.Path, s.Binding);
                webServers.Add(webServer);
            }
        }

        public void Stop()
        {
            foreach (var webServer in webServers)
            {
                webServer.Stop();
            }
            webServers.Clear();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Error(e.ExceptionObject);
        }
    }
}
