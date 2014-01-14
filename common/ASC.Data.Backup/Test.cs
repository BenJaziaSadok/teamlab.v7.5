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

#if DEBUG
using ASC.Core;
using ASC.Data.Backup.Tasks;
using ASC.Data.Backup.Tasks.Modules;
using log4net;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[assembly: XmlConfigurator]

namespace ASC.Data.Backup
{
	[TestClass]
	public class Test
	{
		private static ILog log = LogManager.GetLogger("ASC.Data.Backup");


		[TestMethod]
		public void DatabaseBackupTest()
		{
			var backup = CreateBackupManager();
			backup.RemoveBackupProvider("Files");
			backup.Save(1);
		}

		[TestMethod]
		public void BackupTest()
		{
			var backup = CreateBackupManager(@"..\..\..\..\..\_ci\deploy\webstudio\Web.config");
			backup.Save(0);
		}

		private BackupManager CreateBackupManager(string config)
		{
			var backup = new BackupManager("Backup.zip", config);
			backup.ProgressChanged += ProgressChanged;
			return backup;
		}

		private BackupManager CreateBackupManager()
		{
			return CreateBackupManager(null);
		}


		private void ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (e.Progress == -1d)
			{
				log.InfoFormat("\r\n\r\n{0}", e.Status);
			}
			else
			{
				log.InfoFormat("{0}% / {1}", e.Progress, e.Status);
			}
		}
	}

    [TestClass]
    public class TransferTests
    {
        [TestMethod]
        public void TransferPortalTest()
        {
            var transferTask = new TransferPortalTask(
                CoreContext.TenantManager.GetTenant(0),
                @"..\..\Tests\Configs\localhost\Web.config",
                @"..\..\Tests\Configs\restore\Web.config");

            transferTask.IgnoreModule(ModuleName.Mail);

            transferTask.BlockOldPortalAfterStart = false;
            transferTask.DeleteOldPortalAfterCompletion = false;
            transferTask.ProcessStorage = false;
            transferTask.DeleteBackupFileAfterCompletion = false;

            transferTask.Message += (sender, args) => Console.WriteLine("{0}: {1}", args.Reason.ToString("g"), args.Message);
            transferTask.ProgressChanged += (sender, args) => Console.WriteLine("progress: {0}%", args.Progress);

            transferTask.Run();
        }
    }
}
#endif