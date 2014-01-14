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
using ASC.Notify.Model;
using ASC.Web.Community.Birthdays.Resources;
using ASC.Web.Core.Subscriptions;

namespace ASC.Web.Community.Birthdays
{
    public class BirthdaysSubscriptionManager : ISubscriptionManager
    {
        public List<SubscriptionObject> GetSubscriptionObjects(Guid subItem)
        {
            return new List<SubscriptionObject>();
        }

        public List<SubscriptionType> GetSubscriptionTypes()
        {
            var subscriptionTypes = new List<SubscriptionType>();

            subscriptionTypes.Add(new SubscriptionType()
            {
                ID = new Guid("{3177E937-9189-45db-BA7C-916C69C4A574}"),
                Name = BirthdaysResource.BirthdaysSubscribeAll,
                NotifyAction = BirthdaysNotifyClient.Event_BirthdayReminder,
                Single = true,
                IsEmptySubscriptionType = new IsEmptySubscriptionTypeDelegate(IsEmptySubscriptionType),
                CanSubscribe = true
            });
            return subscriptionTypes;
        }

        private bool IsEmptySubscriptionType(Guid productID, Guid moduleID, Guid typeID)
        {
            return false;
        }

        public ISubscriptionProvider SubscriptionProvider
        {
            get { return BirthdaysNotifyClient.NotifySource.GetSubscriptionProvider(); }
        }
    }
}
