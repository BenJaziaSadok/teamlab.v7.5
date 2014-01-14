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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.IO;


namespace ASC.Mail.Watchdog.Service
{
    [RunInstaller(true)]
    public partial class WatchdogServiceInstaller : Installer
    {
        public WatchdogServiceInstaller()
        {
            InitializeComponent();
            InstallingService();
        }
        private void InstallingService()
        {
            ServiceProcessInstaller process = new ServiceProcessInstaller();

            process.Account = ServiceAccount.LocalSystem;

            ServiceInstaller serviceAdmin = new ServiceInstaller();

            serviceAdmin.StartType = ServiceStartMode.Automatic;
            string sServiceName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
            serviceAdmin.ServiceName = sServiceName;
            serviceAdmin.DisplayName = sServiceName;
            serviceAdmin.Description = sServiceName;

            Installers.Add(process);
            Installers.Add(serviceAdmin);
        }
    }
}
