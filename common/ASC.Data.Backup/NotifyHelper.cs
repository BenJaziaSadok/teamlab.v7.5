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
using ASC.Core.Notify;
using log4net;

namespace ASC.Data.Backup
{
    internal static class NotifyHelper
    {
        private const string NotifyService = "ASC.Web.Studio.Core.Notify.StudioNotifyService, ASC.Web.Studio";
        private const string MethodTransferStart = "MigrationPortalStart";
        private const string MethodTransferCompleted = "MigrationPortalSuccess";
        private const string MethodTransferError = "MigrationPortalError";
        private const string MethodBackupCompleted = "SendMsgBackupCompleted";

        public static void SendAboutTransferStart(int tenantId, string targetRegion, bool notifyUsers)
        {
            SendNotification(MethodTransferStart, tenantId, targetRegion, notifyUsers);
        }

        public static void SendAboutTransferComplete(int tenantId, string targetRegion, string targetAddress, bool notifyUsers)
        {
            SendNotification(MethodTransferCompleted, tenantId, targetRegion, targetAddress, notifyUsers);
        }

        public static void SendAboutTransferError(int tenantId, string targetRegion, string resultAddress, string errorText, bool notifyUsers)
        {
            SendNotification(MethodTransferError, tenantId, targetRegion, resultAddress, notifyUsers);
        }

        public static void SendAboutBackupCompleted(int tenantId, Guid userId, string link, DateTime expireDate)
        {
            SendNotification(MethodBackupCompleted, tenantId, userId, link, expireDate);
        }

        private static void SendNotification(string method, int tenantId, params object[] args)
        {
            try
            {
                using (var notifyClient = new NotifyServiceClient())
                {
                    notifyClient.InvokeSendMethod(NotifyService, method, tenantId, args);
                }
            }
            catch (Exception error)
            {
                LogManager.GetLogger("ASC.Backup").Warn("Error while sending notification", error);
            }
        }
    }
}
