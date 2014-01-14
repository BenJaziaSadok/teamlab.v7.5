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
using ASC.Notify.Model;
using System.Collections.Generic;

namespace ASC.Web.Core.Subscriptions
{
    public delegate bool IsEmptySubscriptionTypeDelegate(Guid productID, Guid moduleOrGroupID, Guid typeID);

    public delegate List<SubscriptionObject> GetSubscriptionObjectsDelegate(Guid productID, Guid moduleOrGroupID, Guid typeID);

    public class SubscriptionType
    {
        public INotifyAction NotifyAction { get; set; }

        public Guid ID { get; set; }

        public string Name { get; set; }

        public bool Single { get; set; }

        public bool CanSubscribe { get; set; }

        public IsEmptySubscriptionTypeDelegate IsEmptySubscriptionType;

        public GetSubscriptionObjectsDelegate GetSubscriptionObjects;
        
    }
}
