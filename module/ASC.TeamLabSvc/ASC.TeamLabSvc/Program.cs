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
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using ASC.Common.Module;
using ASC.TeamLabSvc.Configuration;
using log4net;
using log4net.Config;

namespace ASC.TeamLabSvc
{
    sealed class Program : ServiceBase
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.TeamLabSvc");
        private static List<IServiceController> services = new List<IServiceController>();

        private static void Main(string[] args)
        {
#if DEBUG
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["debugBreak"]))
            {
                Debugger.Launch();
            }
#endif
            XmlConfigurator.Configure();

            var program = new Program();
            if (Environment.UserInteractive)
            {
                program.OnStart(args);

                Console.WriteLine("\r\nPress any key to stop...\r\n");
                Console.ReadKey();

                program.OnStop();
            }
            else
            {
                Run(program);
            }
        }


        protected override void OnStart(string[] args)
        {
            try
            {
                var section = TeamLabSvcConfigurationSection.GetSection();
                foreach (TeamLabSvcConfigurationElement e in section.TeamlabServices)
                {
                    if (!e.Disable)
                    {
                        services.Add((IServiceController)Activator.CreateInstance(Type.GetType(e.Type, true)));
                    }
                    else
                    {
                        log.InfoFormat("Skip service {0}", e.Type);
                    }
                }
            }
            catch (Exception error)
            {
                log.ErrorFormat("Can not start services: {0}", error);
                return;
            }

            foreach (var s in services)
            {
                try
                {
                    s.Start();
                    log.InfoFormat("Service {0} started.", GetServiceName(s));
                }
                catch (Exception error)
                {
                    log.ErrorFormat("Can not start service {0}: {1}", GetServiceName(s), error);
                }
            }
        }

        protected override void OnStop()
        {
            foreach (var s in services)
            {
                try
                {
                    s.Stop();
                    log.InfoFormat("Service {0} stopped.", GetServiceName(s));
                }
                catch (Exception error)
                {
                    log.ErrorFormat("Can not stop service {0}: {1}", GetServiceName(s), error);
                }
            }

            services.Clear();
        }

        private string GetServiceName(IServiceController controller)
        {
            var type = controller.GetType();
            var attributes = type.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            return 0 < attributes.Length ? ((DisplayNameAttribute)attributes[0]).DisplayName : type.Name;
        }
    }
}
