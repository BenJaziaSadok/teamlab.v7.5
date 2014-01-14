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
using ASC.Core.Users;

namespace ASC.Forum
{
    public interface INotifierView
    {
        event EventHandler<NotifyEventArgs> SendNotify;
    }

    public class NotifyEventArgs : EventArgs
    {
        public INotifyAction NotifyAction { get; set; }

        public string ObjectID { get; private set; }

        public string ThreadURL { get; set; }
        public string TopicURL { get; set; }
        public string PostURL { get; set; }
        public string TagURL { get; set; }
        public string UserURL { get; set; }
        public string Date { get; set; }

        public string ThreadTitle { get; set; }
        public string TopicTitle { get; set; }

        public UserInfo Poster { get; set; }

        public string PostText { get; set; }
        public string TagName { get; set; }
        public int TopicId { get; set; }
        public int PostId { get; set; }
        public int TenantId { get; set; }


        public NotifyEventArgs(INotifyAction notifyAction, string objectID)
        {
            this.NotifyAction = notifyAction;
            this.ObjectID = objectID;           
        }
    }
}
