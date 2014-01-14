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

namespace ASC.Forum
{
    public interface ISubscriberView
    {
        bool IsSubscribe { get; set; }

        event EventHandler<SubscribeEventArgs> Subscribe;

        event EventHandler<SubscribeEventArgs> UnSubscribe;

        event EventHandler<SubscribeEventArgs> GetSubscriptionState;
    }

    public class SubscribeEventArgs : EventArgs
    {
        public INotifyAction NotifyAction { get; private set; }

        public string ObjectID { get; private set; }

        public Guid UserID { get; private set; }       
        
        public SubscribeEventArgs(INotifyAction notifyAction, string objectID, Guid userID)
        {
            this.NotifyAction = notifyAction;
            this.ObjectID = objectID;
            this.UserID = userID;            
        }
    }
}
