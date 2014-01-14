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
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using ASC.Common.Threading.Progress;
using ASC.Core;
using ASC.Core.Common.Contracts;
using ASC.Core.Tenants;
using ASC.Data.Backup.Tasks;
using ASC.Data.Backup.Tasks.Modules;
using ASC.Data.Storage;
using log4net;

namespace ASC.Data.Backup.Service
{
    internal class BackupService : IBackupService
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Backup");
        private static ProgressQueue tasks;
        private static Timer cleaner;
        private static TimeSpan expire;
        private static string currentWebConfigPath;
        private static string tmpfolder;

        public static void Initialize()
        {
            var backupConfiguration = BackupConfigurationSection.GetSection();

            currentWebConfigPath = ToAbsolute(backupConfiguration.RegionConfigs.GetCurrentConfig());
            if (!currentWebConfigPath.EndsWith(".config", StringComparison.OrdinalIgnoreCase))
            {
                currentWebConfigPath = Path.Combine(currentWebConfigPath, "web.config");
            }

            tmpfolder = ToAbsolute(backupConfiguration.TmpFolder);
            expire = backupConfiguration.ExpirePeriod;

            tasks = new ProgressQueue(backupConfiguration.ThreadCount, TimeSpan.FromMinutes(15), false);
            cleaner = new Timer(Clean, expire, expire, expire);

            ThreadPool.QueueUserWorkItem(state =>
                {
                    foreach (var tenant in CoreContext.TenantManager
                                                      .GetTenants()
                                                      .Where(t => t.Status == TenantStatus.Transfering))
                    {
                        tenant.SetStatus(TenantStatus.Active);
                        CoreContext.TenantManager.SaveTenant(tenant);
                        var url = tenant.TenantDomain;
                        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                        {
                            url = "http://" + url;
                        }
                        NotifyHelper.SendAboutTransferError(tenant.TenantId, string.Empty, url, string.Empty, true);
                    }
                });
        }

        public static void Terminate()
        {
            if (tasks != null)
            {
                tasks.Terminate();
                tasks = null;
            }
            if (cleaner != null)
            {
                cleaner.Change(Timeout.Infinite, Timeout.Infinite);
                cleaner.Dispose();
                cleaner = null;
            }
        }

        public BackupResult CreateBackup(int tenantID, Guid userID)
        {
            lock (tasks.SynchRoot)
            {
                var task = tasks.GetItems().OfType<BackupTask>().FirstOrDefault(t => t.Tenant == tenantID);
                if (task != null && task.IsCompleted)
                {
                    tasks.Remove(task);
                    task = null;
                }
                if (task == null)
                {
                    task = new BackupTask(tenantID, userID);
                    tasks.Add(task);
                }
                return ToResult(task);
            }
        }

        public BackupResult GetBackupStatus(string id)
        {
            lock (tasks.SynchRoot)
            {
                return ToResult(tasks.GetItems().OfType<BackupTask>().FirstOrDefault(t => (string)t.Id == id));
            }
        }

        public BackupResult TransferPortal(TransferRequest request)
        {
            lock (tasks.SynchRoot)
            {
                var task = tasks.GetItems().OfType<TransferTask>().FirstOrDefault(x => (int)x.Id == request.TenantId);
                if (task != null && task.IsCompleted)
                {
                    tasks.Remove(task);
                    task = null;
                }
                if (task == null)
                {
                    if (request.TargetRegion == BackupConfigurationSection.GetSection().RegionConfigs.CurrentRegion)
                    {
                        throw new FaultException("Current and target regions are the same!");
                    }
                    task = new TransferTask(request);
                    tasks.Add(task);
                }
                return ToResult(task);
            }
        }

        public BackupResult GetTransferStatus(int tenantID)
        {
            lock (tasks.SynchRoot)
            {
                return ToResult(tasks.GetItems().OfType<TransferTask>().FirstOrDefault(t => (int)t.Id == tenantID));
            }
        }

        public BackupResult RestorePortal(int tenantID, string demoID)
        {
            lock (tasks.SynchRoot)
            {
                IProgressItem task = tasks.GetItems().FirstOrDefault(t => (int)t.Id == tenantID);
                if (task != null && task.IsCompleted)
                {
                    tasks.Remove(task);
                    task = null;
                }
                if (task == null)
                {
                    var config = BackupConfigurationSection.GetSection();
                    var demoPortal = config.DemoPortals.GetDemoPortal(demoID) ?? config.DemoPortals.Default;
                    if (demoPortal == null)
                        throw new FaultException("Can't find demo portal with id = " + demoID);
                    task = new RestoreTask(tenantID, demoPortal.DataPath);
                    tasks.Add(task);
                }
                return ToResult(task);
            }
        }

