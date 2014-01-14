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
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Xml.Linq;

namespace ASC.Data.Backup.Restore
{
    class Restarter : IBackupProvider
    {
        public string Name
        {
            get { return "restarter"; }
        }

        public IEnumerable<XElement> GetElements(int tenant, string[] configs, IDataWriteOperator writer)
        {
            return null;
        }

        public void LoadFrom(IEnumerable<XElement> elements, int tenant, string[] configs, IDataReadOperator reader)
        {
            try
            {
                RestartService("TeamlabOfficeServer");
                KillProcesses("w3wp");
                KillProcesses("aspnet_wp");
            }
            catch { }
            finally
            {
                OnProgressChanged("OK", 100);
            }
        }

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;


        private void OnProgressChanged(string status, int progress)
        {
            try
            {
                if (ProgressChanged != null) ProgressChanged(this, new ProgressChangedEventArgs(status, progress));
            }
            catch { }
        }


        private void RestartService(string serviceName)
        {
            var service = ServiceController.GetServices().Where(s => s.ServiceName == serviceName).FirstOrDefault();
            if (service != null)
            {
                var controllers = new List<ServiceController>();
                controllers.AddRange(service.DependentServices);
                controllers.Add(service);
                RestartServices(controllers);
            }
        }

        private void RestartServices(IEnumerable<ServiceController> controllers)
        {
            var timeout = TimeSpan.FromSeconds(30);
            var count = controllers.Count() * 2 + 1;
            var counter = 0;

            foreach (var controller in controllers)
            {
                try
                {
                    OnProgressChanged("Stopping service " + controller.DisplayName, counter++ * 100 / count);
                    WaitPendingStatuses(controller, timeout);
                    if (controller.Status != ServiceControllerStatus.Stopped)
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    }
                }
                catch (Exception) { }
            }
            
            foreach (var controller in controllers.Reverse())
            {
                try
                {
                    OnProgressChanged("Starting service " + controller.DisplayName, counter++ * 100 / count);
                    WaitPendingStatuses(controller, timeout);
                    if (controller.Status == ServiceControllerStatus.Stopped)
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running, timeout);
                    }
                }
                catch (Exception) { }
            }
        }

        private void WaitPendingStatuses(ServiceController controller, TimeSpan timeout)
        {
            if (controller.Status == ServiceControllerStatus.StartPending)
            {
                controller.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            if (controller.Status == ServiceControllerStatus.PausePending)
            {
                controller.WaitForStatus(ServiceControllerStatus.Paused, timeout);
            }
            if (controller.Status == ServiceControllerStatus.ContinuePending)
            {
                controller.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            if (controller.Status == ServiceControllerStatus.StopPending)
            {
                controller.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
        }

        private void KillProcesses(string processName)
        {
            OnProgressChanged("Restart process " + processName, 95);
            foreach (var process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }
    }
}
