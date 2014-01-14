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

namespace ASC.Core.Common.Notify.Push
{
    [ServiceContract]
    public interface IPushService
    {
        [OperationContract]
        string RegisterDevice(int tenantID, string userID, string token, DeviceType type);

        [OperationContract]
        void DeregisterDevice(int tenantID, string userID, string token);

        [OperationContract]
        void EnqueueNotification(int tenantID, string userID, PushNotification notification, List<string> targetDevices);

        [OperationContract]
        List<PushNotification> GetFeed(int tenantID, string userID, string deviceToken, DateTime from, DateTime to);
    }
}
