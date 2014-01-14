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

using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace ASC.Mail.Aggregator.CollectionService
{

        [RunInstaller(true)]
        public class CollectorServiceInstaller : Installer
        {
            /// <summary>

            /// Public Constructor for WindowsServiceInstaller.

            /// - Put all of your Initialization code here.

            /// </summary>

            public CollectorServiceInstaller()
            {
                ServiceProcessInstaller serviceProcessInstaller =
                                   new ServiceProcessInstaller();
                ServiceInstaller serviceInstaller = new ServiceInstaller();

                //# Service Account Information

                serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
                serviceProcessInstaller.Username = null;
                serviceProcessInstaller.Password = null;

                //# Service Information

                serviceInstaller.DisplayName = CollectorService.AscMailCollectionServiceName;
                serviceInstaller.StartType = ServiceStartMode.Automatic;

                serviceInstaller.ServiceName = CollectorService.AscMailCollectionServiceName;
                this.Installers.Add(serviceProcessInstaller);
                this.Installers.Add(serviceInstaller);
            }
        }
    
}