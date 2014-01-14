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

using System.ServiceModel;
using ASC.Common.Module;
using log4net.Config;

namespace ASC.Data.Backup.Service
{
    public class BackupServiceLauncher : IServiceController
    {
        private ServiceHost host;


        public void Start()
        {
            XmlConfigurator.Configure();

            BackupService.Initialize();
            host = new ServiceHost(typeof(BackupService));
            host.Open();
        }

        public void Stop()
        {
            BackupService.Terminate();
            if (host != null)
            {
                host.Close();
                host = null;
            }
        }
    }
}
