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

using ASC.Common.Module;
using ASC.Core.Data;
using ASC.Core.Tenants;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;

namespace ASC.Core.Billing
{
    class TariffSyncService : ITariffSyncService, IServiceController
    {
        private readonly static ILog log = LogManager.GetLogger(typeof(TariffSyncService));
        private readonly TariffSyncServiceSection config;
        private readonly IDictionary<int, IEnumerable<TenantQuota>> quotaServices = new Dictionary<int, IEnumerable<TenantQuota>>();
        private Timer timer;


        public TariffSyncService()
        {
            config = TariffSyncServiceSection.GetSection();
        }


        // server part of service
        public IEnumerable<TenantQuota> GetTariffs(int version, string key)
        {
            lock (quotaServices)
            {
                if (!quotaServices.ContainsKey(version))
                {
                    var cs = ConfigurationManager.ConnectionStrings[config.ConnectionStringName + version] ??
                             ConfigurationManager.ConnectionStrings[config.ConnectionStringName];
                    quotaServices[version] = new DbQuotaService(cs).GetTenantQuotas();
                }
                return quotaServices[version];
            }
        }


        // client part of service
        public string ServiceName
        {
            get { return "Tariffs synchronizer"; }
        }

        public void Start()
        {
            if (timer == null)
            {
                timer = new Timer(Sync, null, TimeSpan.Zero, config.Period);
            }
        }

        public void Stop()
        {
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                timer = null;
            }
        }

        private void Sync(object _)
        {
            try
            {
                var tenant = CoreContext.TenantManager.GetTenants().OrderByDescending(t => t.Version).FirstOrDefault();
                if (tenant != null)
                {
                    using (var wcfClient = new TariffSyncClient())
                    {
                        var quotaService = new DbQuotaService(ConfigurationManager.ConnectionStrings[config.ConnectionStringName]);

                        var oldtariffs = quotaService.GetTenantQuotas().ToDictionary(t => t.Id);
                        // save new
                        foreach (var tariff in wcfClient.GetTariffs(tenant.Version, CoreContext.Configuration.GetKey(tenant.TenantId)))
                        {
                            quotaService.SaveTenantQuota(tariff);
                            oldtariffs.Remove(tariff.Id);
                        }

                        // remove old
                        foreach (var tariff in oldtariffs.Values)
                        {
                            tariff.Visible = false;
                            quotaService.SaveTenantQuota(tariff);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                log.Error(error);
            }
        }
    }
}
