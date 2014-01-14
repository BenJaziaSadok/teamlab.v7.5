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
using ASC.Forum.Module;
using ASC.Notify.Recipients;

namespace ASC.Forum
{
    internal class SubscriptionGetcherPresenter : PresenterTemplate<ISubscriptionGetcherView>
    {
        protected override void RegisterView()
        {
            _view.GetSubscriptionObjects += new EventHandler<SubscriptionEventArgs>(GetSubscriptionObjectsHandler);            
        }

        private void GetSubscriptionObjectsHandler(object sender, SubscriptionEventArgs e)
        {
            var recipient = (IDirectRecipient)ForumNotifySource.Instance.GetRecipientsProvider().GetRecipient(e.UserID.ToString());
            if (recipient == null)
            {
                _view.SubscriptionObjects = new List<object>(0);
                return;
            }

            List<string> objIDs = new List<string>(ForumNotifySource.Instance.GetSubscriptionProvider().GetSubscriptions(e.NotifyAction, recipient));

            if (objIDs == null || objIDs.Count == 0)
            {
                _view.SubscriptionObjects = new List<object>(0);
                return;
            }

            if (String.Equals(e.NotifyAction.ID, SubscriptionConstants.NewPostInTopic.ID, StringComparison.InvariantCultureIgnoreCase))
            {
                _view.SubscriptionObjects = ForumDataProvider.GetTopicsByIDs(e.TenantID, objIDs.ConvertAll<int>(id => Convert.ToInt32(id)), false)
                                            .ConvertAll<object>(topic => (object)topic);
            }

            else if (String.Equals(e.NotifyAction.ID, SubscriptionConstants.NewPostInThread.ID, StringComparison.InvariantCultureIgnoreCase))
            {
                List<ThreadCategory> categories = null;
                List<Thread> threads = null;

                ForumDataProvider.GetThreadCategories(e.TenantID, false, out categories, out threads);
                threads.RemoveAll(tid => (objIDs.Find(id => id == tid.ID.ToString()) == null));

                _view.SubscriptionObjects = threads.ConvertAll<object>(thread => (object)thread);
            }

            else if (String.Equals(e.NotifyAction.ID, SubscriptionConstants.NewPostByTag.ID, StringComparison.InvariantCultureIgnoreCase))
            {
                _view.SubscriptionObjects = ForumDataProvider.GetTagByIDs(e.TenantID, objIDs.ConvertAll<int>(id => Convert.ToInt32(id)))
                                         .ConvertAll<object>(tag => (object)tag);
            }

            else if (String.Equals(e.NotifyAction.ID, SubscriptionConstants.NewTopicInForum.ID, StringComparison.InvariantCultureIgnoreCase))
            {
                if (objIDs != null && objIDs.Count == 1 && objIDs[0] == null)
                {
                    var objList = new List<object>();
                    objList.Add(null);
                    _view.SubscriptionObjects = objList;
                }
            }

        }
    }
}
