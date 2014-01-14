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

namespace ASC.Core.Common.Notify.Push
{
    public class PushServiceClient : BaseWcfClient<IPushService>, IPushService
    {
        public string RegisterDevice(int tenantID, string userID, string token, DeviceType type)
        {
            return Channel.RegisterDevice(tenantID, userID, token, type);
        }

        public void DeregisterDevice(int tenantID, string userID, string token)
        {
            Channel.DeregisterDevice(tenantID, userID, token);
        }

        public void EnqueueNotification(int tenantID, string userID, PushNotification notification, List<string> targetDevices)
        {
            Channel.EnqueueNotification(tenantID, userID, notification, targetDevices);
        }

        public List<PushNotification> GetFeed(int tenantID, string userID, string deviceToken, DateTime from, DateTime to)
        {
            return Channel.GetFeed(tenantID, userID, deviceToken, from, to);
        }
    }
}
