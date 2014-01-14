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
using System.Linq;

namespace ASC.Core
{
    class ClientSubscriptionManager : ISubscriptionManagerClient
    {
        private readonly ISubscriptionService service;


        public ClientSubscriptionManager(ISubscriptionService service)
        {
            if (service == null) throw new ArgumentNullException("subscriptionManager");
            this.service = service;
        }


        public void Subscribe(string sourceID, string actionID, string objectID, string recipientID)
        {
            var s = new SubscriptionRecord
            {
                Tenant = GetTenant(),
                SourceId = sourceID,
                ActionId = actionID,
                RecipientId = recipientID,
                ObjectId = objectID,
                Subscribed = true,
            };
            service.SaveSubscription(s);
        }

        public void Unsubscribe(string sourceID, string actionID, string objectID, string recipientID)
        {
            var s = new SubscriptionRecord
            {
                Tenant = GetTenant(),
                SourceId = sourceID,
                ActionId = actionID,
                RecipientId = recipientID,
                ObjectId = objectID,
                Subscribed = false,
            };
            service.SaveSubscription(s);
        }

        public void UnsubscribeAll(string sourceID, string actionID, string objectID)
        {
            service.RemoveSubscriptions(GetTenant(), sourceID, actionID, objectID);
        }

        public void UnsubscribeAll(string sourceID, string actionID)
        {
            service.RemoveSubscriptions(GetTenant(), sourceID, actionID);
        }

        public string[] GetSubscriptionMethod(string sourceID, string actionID, string recipientID)
        {
            var m = service.GetSubscriptionMethods(GetTenant(), sourceID, actionID, recipientID)
                .FirstOrDefault(x => x.ActionId.Equals(actionID, StringComparison.OrdinalIgnoreCase));
            if (m == null)
            {
                m = service.GetSubscriptionMethods(GetTenant(), sourceID, actionID, recipientID).FirstOrDefault();
            }
            if (m == null)
            {
                m = service.GetSubscriptionMethods(GetTenant(), sourceID, actionID, Guid.Empty.ToString()).FirstOrDefault();
            }
            return m != null ? m.Methods : new string[0];
        }

        public string[] GetRecipients(string sourceID, string actionID, string objectID)
        {
            return service.GetSubscriptions(GetTenant(), sourceID, actionID, null, objectID)
                .Where(s => s.Subscribed)
                .Select(s => s.RecipientId)
                .ToArray();
        }

        public string[] GetSubscriptions(string sourceID, string actionID, string recipientID)
        {
            return service.GetSubscriptions(GetTenant(), sourceID, actionID, recipientID, null)
                .Where(s => s.Subscribed)
                .Select(s => s.ObjectId)
                .ToArray();
        }

        public bool IsUnsubscribe(string sourceID, string recipientID, string actionID, string objectID)
        {
            var s = service.GetSubscription(GetTenant(), sourceID, actionID, recipientID, objectID);
            if (s == null && !string.IsNullOrEmpty(objectID))
            {
                s = service.GetSubscription(GetTenant(), sourceID, actionID, recipientID, null);
            }
            return s != null && !s.Subscribed;
        }

        public void UpdateSubscriptionMethod(string sourceID, string actionID, string recipientID, string[] senderNames)
        {
            var m = new SubscriptionMethod
            {
                Tenant = GetTenant(),
                SourceId = sourceID,
                ActionId = actionID,
                RecipientId = recipientID,
                Methods = senderNames,
            };
            service.SetSubscriptionMethod(m);
        }


        private int GetTenant()
        {
            return CoreContext.TenantManager.GetCurrentTenant().TenantId;
        }
    }
}
