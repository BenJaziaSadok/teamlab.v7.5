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

using System.Collections.Generic;
using System.Linq;
using ASC.Core.Tenants;
using ASC.Data.Backup.Exceptions;
using ASC.Data.Backup.Extensions;
using ASC.Data.Backup.Tasks.Data;
using ASC.Data.Backup.Tasks.Modules;
using ASC.Data.Storage;

namespace ASC.Data.Backup.Tasks
{
    public class DeletePortalTask : PortalTaskBase
    {
        public DeletePortalTask(Tenant tenant, string configPath)
            : base(tenant, configPath)
        {
        }

        public override void Run()
        {
            InvokeInfo("begin delete portal ({0})", Tenant.TenantAlias);
            List<IModuleSpecifics> modulesToProcess = GetModulesToProcess().Reverse().ToList();
            InitProgress(ProcessStorage ? modulesToProcess.Count + 1 : modulesToProcess.Count);
            var dbFactory = new DbFactory(ConfigPath);
            foreach (var module in modulesToProcess)
            {
                DoDeleteModule(dbFactory, module);
            }
            if (ProcessStorage)
            {
                DoDeleteStorage();
            }
            InvokeInfo("end delete portal ({0})", Tenant.TenantAlias);
        }

        private void DoDeleteModule(DbFactory dbFactory, IModuleSpecifics module)
        {
            InvokeInfo("begin delete data for module ({0})", module.ModuleName);
            int tablesCount = module.Tables.Count();
            int tablesProcessed = 0;
            using (var connection = dbFactory.OpenConnection(module.ConnectionStringName))
            {
                foreach (var table in module.GetTablesOrdered().Reverse().Where(t => !IgnoredTables.Contains(t.Name)))
                {
                    ActionInvoker.Try(state =>
                        {
                            var t = (TableInfo)state;
                            module.CreateDeleteCommand(connection.Fix(), Tenant.TenantId, t).WithTimeout(120).ExecuteNonQuery();
                        }, table, 5, onFailure: error => { throw ThrowHelper.CantDeleteTable(table.Name, error); });
                    SetStepProgress((int)((++tablesProcessed*100)/(double)tablesCount));
                }
            }
            InvokeInfo("end delete data for module ({0})", module.ModuleName);
        }

        private void DoDeleteStorage()
        {
            InvokeInfo("begin delete storage");
            List<string> storageModules = StorageFactory.GetModuleList(ConfigPath).Where(IsStorageModuleAllowed).ToList();
            int modulesProcessed = 0;
            foreach (string module in storageModules)
            {
                IDataStore storage = StorageFactory.GetStorage(ConfigPath, Tenant.TenantId.ToString(), module, null, null);
                List<string> domains = StorageFactory.GetDomainList(ConfigPath, module).ToList();
                foreach (var domain in domains)
                {
                    ActionInvoker.Try(state => storage.DeleteFiles((string)state, "\\", "*.*", true), domain, 5,
                                      onFailure: error => InvokeWarning("Can't delete files for domain {0}: \r\n{1}", domain, error));
                }
                storage.DeleteFiles("\\", "*.*", true);
                SetStepProgress((int)((++modulesProcessed*100)/(double)storageModules.Count));
            }
            InvokeInfo("end delete storage");
        }
    }
}
