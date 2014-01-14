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
using System.ServiceModel;
using ASC.Common.Data;
using ASC.Common.Module;
using ASC.Notify.Config;
using ASC.Web.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;
using log4net;
using log4net.Config;
using TMResourceData;

namespace ASC.Notify
{
    public class NotifyServiceLauncher : IServiceController
    {
        private ServiceHost serviceHost;
        private NotifyService service;


        public void Start()
        {
            XmlConfigurator.Configure();

            serviceHost = new ServiceHost(typeof(NotifyService));
            serviceHost.Open();

            service = new NotifyService();
            service.StartSending();

            if (0 < NotifyServiceCfg.Schedulers.Count)
            {
                InitializeNotifySchedulers();
            }
        }

        public void Stop()
        {
            if (service != null)
            {
                service.StopSending();
            }
            if (serviceHost != null)
            {
                serviceHost.Close();
            }
        }

        private void InitializeNotifySchedulers()
        {
            CommonLinkUtility.Initialize(NotifyServiceCfg.ServerRoot);
            DbRegistry.Configure();
            InitializeDbResources();
            NotifyConfiguration.Configure();
            WebItemManager.Instance.LoadItems();
            foreach (var pair in NotifyServiceCfg.Schedulers)
            {
                LogManager.GetLogger("ASC.Notify").DebugFormat("Start scheduler {0} ({1})", pair.Key, pair.Value);
                pair.Value.Invoke(null, null);
            }
        }

        private void InitializeDbResources()
        {
            AssemblyWork.UploadResourceData(AppDomain.CurrentDomain.GetAssemblies());
            AppDomain.CurrentDomain.AssemblyLoad += (sender, args) => AssemblyWork.UploadResourceData(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}
