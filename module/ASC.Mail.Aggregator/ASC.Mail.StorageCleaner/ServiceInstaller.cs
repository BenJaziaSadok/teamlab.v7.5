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
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using System.IO;


namespace ASC.Mail.StorageCleaner
{
    [RunInstaller(true)]
    public class ServiceInstaller : Installer
    {
        public ServiceInstaller()
        {
            InstallingService();
        }

        private void InstallingService()
        {
            var process = new ServiceProcessInstaller {Account = ServiceAccount.LocalSystem};

            var s_service_name = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);

            var service_admin = new System.ServiceProcess.ServiceInstaller
                {
                    StartType = ServiceStartMode.Automatic,
                    ServiceName = s_service_name,
                    DisplayName = s_service_name,
                    Description = s_service_name
                };

            Installers.Add(process);
            Installers.Add(service_admin);
        }
    }
}
