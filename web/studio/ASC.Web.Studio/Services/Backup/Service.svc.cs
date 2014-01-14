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

using ASC.Core;
using ASC.Core.Common.Contracts;
using ASC.Web.Core;
using ASC.Web.Studio.Utility;
using Resources;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;

// The following line sets the default namespace for DataContract serialized typed to be ""
[assembly: ContractNamespace("", ClrNamespace = "ASC.Web.Studio.Services.Backup")]
namespace ASC.Web.Studio.Services.Backup
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service : IBackupService
    {
        public BackupRequest RequestBackup()
        {
            try
            {
                CheckPermission();

                using (var client = new BackupServiceClient())
                {
                    var result = client.CreateBackup(
                        TenantProvider.CurrentTenantID,
                        SecurityContext.CurrentAccount.ID);

                    return ToBackupRequest(result);
                }
            }
            catch (Exception e)
            {
                return new BackupRequest { Status = BackupRequestStatus.Error, FileLink = e.Message };
            }
        }

        public BackupRequest GetBackupStatus(string id)
        {
            try
            {
                CheckPermission();

                using (var client = new BackupServiceClient())
                {
                    var result = client.GetBackupStatus(id);
                    if (result == null)
                    {
                        throw GenerateException(HttpStatusCode.NotFound, "Not found", new Exception("item not found"));
                    }

                    return ToBackupRequest(result);
                }
            }
            catch (Exception e)
            {
                return new BackupRequest { Status = BackupRequestStatus.Error, FileLink = e.Message };
            }
        }

        private BackupRequest ToBackupRequest(BackupResult result)
        {
            return new BackupRequest
            {
                Id = result.Id,
                Completed = result.Completed,
                FileLink = result.Link,
                Percentdone = result.Percent,
                Status = BackupRequestStatus.Started,
            };
        }


        private void CheckPermission()
        {
            if (!SecurityContext.IsAuthenticated)
            {
                try
                {
                    if (!TenantExtra.GetTenantQuota().HasBackup)
                    {
                        throw new Exception(Resource.ErrorNotAllowedOption);
                    }
                    if (!SecurityContext.AuthenticateMe(CookiesManager.GetCookies(CookiesType.AuthKey)))
                    {
                        throw GenerateException(HttpStatusCode.Unauthorized, "Unauthorized", null);
                    }
                    else
                    {
                        if (!CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, ASC.Core.Users.Constants.GroupAdmin.ID))
                        {
                            throw GenerateException(HttpStatusCode.Unauthorized, "Permission denied", null);
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw GenerateException(HttpStatusCode.Unauthorized, "Unauthorized", exception);
                }
            }
        }

        private HttpException GenerateException(HttpStatusCode code, string message, Exception inner)
        {
            return new HttpException((int)code, message, inner);
        }
    }
}