        public BackupResult GetRestoreStatus(int tenantID)
        {
            lock (tasks.SynchRoot)
            {
                return ToResult(tasks.GetItems().OfType<RestoreTask>().FirstOrDefault(t => (int)t.Id == tenantID));
            }
        }

        public List<TransferRegion> GetTransferRegions()
        {
            var config = BackupConfigurationSection.GetSection();
            var currentRegion = config.RegionConfigs.CurrentRegion;

            return (from WebConfigElement configFileElement in config.RegionConfigs
                    let webConfig = ConfigurationHelper.OpenConfiguration(ToAbsolute(configFileElement.Path))
                    let baseDomain = webConfig.AppSettings.Settings["core.base-domain"].Value
                    select new TransferRegion
                        {
                            Name = configFileElement.Region,
                            BaseDomain = baseDomain,
                            IsCurrentRegion = configFileElement.Region.Equals(currentRegion, StringComparison.InvariantCultureIgnoreCase)
                        }).ToList();
        }

        public List<string> GetDemoData()
        {
            return BackupConfigurationSection.GetSection().DemoPortals.Cast<DemoPortalElement>().Select(demoData => demoData.ID).ToList();
        }

        private static BackupResult ToResult(IProgressItem task)
        {
            if (task == null)
                return null;

            if (task.Error != null)
                throw new FaultException(((Exception)task.Error).Message);

            var result = new BackupResult
                {
                    Id = task.Id.ToString(),
                    Completed = task.IsCompleted,
                    Percent = (int)task.Percentage,
                };

            var backupTask = task as BackupTask;
            if (backupTask != null)
            {
                result.Link = task.Status as string;
                result.ExpireDate = backupTask.ExpireDate;
            }
            return result;
        }

        private static void Clean(object period)
        {
            try
            {
                lock (tasks.SynchRoot)
                {
                    tasks.GetItems()
                        .OfType<BackupTask>()
                        .Where(t => t.ExpireDate < DateTime.UtcNow)
                        .ToList()
                        .ForEach(tasks.Remove);

                    tasks.GetItems()
                         .OfType<TransferTask>()
                         .Where(t => t.IsCompleted)
                         .ToList()
                         .ForEach(tasks.Remove);

                    tasks.GetItems()
                         .OfType<RestoreTask>()
                         .Where(t => t.IsCompleted)
                         .ToList()
                         .ForEach(tasks.Remove);
                }
                GetStore().DeleteExpired(string.Empty, string.Empty, (TimeSpan)period);
            }
            catch (Exception error)
            {
                log.Error(error);
            }
        }

