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

namespace ASC.Forum
{
    public interface ISubscriptionGetcherView
    {
        IList<object> SubscriptionObjects { get; set; }

        event EventHandler<SubscriptionEventArgs> GetSubscriptionObjects;
    }

    public class SubscriptionEventArgs: EventArgs
    {
        public Guid UserID { get; private set; }

        public int TenantID { get; private set; }

        public INotifyAction NotifyAction { get; private set; }

        public SubscriptionEventArgs(INotifyAction notifyAction, Guid userID, int tenantID)
        {
            this.NotifyAction = notifyAction;         
            this.UserID = userID;
            this.TenantID = tenantID;
        }
    }
}
