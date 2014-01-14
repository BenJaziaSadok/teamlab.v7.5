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
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ASC.Data.Backup.Tasks.Modules;
using ASC.Data.Storage;

namespace ASC.Data.Backup.Tasks
{
    public class RestorePortalTask : PortalTaskBase
    {
        private readonly ColumnMapper _columnMapper;

        public string BackupFilePath { get; private set; }

        public RestorePortalTask(string toConfigPath, string fromFilePath, ColumnMapper columnMapper = null)
            : base(null, toConfigPath)
        {
            if (fromFilePath == null)
                throw new ArgumentNullException("fromFilePath");

            if (!File.Exists(fromFilePath))
                throw new FileNotFoundException("file not found at given path");

            BackupFilePath = fromFilePath;
            _columnMapper = columnMapper ?? new ColumnMapper();
        }

        public override void Run()
        {
            InvokeInfo("begin restore portal");
            
            List<IModuleSpecifics> modulesToProcess = GetModulesToProcess().ToList();
            InitProgress(ProcessStorage ? modulesToProcess.Count + 1 : modulesToProcess.Count);

            using (var dataReader = new ZipReadOperator(BackupFilePath))
            {
                InvokeInfo("begin restore data");

                var dbFactory = new DbFactory(ConfigPath);
                foreach (var module in modulesToProcess)
                {
                    var restoreTask = new RestoreDbModuleTask(module, dataReader, _columnMapper, dbFactory);
                    foreach (var tableName in IgnoredTables)
                    {
                        restoreTask.IgnoreTable(tableName);
                    }
                    RunSubtask(restoreTask);
                }

                InvokeInfo("end restore data");

                if (ProcessStorage)
                {
                    DoRestoreStorage(dataReader);
                }
            }

            InvokeInfo("end restore portal");
        }

        private void DoRestoreStorage(IDataReadOperator dataReader)
        {
            InvokeInfo("begin restore storage");
            var fileGroups = GetFilesToProcess(dataReader).GroupBy(file => file.Module).ToList();
            int groupsProcessed = 0;
            foreach (var group in fileGroups)
            {
                IDataStore storage = StorageFactory.GetStorage(ConfigPath, _columnMapper.GetTenantMapping().ToString(), group.Key, null, null);
                foreach (BackupFileInfo file in group)
                {
                    string adjustedPath = file.Path;

                    IModuleSpecifics module = ModuleProvider.GetByStorageModule(file.Module);
                    if (module == null || module.TryAdjustFilePath(_columnMapper, ref adjustedPath))
                    {
                        Stream stream = dataReader.GetEntry(KeyHelper.GetFileZipKey(file));
                        try
                        {
                            storage.Save(file.Domain, adjustedPath, stream);
                        }
                        catch (Exception error)
                        {
                            InvokeWarning("can't restore file ({0}:{1}): {2}", file.Module, file.Path, error);
                        }
                    }
                }
                SetStepProgress((int)(++groupsProcessed*100/(double)fileGroups.Count));
            }

            if (fileGroups.Count == 0)
                SetStepCompleted();

            InvokeInfo("end restore storage");
        }

        private IEnumerable<BackupFileInfo> GetFilesToProcess(IDataReadOperator dataReader)
        {
            using (Stream stream = dataReader.GetEntry(KeyHelper.GetStorageRestoreInfoZipKey()))
            {
                if (stream == null)
                    return Enumerable.Empty<BackupFileInfo>();

                var restoreInfo = XElement.Load(new StreamReader(stream));
                return restoreInfo.Elements("file").Select(BackupFileInfo.FromXElement).ToList();
            }
        }

        protected override IEnumerable<BackupFileInfo> GetFilesToProcess()
        {
            throw new NotImplementedException();
        }

        protected override bool IsStorageModuleAllowed(string storageModuleName)
        {
            if (storageModuleName == "fckuploaders")
                return false;
            return base.IsStorageModuleAllowed(storageModuleName);
        }
    }
}