        private static string ToAbsolute(string basePath)
        {
            if (!Path.IsPathRooted(basePath))
                basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), basePath);

            return basePath;
        }

        private static IDataStore GetStore()
        {
            return StorageFactory.GetStorage(currentWebConfigPath, "backupfiles", "backup");
        }

        #region Progress items

        private class BackupTask : IProgressItem
        {
            private readonly Guid userId;

            public object Id { get; set; }

            public int Tenant { get; set; }

            public bool IsCompleted { get; set; }

            public object Status { get; set; }

            public object Error { get; set; }

            public double Percentage { get; set; }

            public DateTime ExpireDate { get; set; }

            public BackupTask(int tenant, Guid userId)
            {
                Id = Guid.NewGuid().ToString("N");
                Tenant = tenant;
                this.userId = userId;
                ExpireDate = DateTime.MaxValue;
            }

            public void RunJob()
            {
                var filename = Path.Combine(tmpfolder, Id + ".tbm");
                try
                {
                    if (!Directory.Exists(tmpfolder))
                    {
                        Directory.CreateDirectory(tmpfolder);
                    }

                    var backuper = new BackupManager(filename, currentWebConfigPath);
                    backuper.ProgressChanged += (o, e) =>
                    {
                        Percentage = Math.Max(0, Math.Min((int)e.Progress / 2, 50));
                    };

                    backuper.Save(Tenant);

                    using (var stream = new FileStream(filename, FileMode.Open))
                    using (var progressStream = new ProgressStream(stream))
                    {
                        progressStream.OnReadProgress += (o, e) =>
                        {
                            Percentage = Math.Max(0, Math.Min(100, 50 + e / 2));
                        };

                        var uploadname = string.Format("{0}-{1:yyyyMMdd-HHmmss}.zip", CoreContext.TenantManager.GetTenant(Tenant).TenantDomain, DateTime.UtcNow).ToLowerInvariant();
                        ExpireDate = DateTime.UtcNow.Add(expire);
                        Status = GetStore().SavePrivate(string.Empty, uploadname, progressStream, ExpireDate);
                    }

                    IsCompleted = true;
                    Percentage = 100;

                    NotifyHelper.SendAboutBackupCompleted(Tenant, userId, (string)Status, ExpireDate);
                }
                catch (Exception e)
                {
                    Error = e;
                    log.Error(e);
                }
                finally
                {
                    try
                    {
                        if (File.Exists(filename))
                        {
                            File.Delete(filename);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(e);
                    }
                }
            }

            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        private class TransferTask : IProgressItem
        {
            private readonly Tenant _tenant;
            private readonly string _targetRegion;
            private readonly bool _notifyAllUsers;
            private readonly bool _backupMail;

            public object Id { get; set; }

            public object Status { get; set; }

            public double Percentage { get; set; }

            public bool IsCompleted { get; set; }

            public object Error { get; set; }

            public TransferTask(TransferRequest request)
            {
                _tenant = CoreContext.TenantManager.GetTenant(request.TenantId);
                _targetRegion = request.TargetRegion;
                _notifyAllUsers = request.NotifyUsers;
                _backupMail = request.BackupMail;
                Id = request.TenantId;
            }

            public void RunJob()
            {
                try
                {
                    NotifyHelper.SendAboutTransferStart(_tenant.TenantId, _targetRegion, _notifyAllUsers);

                    string targetWebConfigPath = ToAbsolute(BackupConfigurationSection.GetSection().RegionConfigs.GetConfig(_targetRegion).Path);

                    var transferTask = new TransferPortalTask(_tenant, currentWebConfigPath, targetWebConfigPath) {BackupDirectory = tmpfolder};

                    if (!_backupMail)
                        transferTask.IgnoreModule(ModuleName.Mail);

                    transferTask.ProgressChanged += (sender, args) => Percentage = args.Progress;
                    transferTask.Message += (sender, args) =>
                        {
                            if (args.Reason == MessageReason.Info)
                            {
                                log.Debug(args.Message);
                            }
                            else if (args.Reason == MessageReason.Warning)
                            {
                                log.Warn(args.Message);
                            }
                        };

                    transferTask.Run();

                    NotifyHelper.SendAboutTransferComplete(_tenant.TenantId, _targetRegion, GetPortalAddress(targetWebConfigPath), _notifyAllUsers);
                }
                catch (Exception error)
                {
                    log.Error(error);
                    NotifyHelper.SendAboutTransferError(_tenant.TenantId, _targetRegion, GetPortalAddress(currentWebConfigPath), error.Message, _notifyAllUsers);
                    Error = error;
                }
                finally
                {
                    IsCompleted = true;
                }
            }

            private string GetPortalAddress(string configPath)
            {
                var baseDomain = ConfigurationHelper.OpenConfiguration(configPath).AppSettings.Settings["core.base-domain"].Value;
                return string.Format("http://{0}.{1}", _tenant.TenantAlias, baseDomain);
            }

            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        private class RestoreTask : IProgressItem
        {
            private readonly int _tenantID;
            private readonly string _fromFilePath;

            public object Id { get; set; }

            public object Status { get; set; }

            public double Percentage { get; set; }

            public bool IsCompleted { get; set; }

            public object Error { get; set; }

            public RestoreTask(int tenantID, string fromFilePath)
            {
                Id = tenantID;
                _tenantID = tenantID;
                _fromFilePath = fromFilePath;
            }

            public void RunJob()
            {
                //download demo data to temporary file
                var tempFilePath = Path.Combine(tmpfolder, _fromFilePath);
                try
                {
                    if (!File.Exists(tempFilePath))
                    {
                        using (var fs = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            var store = StorageFactory.GetStorage(currentWebConfigPath, "demodata", "demo", null, null);
                            store.GetReadStream(_fromFilePath).StreamCopyTo(fs);
                        }
                    }

                    var restoreTask = new RestorePortalTask(currentWebConfigPath, tempFilePath, ColumnMapper.ForRestoreDemoPortal(_tenantID));
                    restoreTask.IgnoreTable("tenants_tenants");
                    restoreTask.IgnoreTable("tenants_tariff");
                    restoreTask.IgnoreModule(ModuleName.Mail);
                    restoreTask.IgnoreModule(ModuleName.WebStudio);

                    restoreTask.ProgressChanged += (sender, args) => Percentage = args.Progress;
                    restoreTask.Message += (sender, args) =>
                        {
                            if (args.Reason == MessageReason.Info)
                            {
                                log.Debug(args.Message);
                            }
                            else if (args.Reason == MessageReason.Warning)
                            {
                                log.Warn(args.Message);
                            }
                        };

                    restoreTask.Run();
                }
                catch (Exception error)
                {
                    log.Error(error);
                    Error = error;
                }
                finally
                {
                    IsCompleted = true;
                }
            }

            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        #endregion
    }
}
