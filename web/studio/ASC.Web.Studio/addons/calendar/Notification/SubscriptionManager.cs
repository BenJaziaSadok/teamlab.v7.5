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
using ASC.Web.Calendar.Notification;
using ASC.Web.Core.Subscriptions;
using ASC.Api.Calendar.Notification;

namespace ASC.Web.Calendar.Notification
{
    public class SubscriptionManager : IProductSubscriptionManager
    {
        public SubscriptionType CalendarSharingSubscription { get {
            return new SubscriptionType() {
                ID = new Guid("{1CE43C40-72F4-4265-A4C6-8B55E29DB447}"),
                Name = Resources.CalendarAddonResource.CalendarSharingSubscription,
                NotifyAction = CalendarNotifySource.CalendarSharing,
                Single = true,
                CanSubscribe = true};
        } }

        public SubscriptionType EventAlertSubscription
        {
            get
            {
                return new SubscriptionType()
                {
                    ID = new Guid("{862D17FE-7119-4aa0-B1AA-E3C23097CB69}"),
                    Name = Resources.CalendarAddonResource.EventAlertSubscription,
                    NotifyAction = CalendarNotifySource.EventAlert,
                    Single = true,
                    CanSubscribe = true
                };
            }
        }

        #region ISubscriptionManager Members

        public List<SubscriptionObject> GetSubscriptionObjects(Guid subItem)
        {
            return new List<SubscriptionObject>();
        }

        public List<SubscriptionType> GetSubscriptionTypes()
        {
            return new List<SubscriptionType>() { CalendarSharingSubscription, EventAlertSubscription};
        }

        public ASC.Notify.Model.ISubscriptionProvider SubscriptionProvider
        {
            get { return CalendarNotifySource.Instance.GetSubscriptionProvider(); }
        }

        #endregion

        #region IProductSubscriptionManager Members

        public List<SubscriptionGroup> GetSubscriptionGroups()
        {
            return new List<SubscriptionGroup>();
        }

        public GroupByType GroupByType
        {
            get { return GroupByType.Simple; }
        }

        #endregion
    }
}
