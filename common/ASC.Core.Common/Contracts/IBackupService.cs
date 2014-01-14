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
using System.ServiceModel;

namespace ASC.Core.Common.Contracts
{
    [ServiceContract]
    public interface IBackupService
    {
        [OperationContract]
        BackupResult CreateBackup(int tenantID, Guid userID);

        [OperationContract]
        BackupResult GetBackupStatus(string id);

        [OperationContract]
        BackupResult TransferPortal(TransferRequest request);

        [OperationContract]
        BackupResult GetTransferStatus(int tenantID);

        [OperationContract]
        BackupResult RestorePortal(int tenantID, string demoID);

        [OperationContract]
        BackupResult GetRestoreStatus(int tenantID);

        [OperationContract]
        List<TransferRegion> GetTransferRegions();

        [OperationContract]
        List<string> GetDemoData();
    }
}
