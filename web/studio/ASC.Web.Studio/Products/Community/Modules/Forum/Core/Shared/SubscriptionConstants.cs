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

using ASC.Forum.Module;
using ASC.Notify.Model;
using ASC.Notify;

namespace ASC.Forum
{
    public static class SubscriptionConstants
    {
        public static INotifyAction NewPostInTopic { get { return Constants.NewPostInTopic; } }

        public static INotifyAction NewPostInThread { get { return Constants.NewPostInThread; } }

        public static INotifyAction NewPostByTag { get { return Constants.NewPostByTag; } }

        public static INotifyAction NewTopicInForum { get { return Constants.NewTopicInForum; } }

        public static ISubscriptionProvider SubscriptionProvider { get { return ForumNotifySource.Instance.GetSubscriptionProvider(); } }

        public static INotifyClient NotifyClient { get { return ForumNotifyClient.NotifyClient; } }

        public static string SyncName { get { return "asc_forum"; } }

        
        
    }
}
