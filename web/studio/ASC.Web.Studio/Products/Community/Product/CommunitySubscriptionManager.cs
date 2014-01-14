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
using ASC.Web.Core.Subscriptions;

namespace ASC.Web.Community.Product
{
    public class CommunitySubscriptionManager : IProductSubscriptionManager
    {
        #region IProductSubscriptionManager Members

        public System.Collections.Generic.List<SubscriptionGroup> GetSubscriptionGroups()
        {
            return new System.Collections.Generic.List<SubscriptionGroup>();
        }

        public GroupByType GroupByType
        {
            get { return GroupByType.Modules; }
        }

        #endregion

        #region ISubscriptionManager Members

        public List<SubscriptionObject> GetSubscriptionObjects(Guid subItem)
        {
            return null;
        }

        public System.Collections.Generic.List<SubscriptionType> GetSubscriptionTypes()
        {
            return null;
        }

        public ASC.Notify.Model.ISubscriptionProvider SubscriptionProvider
        {
            get { return null; }
        }

        #endregion

    }
}
