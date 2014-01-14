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
using ASC.Common.Module;

namespace ASC.Core.Common.Contracts
{
    public class BackupServiceClient : BaseWcfClient<IBackupService>, IBackupService
    {
        public BackupResult CreateBackup(int tenantID, Guid userID)
        {
            return Channel.CreateBackup(tenantID, userID);
        }

        public BackupResult GetBackupStatus(string id)
        {
            return Channel.GetBackupStatus(id);
        }

        public BackupResult TransferPortal(TransferRequest request)
        {
            return Channel.TransferPortal(request);
        }

        public BackupResult GetTransferStatus(int tenantID)
        {
            return Channel.GetTransferStatus(tenantID);
        }

        public BackupResult RestorePortal(int tenantID, string demoID)
        {
            return Channel.RestorePortal(tenantID, demoID);
        }

        public BackupResult GetRestoreStatus(int tenantID)
        {
            return Channel.GetRestoreStatus(tenantID);
        }

        public List<TransferRegion> GetTransferRegions()
        {
            return Channel.GetTransferRegions();
        }

        public List<string> GetDemoData()
        {
            return Channel.GetDemoData();
        }
    }
}