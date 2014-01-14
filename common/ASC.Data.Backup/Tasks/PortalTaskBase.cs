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
using ASC.Data.Backup.Tasks.Modules;
using ASC.Data.Storage;

namespace ASC.Data.Backup.Tasks
{
    public abstract class PortalTaskBase : ProgressTask
    {
        protected readonly List<ModuleName> IgnoredModules = new List<ModuleName>();
        protected readonly List<string> IgnoredTables = new List<string>(); //todo: add using to backup and transfer tasks

        public Tenant Tenant { get; private set; }
        public string ConfigPath { get; private set; }

        public bool ProcessStorage { get; set; }

        protected PortalTaskBase(Tenant tenant, string configPath)
        {
            Tenant = tenant;
            ConfigPath = configPath;
            ProcessStorage = true;
        }

        public void IgnoreModule(ModuleName moduleName)
        {
            if (!IgnoredModules.Contains(moduleName))
                IgnoredModules.Add(moduleName);
        }

        public void IgnoreTable(string tableName)
        {
            if (!IgnoredTables.Contains(tableName))
                IgnoredTables.Add(tableName);
        }

        internal virtual IEnumerable<IModuleSpecifics> GetModulesToProcess()
        {
            return ModuleProvider.GetAll().Where(module => !IgnoredModules.Contains(module.ModuleName));
        }

        protected virtual IEnumerable<BackupFileInfo> GetFilesToProcess()
        {
            var files = new List<BackupFileInfo>();
            foreach (var module in StorageFactory.GetModuleList(ConfigPath).Where(IsStorageModuleAllowed))
            {
                var store = StorageFactory.GetStorage(ConfigPath, Tenant.TenantId.ToString(), module, null, null);
                var domains = StorageFactory.GetDomainList(ConfigPath, module).ToArray();

                foreach (var domain in domains)
                {
                    files.AddRange(
                        store.ListFilesRelative(domain, "\\", "*.*", true)
                             .Select(path => new BackupFileInfo(domain, module, path)));
                }

                files.AddRange(
                    store.ListFilesRelative(string.Empty, "\\", "*.*", true)
                         .Where(path => domains.All(domain => !path.Contains(domain + "/")))
                         .Select(path => new BackupFileInfo(string.Empty, module, path)));
            }

            return files.Distinct();
        }

        protected virtual bool IsStorageModuleAllowed(string storageModuleName)
        {
            var allowedStorageModules = new List<string>
                {
                    "forum",
                    "photo",
                    "bookmarking",
                    "wiki",
                    "files",
                    "crm",
                    "projects",
                    "logo",
                    "fckuploaders",
                    "talk",
                    "mailaggregator"
                };

            if (!allowedStorageModules.Contains(storageModuleName))
                return false;

            IModuleSpecifics moduleSpecifics = ModuleProvider.GetByStorageModule(storageModuleName);
            return moduleSpecifics == null || !IgnoredModules.Contains(moduleSpecifics.ModuleName);
        }
    }
}
