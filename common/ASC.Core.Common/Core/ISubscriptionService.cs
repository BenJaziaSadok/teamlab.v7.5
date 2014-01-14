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

using System.Collections.Generic;

namespace ASC.Core
{
    public interface ISubscriptionService
    {
        IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId);

        IEnumerable<SubscriptionRecord> GetSubscriptions(int tenant, string sourceId, string actionId, string recipientId, string objectId);

        SubscriptionRecord GetSubscription(int tenant, string sourceId, string actionId, string recipientId, string objectId);

        void SaveSubscription(SubscriptionRecord s);

        void RemoveSubscriptions(int tenant, string sourceId, string actionId);

        void RemoveSubscriptions(int tenant, string sourceId, string actionId, string objectId);


        IEnumerable<SubscriptionMethod> GetSubscriptionMethods(int tenant, string sourceId, string actionId, string recipientId);

        void SetSubscriptionMethod(SubscriptionMethod m);
    }
}
